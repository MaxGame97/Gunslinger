using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    public PauseMenuSript pause;

    public void ClosePauseMenuButton()
    {
        pause.TogglePauseMenu();
    }
}
