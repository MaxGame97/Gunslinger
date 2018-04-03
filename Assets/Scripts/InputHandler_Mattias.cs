using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to gather input from the player. Can be reached from the static GameManager object.
// Add as needed. If you change the whole structure, then notify me how I can reach the mouse input.

public class InputHandler_Mattias : MonoBehaviour
{
    [HideInInspector]
    public float Horizontal;
    [HideInInspector]
    public float Vertical;
    [HideInInspector]
    public Vector2 MouseInput;
    [HideInInspector]
    public bool Fire1;


    private void Update ()
    {
        Horizontal = Input.GetAxis ("Horizontal");
        Vertical = Input.GetAxis ("Vertical");
        MouseInput = new Vector2 (Input.GetAxisRaw ("Mouse X"), Input.GetAxisRaw ("Mouse Y"));
        Fire1 = Input.GetButtonDown ("Fire1");
    }
}
