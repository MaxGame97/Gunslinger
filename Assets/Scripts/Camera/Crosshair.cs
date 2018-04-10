using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Crosshair : MonoBehaviour
{
    [SerializeField]
    private Texture2D image;
    [SerializeField]
    private float size;
    private Vector2 screenPosition;

    private void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;

        screenPosition = new Vector2 ((Screen.width / 2) - (size / 2), (Screen.height / 2) - (size / 2));
    }

    private void Update()
    {
        if (screenPosition != new Vector2((Screen.width / 2) - (size / 2), (Screen.height / 2) - (size / 2)))   // Check that resolution hasn't changed since start.
        {
            screenPosition = new Vector2((Screen.width / 2) - (size / 2), (Screen.height / 2) - (size / 2));
            //Debug.Log("Updated crosshair resolution");
        }
    }

    private void OnGUI ()
    {
        GUI.DrawTexture (new Rect (screenPosition.x, screenPosition.y, size, size), image);
    }
}
