using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that handles the movement of the character based on argument. This contains vertical and horizontal input from player. Is called from PlayerController.
// Add or change as you want.

public class MovePlayer_Mattias : MonoBehaviour
{
    public void Move (Vector2 moveVector)
    {
        transform.position += transform.forward * moveVector.x * Time.deltaTime
            + transform.right * moveVector.y * Time.deltaTime;
    }
}
