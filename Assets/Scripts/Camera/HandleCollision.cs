using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that handles situations where camera collides with geometry.
// Will probably change to boxcasting when functionality is set.

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

    private float currentEdge;
    private float desiredEdge;
    private float defaultEdge;

    private float currentSmoothing = 0f;
    [SerializeField]
    private float cameraCollisionSmoothing;
    [SerializeField]
    private float cameraResetSmoothing;


    private float edgeVelocity = 0f;
    private float distVelocity = 0f;
    private Vector3 posVelocity = Vector3.zero;


    private void Awake ()
    {
        cameraTransform = gameObject.transform.Find ("Player Camera");
    }

    private void Start ()
    {
        castingStartPosition = new Vector3 (cameraTransform.localPosition.x, cameraTransform.localPosition.y, 0.0f);
        maxDistance = Mathf.Abs (cameraTransform.localPosition.z);
        minDistance = Mathf.Clamp (minDistance, 0.0f, maxDistance);

        currentDistance = Mathf.Abs (cameraTransform.localPosition.z);
        currentDistance = Mathf.Clamp (currentDistance, minDistance, maxDistance);
        defaultDistance = maxDistance;
        desiredDistance = currentDistance;

        currentEdge = cameraTransform.localPosition.x;
        defaultEdge = cameraTransform.localPosition.x;
        desiredEdge = currentEdge;
    }

    private void LateUpdate ()
    {
        print ("Current: " + currentEdge + " Default: " + defaultEdge + " Desired: " + desiredEdge);

        CalculateDesiredPosition ();

        CheckIfColliding ();

        UpdatePosition ();
    }

    private void CalculateDesiredPosition ()
    {
        // Reset the camera if not colliding anymore.
        ResetCamera ();

        currentEdge = Mathf.SmoothDamp (currentEdge, desiredEdge, ref edgeVelocity, currentSmoothing);
        currentDistance = Mathf.SmoothDamp (currentDistance, desiredDistance, ref distVelocity, currentSmoothing);

        desiredPosition = CalculatePosition (currentEdge, currentDistance);
    }

    // Returns a position vector with the distance sent in on the z-axis.
    private Vector3 CalculatePosition (float edge, float distance)
    {
        return new Vector3 (edge, cameraTransform.localPosition.y, -distance);
    }

    private void UpdatePosition ()
    {
        currentPosition = Vector3.SmoothDamp (currentPosition, desiredPosition, ref posVelocity, currentSmoothing);
        cameraTransform.localPosition = currentPosition;
    }

    // Checks if the desired position of the camera should be reset to the default value.
    private void ResetCamera ()
    {
        if (desiredDistance < defaultDistance)
        {
            Vector3 defaultPosition = CalculatePosition (currentEdge, defaultDistance);

            float closestDistance = CheckCameraPoints (transform.TransformPoint (castingStartPosition), transform.TransformPoint (defaultPosition));

            // If closestDistance is -1 there is no collision on the check, and the desired distance is reset.
            if (closestDistance == -1 || closestDistance > defaultDistance)
            {
                currentSmoothing = cameraResetSmoothing;

                desiredDistance = defaultDistance;
            }
        }
    }

    // Checks if colliding, and if that's the case, modifies the desired camera distance.
    private void CheckIfColliding ()
    {
        float edgeCollision = CheckCameraEdges (transform.TransformPoint (currentPosition));
        /*
        if (edgeCollision != currentEdge)
        {
            currentSmoothing = cameraCollisionSmoothing;

            desiredEdge = edgeCollision;
        }
        else
        {
            desiredEdge = defaultEdge;
        }
        */
        // Casts rays to detect collisions, and get distance to closest.
        float closestCollision = CheckCameraPoints (transform.TransformPoint (castingStartPosition), transform.TransformPoint (currentPosition));

        // If a distance (positive value) is found, the camera should move. 
        if (closestCollision != -1)
        {
            currentSmoothing = cameraCollisionSmoothing;

            // If allowed to du another check, push desired distance forward a little.
            //isColliding = true;
            desiredDistance = closestCollision;

            if (desiredDistance < minDistance)
            {
                desiredDistance = minDistance;
            }
        }
    }

    private float CheckCameraEdges (Vector3 to)
    {
        RaycastHit hitInfo;

        float edgeCollision = currentEdge;

        ClipPlaneManager.ClipPlanePoints clipPlanePoints = ClipPlaneManager.ClipPlaneAtNear (to);

        Debug.DrawRay (clipPlanePoints.Left, -transform.right * 0.2f, Color.blue);
        Debug.DrawRay (clipPlanePoints.Right, transform.right * 0.2f, Color.blue);
        
        // Check camera edges.
        if (Physics.Raycast (clipPlanePoints.Left, -transform.right, out hitInfo, 0.2f) && hitInfo.collider.tag != "Player")
        {
            print ("Hit1");
            edgeCollision = hitInfo.point.x + 0.2f; 
        }
        if (Physics.Raycast (clipPlanePoints.Right, transform.right, out hitInfo, 0.2f) && hitInfo.collider.tag != "Player")
        {
            print ("Hit2");
            edgeCollision = hitInfo.point.x - 0.2f;
        }
        
        return edgeCollision;
    }

    // Returns the distance to the nearest collision. If returning -1 no collision has been detected.
    private float CheckCameraPoints (Vector3 from, Vector3 to)
    {
        RaycastHit hitInfo;

        float closestDistance = -1;

        ClipPlaneManager.ClipPlanePoints clipPlanePoints = ClipPlaneManager.ClipPlaneAtNear (to);

        // Draw lines to visualize the linecasting from the player.
        Debug.DrawLine (from, to + transform.forward * Camera.main.nearClipPlane, Color.cyan);
        Debug.DrawLine (from, clipPlanePoints.UpLeft, Color.blue);
        Debug.DrawLine (from, clipPlanePoints.UpRight, Color.blue);
        Debug.DrawLine (from, clipPlanePoints.DownLeft, Color.blue);
        Debug.DrawLine (from, clipPlanePoints.DownRight, Color.blue);

        // Draw lines to visualize the clipping plane.
        Debug.DrawLine (clipPlanePoints.UpLeft, clipPlanePoints.UpRight, Color.blue);
        Debug.DrawLine (clipPlanePoints.DownLeft, clipPlanePoints.DownRight, Color.blue);
        Debug.DrawLine (clipPlanePoints.UpLeft, clipPlanePoints.DownLeft, Color.blue);
        Debug.DrawLine (clipPlanePoints.UpRight, clipPlanePoints.DownRight, Color.blue);
        

        // Check camera points.
        if (Physics.Linecast (from, clipPlanePoints.UpLeft, out hitInfo) && hitInfo.collider.tag != "Player")
        {
            closestDistance = hitInfo.distance;
        }
        if (Physics.Linecast (from, clipPlanePoints.UpRight, out hitInfo) && hitInfo.collider.tag != "Player")
        {
            if (hitInfo.distance < closestDistance || closestDistance == -1)
            {
                closestDistance = hitInfo.distance;
            }        
        }
        if (Physics.Linecast (from, clipPlanePoints.DownLeft, out hitInfo) && hitInfo.collider.tag != "Player")
        {
            if (hitInfo.distance < closestDistance || closestDistance == -1)
            {
                closestDistance = hitInfo.distance;
            }
        }
        if (Physics.Linecast (from, clipPlanePoints.DownRight, out hitInfo) && hitInfo.collider.tag != "Player")
        {
            if (hitInfo.distance < closestDistance || closestDistance == -1)
            {
                closestDistance = hitInfo.distance;
            }
        }
        if (Physics.Linecast (from, to + transform.forward * Camera.main.nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Player")
        {
            if (hitInfo.distance < closestDistance || closestDistance == -1)
            {
                closestDistance = hitInfo.distance;
            }
        }

        return closestDistance;
    }
}


/*
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

    private float distVelocity = 0f;
    private float posVelocity = 0f;

    [SerializeField]
    private int maxCollisionChecks;
    [SerializeField]
    private float collisionDistanceStep;
    

    private void Awake ()
    {
        cameraTransform = transform.GetChild(0);
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

        } while (CheckIfColliding (count));

        UpdatePosition ();
    }

    private void CalculateDesiredPosition ()
    {
        // Reset the camera if not colliding anymore.
        ResetCamera ();

        currentDistance = Mathf.SmoothDamp (currentDistance, desiredDistance, ref distVelocity, currentSmoothing);

        // skicka in x-coordinat?
        desiredPosition = CalculatePosition (currentDistance);
    }

    // Returns a position vector with the distance sent in on the z-axis.
    private Vector3 CalculatePosition (float distance)
    {
        return new Vector3 (cameraTransform.localPosition.x, cameraTransform.localPosition.y, -distance);
    }

    private void UpdatePosition ()
    {
        float posX = cameraTransform.localPosition.x;
        float posY = cameraTransform.localPosition.y;
        float posZ = Mathf.SmoothDamp (currentPosition.z, desiredPosition.z, ref posVelocity, currentSmoothing);
        currentPosition = new Vector3 (posX, posY, posZ);

        cameraTransform.localPosition = currentPosition;
    }

    // Checks if the desired position of the camera should be reset to the default value.
    private void ResetCamera ()
    {
        if (desiredDistance < defaultDistance)
        {
            Vector3 defaultPosition = CalculatePosition (defaultDistance);

            float closestDistance = CheckCameraPoints (transform.TransformPoint (castingStartPosition), transform.TransformPoint (defaultPosition));

            // If closestDistance is -1 there is no collision on the check, and the desired distance is reset.
            if (closestDistance == -1 || closestDistance > defaultDistance)
            {
                currentSmoothing = cameraResetSmoothing;

                desiredDistance = defaultDistance;
            }
        }
    }

    // Checks if colliding, and if that's the case, modifies the desired camera distance.
    private bool CheckIfColliding (int count)
    {
        bool isColliding = false;

        // Casts rays to detect collisions, and get distance to closest.
        float closestCollision = CheckCameraPoints (transform.TransformPoint (castingStartPosition), transform.TransformPoint (currentPosition));

        // If a distance (positive value) is found, the camera should move. 
        if (closestCollision != -1)
        {
            currentSmoothing = cameraCollisionSmoothing;

            // If allowed to du another check, push desired distance forward a little.
            if (count < maxCollisionChecks)
            {
                isColliding = true;
                currentDistance -= collisionDistanceStep;

                if (currentDistance < minDistance)
                {
                    currentDistance = minDistance;
                }
            }
            // If not allowed to do another check, "teleport" to the distance, minus some padding.
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
        RaycastHit hitInfo;

        float closestDistance = -1;

        ClipPlaneManager.ClipPlanePoints clipPlanePoints = ClipPlaneManager.ClipPlaneAtNear (to);

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

        if (Physics.Linecast (from, clipPlanePoints.DownRight, out hitInfo) && hitInfo.collider.tag != "Player")
        {
            closestDistance = hitInfo.distance;
        }
        if (Physics.Linecast (from, clipPlanePoints.DownLeft, out hitInfo) && hitInfo.collider.tag != "Player")
        {
            if (hitInfo.distance < closestDistance || closestDistance == -1)
            {
                closestDistance = hitInfo.distance;
            }
        }
        if (Physics.Linecast (from, clipPlanePoints.UpRight, out hitInfo) && hitInfo.collider.tag != "Player")
        {
            if (hitInfo.distance < closestDistance || closestDistance == -1)
            {
                closestDistance = hitInfo.distance;
            }
        }
        if (Physics.Linecast (from, clipPlanePoints.UpLeft, out hitInfo) && hitInfo.collider.tag != "Player")
        {
            if (hitInfo.distance < closestDistance || closestDistance == -1)
            {
                closestDistance = hitInfo.distance;
            }
        }
        if (Physics.Linecast (from, to + transform.forward * Camera.main.nearClipPlane, out hitInfo) && hitInfo.collider.tag != "Player")
        {
            if (hitInfo.distance < closestDistance || closestDistance == -1)
            {
                closestDistance = hitInfo.distance;
            }
        }

        return closestDistance;
    }
}
*/
