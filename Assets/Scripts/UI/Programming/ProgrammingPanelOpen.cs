using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ProgrammingPanelOpen : MonoBehaviour
{
    public GameObject panel;
    public Button firstButton;

    public void OpenProgrammingPanel(InputAction.CallbackContext value) {
        if (Controller.control) {
            Controller.control = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            panel.SetActive(true);
            firstButton.Select();
        }
    }

    public void CloseProgrammingPanel() {
        Controller.control = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        panel.SetActive(false);
    }
}