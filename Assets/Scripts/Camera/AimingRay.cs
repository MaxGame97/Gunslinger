using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that casts ray from camera to world coordinate that crosshair points at. Can be used to aim the player at this point.

public class AimingRay : MonoBehaviour
{
    private Transform cameraTransform;
    [HideInInspector]
    public Vector3 aimingCoordinate;

    
    private void Awake ()
    {
        // cameraTransform holds the transform of the player camera.
        cameraTransform = gameObject.transform;
    }

    private void Update ()
    {
        CastForwardRay ();
    }

    private void CastForwardRay ()
    {
        RaycastHit hit;

        // DrawRay can (and will) be removed. Used for debugging purposes.
        Debug.DrawRay (cameraTransform.position, cameraTransform.forward * 100, Color.green);

        // Cast a ray in forward direction from camera, which is straight to what crosshair points at.
        if (Physics.Raycast (cameraTransform.position, cameraTransform.forward, out hit))
        {
            //print ("Found an object - coordinates: " + hit.point);

            // Store the point that´s hit in aimingCoordinate. This can be referenced through "GameManager.Instance.AimingRay.aimingCoordinate".
            aimingCoordinate = hit.point;
        }
    }
}
