using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuSript : MonoBehaviour {

    private bool pause = false;

    [SerializeField]
    private PlayerController playerMovement;

    [SerializeField]
    private PlayerWeapon playerShooting;

    [SerializeField]
    private CameraController playerCameraController;

    public GameObject pausePanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        pause = pausePanel.activeInHierarchy;
        playerShooting.enabled = pause;
        playerMovement.enabled = pause;
        playerCameraController.enabled = pause;
        if (pause == false)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        pausePanel.SetActive(!pause);
    }
}
