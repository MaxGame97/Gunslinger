using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that WILL cast ray from camera to crosshair. Can be used to aim the player at the correct surface in the world.
// Implement it if you need it, otherwise I'll do it after what I'm currently working on.

public class CastAimRay : MonoBehaviour
{
    /*
    private Transform cameraTransform;
    [HideInInspector]
    public Vector3 aimingPoint;

    
    private void Awake ()
    {
        cameraTransform = gameObject.transform.Find ("Main Camera");
    }

    private void Update ()
    {
        CastForwardRay ();
    }

    // Raycast from camera, and straight forward. To be used to change player aiming to where crosshair is pointing.
    private void CastForwardRay ()
    {
        RaycastHit forwardRay;

        if (Physics.Raycast (cameraTransform.position, Vector3.forward, out forwardRay))
        {
            print ("Found an object - coordinates: " + forwardRay.point);
            aimingPoint = forwardRay.point;
        }
    }
    */
}
