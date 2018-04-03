using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleObstruction : MonoBehaviour
{
    //-------------------------
    private Transform cameraTransform;

    // Change protection level.
    public float minDistance;
    public float maxDistance;
    public float desiredDistance;


    private void Awake ()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Start ()
    {
        maxDistance = cameraTransform.position.z;
        minDistance = Mathf.Clamp (-minDistance, 0.0f, maxDistance);
    }

    private void LateUpdate ()
    {
        // Position.
        TestIfObstructed ();

        // Lerp between current localPosition.z and the new desired position.
    }

    private void TestIfObstructed ()
    {
        RaycastHit hit;

        if (Physics.Linecast (transform.position, cameraTransform.position, out hit))
        {
            //print ("Obstructed");
            // desiredDistance to the new z-position.
        }
        else
        {
            //print ("Not obstructed");
            desiredDistance = maxDistance;
        }
    }
}
