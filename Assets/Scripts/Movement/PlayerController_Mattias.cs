using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Script that handles the general control of the local player by calling respective methods.
// Add or change as you want.

public class PlayerController_Mattias : NetworkBehaviour
{
    private InputHandler_Mattias playerInput;
    private MovePlayer_Mattias movePlayer;
    private PlayerShoot_Mattias playerShoot;

    [Header ("Movement")]
    [SerializeField]
    private float speed;


    private void Awake ()
    {
        playerInput = GameManager.Instance.InputHandler;
        movePlayer = GetComponent<MovePlayer_Mattias> ();
        playerShoot = GetComponent<PlayerShoot_Mattias> ();
    }

    private void FixedUpdate ()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        // Movement.
        Vector2 direction = new Vector2 (playerInput.Vertical, playerInput.Horizontal) * speed;
        movePlayer.Move (direction);

        // Shooting.
        if (GameManager.Instance.InputHandler.Fire1)
        {
            playerShoot.CmdFire ();
        }
    }
}
