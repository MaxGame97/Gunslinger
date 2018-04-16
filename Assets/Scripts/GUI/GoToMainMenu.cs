using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToMainMenu : MonoBehaviour {

    public GameObject mainMenu;

    public GameObject optionsMenu;

    public void GoToOptions()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }
}
