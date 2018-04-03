using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// Script that grabs the camera and places it in camera rig gameobject.

public class GrabCamera : NetworkBehaviour
{
    private Vector3 cameraOffset;
    

    private void Awake ()
    {
        cameraOffset = GetComponentInChildren<CameraController> ().offset;
    }

    private void Update ()
    {
        if (isLocalPlayer)
        {
            Camera.main.transform.parent = gameObject.transform.Find ("Camera Rig");
            Camera.main.transform.localPosition = new Vector3 (cameraOffset.x, cameraOffset.y, cameraOffset.z);
        }
    }
}
