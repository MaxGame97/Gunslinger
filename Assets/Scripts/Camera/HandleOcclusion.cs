using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This solution to camera collisions and occlusion raycasts the four view frustum corners, plus to a point that is set slightly behind the camera. These points in space should always be visible to the player.

public class HandleOcclusion : MonoBehaviour
{
    private Transform cameraTransform;

    private Vector3 neutralLocalPosition;
    private Vector3 desiredLocalPosition;
    private Vector3 currentWorldPosition;

    private float zoomSpeed;
    [SerializeField]
    private float minDistance;
    private float maxDistance;
    [SerializeField]
    private float moveDistance;

    private float desiredPoint;
    

    private void Awake ()
    {
        cameraTransform = gameObject.transform.Find ("Player Camera");
    }

    private void Start ()
    {
        maxDistance = Mathf.Abs (cameraTransform.localPosition.z);
        minDistance = Mathf.Clamp (minDistance, minDistance, maxDistance);
        moveDistance = 0.1f;
        neutralLocalPosition = cameraTransform.localPosition;
    }

    private void LateUpdate ()
    {
        currentWorldPosition = cameraTransform.position;

        if (CheckIfOccluded ())
        {
            print ("Occluded");

            zoomSpeed = 10f;

            if (cameraTransform.localPosition.z > -minDistance)
            {
                cameraTransform.localPosition = new Vector3 (cameraTransform.localPosition.x, cameraTransform.localPosition.y, -minDistance);
            }
            else
            {
                cameraTransform.localPosition = Vector3.Lerp (cameraTransform.localPosition,
                                                new Vector3 (cameraTransform.localPosition.x, cameraTransform.localPosition.y, cameraTransform.localPosition.z + 0.2f), Time.deltaTime * zoomSpeed); // + moveDistance
            }
        }
        else
        {
            print ("Not occluded");

            zoomSpeed = 4f;
            
            if (Input.GetKey (KeyCode.Space))
                cameraTransform.localPosition = Vector3.Lerp (cameraTransform.localPosition, neutralLocalPosition, Time.deltaTime * zoomSpeed);
        }
    }

    // Checks if it's true that the camera is occluded.
    private bool CheckIfOccluded ()
    {
        bool isOccluded = false;

        float closestDistance = CheckCameraPoints (transform.position, currentWorldPosition);

        if (closestDistance != -1)
        {
            isOccluded = true;
        }

        return isOccluded;
    }

    private float CheckCameraPoints (Vector3 from, Vector3 to)
    {
        RaycastHit hit;

        float closestDistance = -1;

        ClippPlaneManager.ClipPlanePoints clipPlanePoints = ClippPlaneManager.ClipPlaneAtNear (to);

        // Draw lines to visualize the linecasting from the player.
        Debug.DrawLine (from, to + transform.forward * Camera.main.nearClipPlane, Color.cyan);
        Debug.DrawLine (from, clipPlanePoints.DownRight, Color.blue);
        Debug.DrawLine (from, clipPlanePoints.DownLeft, Color.blue);
        Debug.DrawLine (from, clipPlanePoints.UpRight, Color.blue);
        Debug.DrawLine (from, clipPlanePoints.UpLeft, Color.blue);

        // Draw lines to visualize the clipping plane.
        Debug.DrawLine (clipPlanePoints.UpLeft, clipPlanePoints.UpRight, Color.blue);
        Debug.DrawLine (clipPlanePoints.DownLeft, clipPlanePoints.DownRight, Color.blue);
        Debug.DrawLine (clipPlanePoints.UpLeft, clipPlanePoints.DownLeft, Color.blue);
        Debug.DrawLine (clipPlanePoints.UpRight, clipPlanePoints.DownRight, Color.blue);

        // Boxcast på fyra första?

        // Check which occluded point of the five that is the nearest on z-axis.
        if (Physics.Linecast (from, clipPlanePoints.DownRight, out hit) && hit.collider.tag != "Player")
        {
            closestDistance = hit.distance;
        }
        if (Physics.Linecast (from, clipPlanePoints.DownLeft, out hit) && hit.collider.tag != "Player")
        {
            if (hit.distance > closestDistance || closestDistance == -1)
            {
                closestDistance = hit.distance;
                desiredLocalPosition = transform.InverseTransformPoint (cameraTransform.position.x, cameraTransform.position.y, hit.point.z);
            }
        }
        if (Physics.Linecast (from, clipPlanePoints.UpRight, out hit) && hit.collider.tag != "Player")
        {
            if (hit.point.z > closestDistance || closestDistance == -1)
            {
                closestDistance = hit.distance;
            }
        }
        if (Physics.Linecast (from, clipPlanePoints.UpLeft, out hit) && hit.collider.tag != "Player")
        {
            if (hit.point.z > closestDistance || closestDistance == -1)
            {
                closestDistance = hit.distance;
            }
        }
        if (Physics.Linecast (from, to + transform.forward * Camera.main.nearClipPlane, out hit) && hit.collider.tag != "Player")
        {
            if (hit.point.z > closestDistance || closestDistance == -1)
            {
                closestDistance = hit.distance;
            }
        }

        print (closestDistance);

        return closestDistance;
    }
}

