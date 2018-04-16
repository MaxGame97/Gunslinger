using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuSript : MonoBehaviour {

    //Determines if the game is curently paused
    private bool pause = false;

    //Local Player
    public GameObject player;

    private PlayerWeapon playerWeapon;

    private PlayerController playerController;

    private CameraController cameraController;

    //The gameObject which holds the pausemenu
    public GameObject pausePanel;

    private void Update()
    {
        //Checks for input, this could be moved somewhere else and it should probably check for button input instead
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        //Checks if the pausemenu is active or not
        pause = pausePanel.activeInHierarchy;
        if (playerController != null)
            playerController.enabled = pause;
        if (playerWeapon != null)
            playerWeapon.enabled = pause;
        if (cameraController != null)
            cameraController.enabled = pause;
        //Unlocks the mouse if the menu is active and locks it again if it is inactive
        if (!pause)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Toggles the state of the menu
        pausePanel.SetActive(!pause);
    }

    public void SetOwner(GameObject owner)
    {
        player = owner;
        playerController = player.GetComponent<PlayerController>();
        playerWeapon = player.GetComponent<PlayerWeapon>();
        cameraController = player.GetComponentInChildren<CameraController>();
    }
}
