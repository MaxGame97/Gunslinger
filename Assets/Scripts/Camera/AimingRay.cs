using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that WILL cast ray from camera to crosshair. Can be used to aim the player at the correct surface in the world.
// Implement it if you need it, otherwise I'll do it after what I'm currently working on.

public class AimingRay : MonoBehaviour
{
    private Transform cameraTransform;
    [HideInInspector]
    public Vector3 aimingCoordinate;

    
    private void Awake ()
    {
        cameraTransform = gameObject.transform;
    }

    private void Update ()
    {
        CastForwardRay ();
    }

    // Raycast from camera, and straight forward. Can be used to change player aiming to where crosshair is pointing.
    private void CastForwardRay ()
    {
        RaycastHit hit;

        Debug.DrawRay (cameraTransform.position, cameraTransform.forward * 100, Color.green);
        if (Physics.Raycast (cameraTransform.position, cameraTransform.forward, out hit))
        {
            print ("Found an object - coordinates: " + hit.point);
            aimingCoordinate = hit.point;
        }
    }
}