/*
    Ignore raycast går att använda för objekt som är för små för att störa. Dessa kan tillåtas befinna sig mellan kamera spelare och ray-target.
*/

/*
private bool CheckIfOccluded ()
    {
        bool isOccluded = false;

        float closestDistance = CheckCameraPoints (transform.position, desiredPosition);

        if (closestDistance != -1)
        {
            isOccluded = true;
        }

        return isOccluded;
    }

    private float CheckCameraPoints (Vector3 from, Vector3 to)
    {
        float closestDistance = -1f;

        RaycastHit hit;

        ClippPlaneManager.ClipPlanePoints clipPlanePoints = ClippPlaneManager.ClipPlaneAtNear (to);

        // Draw lines to visualize the linecasting from the player.
        Debug.DrawLine (from, to + transform.forward * Camera.main.nearClipPlane, Color.cyan);
        Debug.DrawLine (from, clipPlanePoints.DownRight, Color.blue);
        Debug.DrawLine (from, clipPlanePoints.DownLeft, Color.blue);
        Debug.DrawLine (from, clipPlanePoints.UpRight, Color.blue);
        Debug.DrawLine (from, clipPlanePoints.UpLeft, Color.blue);

        // Draw lines to visualize the clipping plane.
        Debug.DrawLine (clipPlanePoints.UpLeft, clipPlanePoints.UpRight, Color.blue);
        Debug.DrawLine (clipPlanePoints.DownLeft, clipPlanePoints.DownRight, Color.blue);
        Debug.DrawLine (clipPlanePoints.UpLeft, clipPlanePoints.DownLeft, Color.blue);
        Debug.DrawLine (clipPlanePoints.UpRight, clipPlanePoints.DownRight, Color.blue);

        // Check which occluded point of the five that is the nearest.
        if (Physics.Linecast (from, clipPlanePoints.DownRight, out hit) && hit.collider.tag != "Player")
        {
            closestDistance = hit.distance;
        }
        if (Physics.Linecast (from, clipPlanePoints.DownLeft, out hit) && hit.collider.tag != "Player")
        {
            if (hit.distance < closestDistance || closestDistance == -1)
            {
                closestDistance = hit.distance;
            }
        }
        if (Physics.Linecast (from, clipPlanePoints.UpRight, out hit) && hit.collider.tag != "Player")
        {
            if (hit.distance < closestDistance || closestDistance == -1)
            {
                closestDistance = hit.distance;
            }
        }
        if (Physics.Linecast (from, clipPlanePoints.UpLeft, out hit) && hit.collider.tag != "Player")
        {
            if (hit.distance < closestDistance || closestDistance == -1)
            {
                closestDistance = hit.distance;
            }
        }
        if (Physics.Linecast (from, to + transform.forward * Camera.main.nearClipPlane, out hit) && hit.collider.tag != "Player")
        {
            if (hit.distance < closestDistance || closestDistance == -1)
            {
                closestDistance = hit.distance;
            }
        }

        print (closestDistance);

        return closestDistance;
    }
}
*/

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

            // Otherwise, interpolate between the local position and the max distance allowed.
            desiredDistance = Mathf.Lerp (cameraTransform.localPosition.z, maxDistance, Time.deltaTime * zoomSpeed);
        }
    }
}
*/
