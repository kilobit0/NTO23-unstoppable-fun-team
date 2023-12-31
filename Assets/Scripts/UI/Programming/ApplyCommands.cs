using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum BotCommands {
    Move,
    Rotate,
    Jump,
    Up,
    Down,
    Pick,
    Put,
    Attack,
    BreakIf,
    IfEnemy,
    IfWall,
    IfBox,
    IfBot
}

public class ApplyCommands : MonoBehaviour
{
    public List<int> cmds;
    private int currentBotInd = 0;
    [SerializeField] private List<Blocks> blocks = new List<Blocks>();
    [SerializeField] private List<Bots> bots = new List<Bots>();
    public GameObject[] buttons;
    public Transform commandList;
    public Image botChangeButton;
    private GameObject groundBot;
    private GameObject flyingBot;
    private GameObject battleBot;
    private GameObject heavyBot;
    private GameObject shieldBot;
    public GameObject listPrefab;
    private int currentList;
    public GameObject mainPanel;
    public TMP_Text listIndexIndicator;
    public TMP_Text listCounter;
    private bool isStartedCondition = false;
    public Animator warningCondition;
    private List<int> foundedDrones = new List<int>();

    private void Start() {
        UpdateAvaliableList();
        botChangeButton.sprite = bots[currentBotInd].img;  

        groundBot = GameObject.FindGameObjectWithTag(bots[0].name);
        flyingBot = GameObject.FindGameObjectWithTag(bots[1].name);
        battleBot = GameObject.FindGameObjectWithTag(bots[2].name);
        heavyBot  = GameObject.FindGameObjectWithTag(bots[3].name);
        shieldBot = GameObject.FindGameObjectWithTag(bots[4].name);

        if (groundBot != null) foundedDrones.Add(0);
        if (flyingBot != null) foundedDrones.Add(1);
        if (battleBot != null) foundedDrones.Add(2);
        if (heavyBot != null)  foundedDrones.Add(3);
        if (shieldBot != null) foundedDrones.Add(4);
        
        ChangeBot(false);
        mainPanel.SetActive(false);
    }

    public void Apply() {
        switch (bots[currentBotInd].name) {
            case "GroundBot":
                groundBot.GetComponent<GroundBot>().StartDoCommands(bots[currentBotInd].chosenCommands);
                break;
            case "FlightBot":
                flyingBot.GetComponent<FlightBot>().StartDoCommands(bots[currentBotInd].chosenCommands);
                break;
            case "BattleBot":
                battleBot.GetComponent<FlightBot>().StartDoCommands(bots[currentBotInd].chosenCommands);
                break;
            case "HeavyBot":
                heavyBot.GetComponent<GroundBot>().StartDoCommands(bots[currentBotInd].chosenCommands);
                break;
            case "ShieldBot":
                shieldBot.GetComponent<GroundBot>().StartDoCommands(bots[currentBotInd].chosenCommands);
                break;
        }
    }

    public void Restart() =>  SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void ChangeBot(bool clearly) {
        if (clearly) {
            bots[currentBotInd].chosenCommands = cmds;
            Delete(false);
            currentBotInd = ChangeBotIndex(currentBotInd);
        }
        else currentBotInd = foundedDrones[0];

        UpdateAvaliableList();

        botChangeButton.sprite = bots[currentBotInd].img;
        foreach (int i in bots[currentBotInd].chosenCommands) {
            AppedNewCommand(i);
        }
    }

    private int ChangeBotIndex(int value) {
        if (foundedDrones.Count > 1) {
            int k = foundedDrones.IndexOf(currentBotInd);
            k++;
            if (k >= foundedDrones.Count) k = 0;
            return foundedDrones[k];
        }
        else return value;
    }

    public void Delete(bool isAll) {
        if (isAll) bots[currentBotInd].chosenCommands = new List<int>();
        isStartedCondition = false;
        for (int list = 0; list < commandList.childCount; list++) {
            Transform listObj = commandList.GetChild(list);
            for (int itemNum = 0; itemNum < listObj.childCount; itemNum++) {
                
                Transform obj = listObj.GetChild(itemNum).transform;
                if (obj.childCount != 0) {
                    Transform child = obj.GetChild(0).transform;
                    child.parent = null;
                    Destroy(child.gameObject);
                }
                obj.GetComponent<ColorChanging>().ChangeColor(isStartedCondition);
            }
        }
        cmds = new List<int>();
    }

    private void AppedNewCommand(int i) {
        for (int list = 0; list < commandList.childCount; list++) {
            Transform listObj = commandList.GetChild(list);
            for (int itemNum = 0; itemNum < listObj.childCount; itemNum++) {
                
                Transform obj = listObj.GetChild(itemNum).transform;
                
                if (obj.childCount == 0) {
                    if (i == (int)BotCommands.IfEnemy || i == (int)BotCommands.IfWall || i == (int)BotCommands.IfBox || i == (int)BotCommands.IfBot) {
                        if (isStartedCondition) {
                            warningCondition.Play("Open");
                            return;
                        } else isStartedCondition = true;
                    }

                    if (i == (int)BotCommands.BreakIf) isStartedCondition = false;
                    
                    GameObject newObj = Instantiate(blocks[i].prefab);
                    newObj.transform.position = obj.position;
                    newObj.transform.SetParent(obj, false);
                    newObj.name = blocks[i].name;

                    
                    obj.GetComponent<ColorChanging>().ChangeColor(isStartedCondition);
                    cmds.Add(i);
                    bots[currentBotInd].chosenCommands = cmds;
                    return;
                }
            }
        }    
    }

    public void ButtonDistributor(string buttonName) {
        for (int i = 0; i < blocks.Count; i++) {
            if (blocks[i].name == buttonName) {
                AppedNewCommand(i);
            }
        }
    }

    private void UpdateAvaliableList() {
        for (int k = 0; k < buttons.Length; k++) {
            buttons[k].SetActive(false);
        }
        foreach (int index in bots[currentBotInd].avaliableCommands) {
            buttons[index].SetActive(true);
        }
    }

    public void AddList() {
        GameObject newList = Instantiate(listPrefab);
        newList.transform.parent = commandList;
        newList.transform.localScale = new Vector3(1,1,1);

        RectTransform rt = newList.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector3(0,-280,0);
        rt.sizeDelta = new Vector2(1,550);

        newList.GetComponent<Canvas>().enabled = false;
        listCounter.text = "At all: " + commandList.childCount.ToString();
    }

    public void ChangeListIndex(bool next) {
        if (next) {
            currentList++;
            if (currentList >= commandList.childCount) currentList = 0;
        }
        else {
            currentList--;
            if (currentList < 0) currentList = commandList.childCount-1;
        }
        listIndexIndicator.text = currentList.ToString();
        for (int i = 0; i < commandList.childCount; i++) {
            Transform child = commandList.GetChild(i);
            child.gameObject.GetComponent<Canvas>().enabled = false;
        }
        commandList.GetChild(currentList).GetComponent<Canvas>().enabled = true;
    }
}

[System.Serializable]
public class Blocks {
    public string name;
    public GameObject prefab;
}

[System.Serializable]
public class Bots {
    public string name;
    public Sprite img;
    public List<int> avaliableCommands;
    public List<int> chosenCommands;
}