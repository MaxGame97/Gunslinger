using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that handles situations where camera collides with geometry.
// Will probably change to boxcasting.

public class HandleCollision : MonoBehaviour
{
    private Transform cameraTransform;
    private Vector3 castingStartPosition;
    private Vector3 currentPosition = Vector3.zero;
    private Vector3 desiredPosition = Vector3.zero;

    private float currentDistance;
    private float desiredDistance;
    private float defaultDistance;
    [SerializeField]
    private float minDistance;
    private float maxDistance;

    private float currentSmoothing = 0f;
    [SerializeField]
    private float cameraCollisionSmoothing;
    [SerializeField]
    private float cameraResetSmoothing;

    private float zoomVelocity = 0f;
    private float zVelocity = 0f;

    [SerializeField]
    private int maxCollisionChecks;
    [SerializeField]
    private float collisionDistanceStep;
    

    private void Awake ()
    {
        cameraTransform = gameObject.transform.Find ("Player Camera");
    }

    private void Start ()
    {
        castingStartPosition = new Vector3 (cameraTransform.localPosition.x, cameraTransform.localPosition.y, 0.0f);
        maxDistance = Mathf.Abs (cameraTransform.localPosition.z);
        minDistance = Mathf.Clamp (minDistance, minDistance, maxDistance);
        currentDistance = Mathf.Abs (cameraTransform.localPosition.z);
        currentDistance = Mathf.Clamp (currentDistance, minDistance, maxDistance);
        defaultDistance = maxDistance;
        desiredDistance = currentDistance;
    }

    private void LateUpdate ()
    {
        int count = 0;

        // Calculates new camera position, and if colliding, loops until fixed (but at most "maxCollisionChecks" times).
        do
        {
            CalculateDesiredPosition ();
            count++;
            print (currentDistance);

        } while (CheckIfColliding (count));

        UpdatePosition ();
    }

    private void CalculateDesiredPosition ()
    {
        // Reset the camera if needed. (Not colliding anymore, etc)
        ResetCamera ();

        currentDistance = Mathf.SmoothDamp (currentDistance, desiredDistance, ref zoomVelocity, currentSmoothing);

        desiredPosition = CalculatePosition (currentDistance);
    }

    private Vector3 CalculatePosition (float distance)
    {
        // Returns a position vector with the distance sent in on the z-axis.
        return new Vector3 (cameraTransform.localPosition.x, cameraTransform.localPosition.y, -distance);
    }

    private void UpdatePosition ()
    {
        float posZ = Mathf.SmoothDamp (currentPosition.z, desiredPosition.z, ref zVelocity, cameraCollisionSmoothing);
        currentPosition = new Vector3 (cameraTransform.localPosition.x, cameraTransform.localPosition.y, posZ);

        cameraTransform.localPosition = currentPosition;
    }

    // Checks if colliding, and if that's the case, changes the desired camera position.
    private bool CheckIfColliding (int count)
    {
        bool isColliding = false;

        // Casts rays to identify potential collisions, and get distance to closest.
        float closestCollision = CheckCameraPoints (transform.TransformPoint (castingStartPosition), transform.TransformPoint (currentPosition));

        // If a distance (positive value) is found, the camera should move. 
        if (closestCollision != -1)
        {
            currentSmoothing = cameraCollisionSmoothing;

            // If limit of allowed checks not reached, push camera distance forward a little.
            if (count < maxCollisionChecks)
            {
                isColliding = true;
                currentDistance -= collisionDistanceStep;

                if (currentDistance < minDistance)
                {
                    currentDistance = minDistance;
                }
            }
            // If limit of allowed checks reached, "teleport" to the position.
            else
            {
                currentDistance = closestCollision - Camera.main.nearClipPlane;
            }

            desiredDistance = currentDistance;
        }
        
        return isColliding;
    }
    
    // Returns the distance to the nearest collision. If returning -1 no collision has been detected.
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

        if (Physics.Linecast (from, clipPlanePoints.DownRight, out hit) && hit.collider.tag != "Player")
        {
            closestDistance = hit.distance;
        }
        if (Physics.Linecast (from, clipPlanePoints.DownLeft, out hit) && hit.collider.tag != "Player")
        {
            if (hit.distance > closestDistance || closestDistance == -1)
            {
                closestDistance = hit.distance;
                desiredPosition = transform.InverseTransformPoint (cameraTransform.position.x, cameraTransform.position.y, hit.point.z);
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

        return closestDistance;
    }

    // Checks if the desired position of the camera should be reset to the default value.
    private void ResetCamera ()
    {
        if (desiredDistance < defaultDistance)
        {
            Vector3 defaultPosition = CalculatePosition (defaultDistance);

            var closestDistance = CheckCameraPoints (transform.TransformPoint (castingStartPosition), transform.TransformPoint (defaultPosition));

            // If closestDistance is -1 there is no collision on the check, and the desired distance is reset.
            if (closestDistance == -1 || closestDistance > defaultDistance)
            {
                currentSmoothing = cameraResetSmoothing;

                desiredDistance = defaultDistance;
            }
        }
    }
}

