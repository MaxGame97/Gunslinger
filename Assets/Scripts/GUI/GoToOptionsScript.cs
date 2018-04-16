using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToOptionsScript : MonoBehaviour {

    public GameObject mainMenu;

    public GameObject optionsMenu;



    public void GoToOptions()
    {
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
}
