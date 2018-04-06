using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleObstruction : MonoBehaviour
{
    //-------------------------
    private Transform cameraTransform;

    public float zoomSpeed;

    // Change protection level!!
    public float minDistance;
    public float maxDistance;
    public float desiredDistance;


    private void Awake ()
    {
        cameraTransform = gameObject.transform.Find ("Player Camera");
    }

    private void Start ()
    {
        // Set the maximum distance allowed to the offset on the z-axis.
        maxDistance = cameraTransform.localPosition.z;
        // Set the minimum distance allowed to the negative value decided in the inspector.
        minDistance = Mathf.Clamp (minDistance, maxDistance, minDistance) * -1;
    }

    private void LateUpdate ()
    {
        TestIfObstructed ();

        // Interpolate between the cameras current local transform and the transform with the desired z-distance.
        cameraTransform.localPosition = Vector3.Lerp (cameraTransform.localPosition,
                                        new Vector3 (cameraTransform.localPosition.x, cameraTransform.localPosition.y, desiredDistance), Time.deltaTime * zoomSpeed);
    }

    private void TestIfObstructed ()
    {
        RaycastHit hit;

        // Shoot a ray between the player and the camera to look for an obstruction.
        //if (Physics.SphereCast (transform.position, 2, -transform.forward, out hit, -maxDistance))
        if (Physics.Linecast (transform.position, cameraTransform.position, out hit))
        {
            print ("Obstructed");

            // If an obstruction is found, calculate the distance the camera has to move on the z-axis.
            Vector3 difference = cameraTransform.position - hit.point;
            Vector3 desiredPosition = cameraTransform.localPosition - difference;
            desiredDistance = Mathf.Clamp (Mathf.Abs (desiredPosition.z) * -1, maxDistance, minDistance);
        }
        else
        {
            print ("Not obstructed");

            desiredDistance = maxDistance;
            // Otherwise, interpolate between the local position and the max distance allowed.
            desiredDistance = Mathf.Lerp (cameraTransform.localPosition.z, maxDistance, Time.deltaTime * zoomSpeed);
        }
    }
}

/*
private void Start ()
    {
        // Set the maximum distance allowed to the offset on the z-axis.
        maxDistance = cameraTransform.localPosition.z;
        // Set the minimum distance allowed to the negative value decided in the inspector.
        minDistance = Mathf.Clamp (minDistance, maxDistance, minDistance) * -1;
    }

    private void LateUpdate ()
    {
        TestIfObstructed ();

        //print ("Desired: " + desiredDistance);

        // Interpolate between the cameras current local transform and the transform with the desired z-distance.
        cameraTransform.localPosition = Vector3.Lerp (cameraTransform.localPosition,
                                        new Vector3 (cameraTransform.localPosition.x, cameraTransform.localPosition.y, desiredDistance), Time.deltaTime * zoomSpeed);
    }

    private void TestIfObstructed ()
    {
        RaycastHit hit;

        // Shoot a ray between the player and the camera to look for an obstruction.
        //if (Physics.SphereCast (transform.position, 2, -transform.forward, out hit, -maxDistance))
        if (Physics.Linecast (transform.position, cameraTransform.position, out hit))
        {
            print ("Obstructed");

            zoomSpeed = 10f;
            // If an obstruction is found, calculate the distance the camera has to move on the z-axis.
            Vector3 difference = cameraTransform.position - hit.point;
            desiredPosition = cameraTransform.localPosition - difference;
            desiredDistance = Mathf.Clamp (Mathf.Abs (desiredPosition.z) * -1, maxDistance, minDistance);
            //desiredPosition = new Vector3 (hit.point.x, hit.point.y, hit.point.z + hit.normal.z * 0.5f);
        }
        else
        {
            print ("Not obstructed");

            zoomSpeed = 4f;

            //desiredDistance = maxDistance;
            // Otherwise, interpolate between the local position and the max distance allowed.
            desiredDistance = Mathf.Lerp (cameraTransform.localPosition.z, maxDistance, Time.deltaTime * zoomSpeed);
        }
    }
} 
*/
