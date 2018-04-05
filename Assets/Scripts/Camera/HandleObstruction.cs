using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleObstruction : MonoBehaviour
{
    //-------------------------
    private Transform cameraTransform;

    [SerializeField]
    private float zoomSpeed;

    // Change protection level.
    public float minDistance;
    public float maxDistance;
    public float desiredDistance;


    private void Awake ()
    {
        cameraTransform = gameObject.transform.Find ("Player Camera");
    }

    private void Start ()
    {
        maxDistance = cameraTransform.localPosition.z;
        minDistance = Mathf.Clamp (minDistance, 0.0f, Mathf.Abs (maxDistance));
        minDistance *= -1;
    }

    private void LateUpdate ()
    {
        TestIfObstructed ();
        
        //print ("Desired: " + desiredDistance);

        cameraTransform.localPosition = Vector3.Lerp (cameraTransform.localPosition, 
                                        new Vector3 (cameraTransform.localPosition.x, cameraTransform.localPosition.y, desiredDistance), Time.deltaTime * zoomSpeed);
    }

    private void TestIfObstructed ()
    {
        RaycastHit hit;

        //if (Physics.SphereCast (transform.position, 2, -transform.forward, out hit, 3))
        if (Physics.Linecast (transform.position, cameraTransform.position, out hit))
        {
            print ("Obstructed");

            Vector3 desiredVector = cameraTransform.position - hit.point;
            desiredVector = cameraTransform.localPosition - desiredVector;
            desiredDistance = Mathf.Clamp (Mathf.Abs (desiredVector.z) * -1, maxDistance, minDistance);
        }
        else
        {
            print ("Not obstructed");

            desiredDistance = maxDistance;
        }
    }
}

// Player -- hit.point.z -- cameraTransform.position.z
// 