using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuSript : MonoBehaviour {

    //Determines if the game is curently paused
    private bool pause = false;

    //Needed to disable the movement while paused
    [Tooltip("Drag in the player")]
    [SerializeField]
    private PlayerController playerMovement;

    //Needed to disable the shoting while paused
    [Tooltip("Drag in the player")]
    [SerializeField]
    private PlayerWeapon playerShooting;

    //Needed to disable the camera while paused
    [Tooltip("Drag in the playerCamera")]
    [SerializeField]
    private CameraController playerCameraController;

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

        //Sets the movement behaviors to the current state of the menu
        playerShooting.enabled = pause;
        playerMovement.enabled = pause;
        playerCameraController.enabled = pause;

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
}
