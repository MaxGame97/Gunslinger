﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that handles situations where camera collides with geometry.

public class CollisionHandler : MonoBehaviour
{
    public Transform cameraTransform;

    [SerializeField] private Vector3 offset;

    private struct CameraPosition
    {
        private Vector3 castingStartPosition;
        public Vector3 CastingStartPosition
        {
            get
            {
                return castingStartPosition;
            }
            set
            {
                castingStartPosition = value;
            }
        }

        private Vector3 currentPosition;
        public Vector3 CurrentPosition
        {
            get
            {
                return currentPosition;
            }
            set
            {
                currentPosition = value;
            }
        }

        private Vector3 desiredPosition;
        public Vector3 DesiredPosition
        {
            get
            {
                return desiredPosition;
            }
            set
            {
                desiredPosition = value;
            }
        }

        private float smoothingSpeed;
        public float SmoothingSpeed
        {
            get
            {
                return smoothingSpeed;
            }
            set
            {
                smoothingSpeed = value;
            }
        }

        public Vector3 SmoothingVelocity;
    }
    CameraPosition cameraPosition;

    private struct Axis
    {
        private float currentValue;
        public float CurrentValue
        {
            get
            {
                return currentValue;
            }
            set
            {
                currentValue = value;
            }
        }

        private float desiredValue;
        public float DesiredValue
        {
            get
            {
                return desiredValue;
            }
            set
            {
                desiredValue = value;
            }
        }

        private float smoothingSpeed;
        public float SmoothingSpeed
        {
            get
            {
                return smoothingSpeed;
            }
            set
            {
                smoothingSpeed = value;
            }
        }

        public float SmoothingVelocity;
    }
    Axis xAxis, yAxis, zAxis;

    [SerializeField] private float minDistance;

    [SerializeField] private float collisionSmoothing;
    [SerializeField] private float resetSmoothing;


    private void Awake ()
    {
        cameraTransform = gameObject.transform.Find ("Player Camera");
    }

    private void Start ()
    {
        offset = new Vector3 (offset.x, offset.y, Mathf.Abs (offset.z));

        cameraPosition.CastingStartPosition = new Vector3 (0.0f, 0.0f, 0.0f);
        cameraPosition.CurrentPosition = Vector3.zero;
        cameraPosition.DesiredPosition = Vector3.zero;
        cameraPosition.SmoothingSpeed = 0.0f;
        cameraPosition.SmoothingVelocity = Vector3.zero;

        minDistance = Mathf.Clamp (minDistance, 0.0f, offset.z);

        xAxis.CurrentValue = offset.x;
        xAxis.DesiredValue = 0.0f;
        xAxis.SmoothingSpeed = 0.0f;
        xAxis.SmoothingVelocity = 0.0f;

        yAxis.CurrentValue = offset.y;
        yAxis.DesiredValue = 0.0f;
        yAxis.SmoothingSpeed = 0.0f;
        yAxis.SmoothingVelocity = 0.0f;

        zAxis.CurrentValue = offset.z;
        zAxis.DesiredValue = 0.0f;
        zAxis.SmoothingSpeed = 0.0f;
        zAxis.SmoothingVelocity = 0.0f;
    }

    private void LateUpdate ()
    {
        CheckIfColliding ();

        CalculateDesiredPosition ();

        UpdatePosition ();
    }

    private void CalculateDesiredPosition ()
    {
        //ResetCamera ();

        xAxis.CurrentValue = Mathf.SmoothDamp (xAxis.CurrentValue, xAxis.DesiredValue, ref xAxis.SmoothingVelocity, xAxis.SmoothingSpeed);
        yAxis.CurrentValue = Mathf.SmoothDamp (yAxis.CurrentValue, yAxis.DesiredValue, ref yAxis.SmoothingVelocity, yAxis.SmoothingSpeed);
        zAxis.CurrentValue = Mathf.SmoothDamp (zAxis.CurrentValue, zAxis.DesiredValue, ref zAxis.SmoothingVelocity, zAxis.SmoothingSpeed);

        cameraPosition.DesiredPosition = new Vector3 (xAxis.CurrentValue, yAxis.CurrentValue, -zAxis.CurrentValue);
    }
    
    /*
    private Vector3 CalculatePosition (float localXCoordinate, float localYCoordinate, float localZDistance)
    {
        return new Vector3 (localXCoordinate, localYCoordinate, -localZDistance);
    }
    */

    private void UpdatePosition ()
    {
        cameraPosition.CurrentPosition = Vector3.SmoothDamp (cameraPosition.CurrentPosition, cameraPosition.DesiredPosition, ref cameraPosition.SmoothingVelocity, cameraPosition.SmoothingSpeed);
        cameraTransform.localPosition = cameraPosition.CurrentPosition;
    }
    /*
    private void ResetCamera ()
    { 
        
    }
    */
    private void CheckIfColliding ()
    {
        // Check y-axis.
        cameraPosition.CastingStartPosition = new Vector3 (0.0f, 0.0f, 0.0f);

        float allowedYValue = CheckCollisions (transform.TransformPoint (cameraPosition.CastingStartPosition), transform.up, offset.y);

        if (allowedYValue < offset.y)
        {
            yAxis.SmoothingSpeed = collisionSmoothing;
        }
        else
        {
            yAxis.SmoothingSpeed = resetSmoothing;
        }

        yAxis.DesiredValue = allowedYValue;
        
        // Check x-axis.
        cameraPosition.CastingStartPosition = new Vector3 (0.0f, allowedYValue, 0.0f);

        float allowedXValue = CheckCollisions (transform.TransformPoint (cameraPosition.CastingStartPosition), transform.right, offset.x);

        if (allowedXValue < offset.x)
        {
            xAxis.SmoothingSpeed = collisionSmoothing;
        }
        else
        {
            xAxis.SmoothingSpeed = resetSmoothing;
        }
        
        xAxis.DesiredValue = allowedXValue;

        // Check z-axis.
        cameraPosition.CastingStartPosition = new Vector3 (allowedXValue, allowedYValue, 0.0f);

        float allowedZValue = CheckCollisions (transform.TransformPoint (cameraPosition.CastingStartPosition), -transform.forward, offset.z);

        if (allowedZValue < offset.z)
        {
            zAxis.SmoothingSpeed = collisionSmoothing;
        }
        else
        {
            zAxis.SmoothingSpeed = resetSmoothing;
        }
        
        zAxis.DesiredValue = allowedZValue;

        if (zAxis.DesiredValue < minDistance)
        {
            zAxis.DesiredValue = minDistance;
        }
    }

    private float CheckCollisions (Vector3 from, Vector3 direction, float distance)
    {
        const float PADDING = 0.1f;

        RaycastHit hitInfo;

        float coordinateDistance = distance;

        ClipPlaneManager.ClipPlanePoints clipPlanePoints = ClipPlaneManager.ClipPlaneAtNear (transform.TransformPoint (cameraPosition.CurrentPosition));

        // Draw lines to visualize the clipping plane.
        Debug.DrawLine (clipPlanePoints.UpLeft, clipPlanePoints.UpRight, Color.blue);
        Debug.DrawLine (clipPlanePoints.DownLeft, clipPlanePoints.DownRight, Color.blue);
        Debug.DrawLine (clipPlanePoints.UpLeft, clipPlanePoints.DownLeft, Color.blue);
        Debug.DrawLine (clipPlanePoints.UpRight, clipPlanePoints.DownRight, Color.blue);

        // Cast box in size of near frustum to check for camera collisions.
        if (Physics.BoxCast (from, clipPlanePoints.Size, direction, out hitInfo, cameraTransform.rotation, distance + PADDING) && hitInfo.collider.tag != "Player" && hitInfo.collider.tag != "Hitbox")
        {
            coordinateDistance = hitInfo.distance;
        }

        return coordinateDistance;
    }

    private float PointToPlaneDistance (Vector3 pointPosition, Vector3 planePosition, Vector3 planeNormal)
    {
        float sb, sn, sd;

        sn = -Vector3.Dot (planeNormal, (pointPosition - planePosition));
        sd = Vector3.Dot (planeNormal, planeNormal);
        sb = sn / sd;

        Vector3 result = pointPosition + sb * planeNormal;
        return Vector3.Distance (pointPosition, result);
    }
}


/*
public class CollisionHandler : MonoBehaviour
{
    public Transform cameraTransform;
    
    private struct CameraPosition
    {
        private Vector3 castingStartPosition;
        public Vector3 CastingStartPosition
        {
            get
            {
                return castingStartPosition;
            }
            set
            {
                castingStartPosition = value;
            }
        }

        private Vector3 currentPosition;
        public Vector3 CurrentPosition
        {
            get
            {
                return currentPosition;
            }
            set
            {
                currentPosition = value;
            }
        }

        private Vector3 desiredPosition;
        public Vector3 DesiredPosition
        {
            get
            {
                return desiredPosition;
            }
            set
            {
                desiredPosition = value;
            }
        }

        private Vector3 defaultPosition;
        public Vector3 DefaultPosition
        {
            get
            {
                return defaultPosition;
            }
            set
            {
                defaultPosition = value;
            }
        }

        public Vector3 SmoothingVelocity;
    }
    CameraPosition cameraPosition;

    private struct Axis
    {
        private float currentValue;
        public float CurrentValue
        {
            get
            {
                return currentValue;
            }
            set
            {
                currentValue = value;
            }
        }

        private float desiredValue;
        public float DesiredValue
        {
            get
            {
                return desiredValue;
            }
            set
            {
                desiredValue = value;
            }
        }

        private float defaultValue;
        public float DefaultValue
        {
            get
            {
                return defaultValue;
            }
            set
            {
                defaultValue = value;
            }
        }

        public float SmoothingVelocity;
    }
    Axis xAxis, yAxis, zAxis;

    [SerializeField] private float minDistance;
    private float maxDistance;

    private float currentSmoothingSpeed = 0f;
    [SerializeField] private float collisionSmoothing;
    [SerializeField] private float resetSmoothing;


    private void Awake ()
    {
        cameraTransform = gameObject.transform.Find ("Player Camera");
    }

    private void Start ()
    {
        cameraPosition.CastingStartPosition = new Vector3 (cameraTransform.localPosition.x, cameraTransform.localPosition.y, 0.0f);
        cameraPosition.DefaultPosition = cameraTransform.localPosition;
        cameraPosition.CurrentPosition = Vector3.zero;
        cameraPosition.DesiredPosition = Vector3.zero;
        cameraPosition.SmoothingVelocity = Vector3.zero;

        maxDistance = Mathf.Abs (cameraTransform.localPosition.z);
        minDistance = Mathf.Clamp (minDistance, 0.0f, maxDistance);

        xAxis.DefaultValue = cameraTransform.localPosition.x;
        xAxis.CurrentValue = xAxis.DefaultValue;
        xAxis.DesiredValue = xAxis.DefaultValue;
        xAxis.SmoothingVelocity = 0f;

        yAxis.DefaultValue = cameraTransform.localPosition.y;
        yAxis.CurrentValue = yAxis.DefaultValue;

        zAxis.DefaultValue = maxDistance;
        zAxis.CurrentValue = zAxis.DefaultValue;
        zAxis.DesiredValue = zAxis.DefaultValue;
        zAxis.SmoothingVelocity = 0f;
    }

    private void LateUpdate ()
    {
        CheckIfColliding ();

        CalculateDesiredPosition ();

        UpdatePosition ();
    }

    private void CalculateDesiredPosition ()
    {
        ResetCamera ();

        xAxis.CurrentValue = Mathf.SmoothDamp (xAxis.CurrentValue, xAxis.DesiredValue, ref xAxis.SmoothingVelocity, currentSmoothingSpeed);
        yAxis.CurrentValue = yAxis.DefaultValue;
        zAxis.CurrentValue = Mathf.SmoothDamp (zAxis.CurrentValue, zAxis.DesiredValue, ref zAxis.SmoothingVelocity, currentSmoothingSpeed);

        cameraPosition.DesiredPosition = CalculatePosition (xAxis.CurrentValue, yAxis.CurrentValue, zAxis.CurrentValue);
    }

    private Vector3 CalculatePosition (float localXCoordinate, float localYCoordinate, float localZDistance)
    {
        return new Vector3 (localXCoordinate, localYCoordinate, -localZDistance);
    }

    private void UpdatePosition ()
    {
        cameraPosition.CurrentPosition = Vector3.SmoothDamp (cameraPosition.CurrentPosition, cameraPosition.DesiredPosition, ref cameraPosition.SmoothingVelocity, currentSmoothingSpeed);
        cameraTransform.localPosition = cameraPosition.CurrentPosition;
    }

    private void ResetCamera ()
    {
        if (xAxis.CurrentValue != xAxis.DefaultValue)
        {
            float allowedXValue = CheckXCollisions (transform.TransformPoint (cameraPosition.CurrentPosition));

            if (allowedXValue == xAxis.DefaultValue)
            {
                currentSmoothingSpeed = resetSmoothing;

                xAxis.DesiredValue = allowedXValue;
            }
        }

        if (zAxis.CurrentValue < maxDistance)
        {
            float allowedZValue = CheckZCollisions (transform.TransformPoint (cameraPosition.CastingStartPosition), transform.TransformPoint (cameraPosition.DefaultPosition));

            if (allowedZValue == -1 || allowedZValue > maxDistance)
            {
                currentSmoothingSpeed = resetSmoothing;

                zAxis.DesiredValue = maxDistance;
            }
        }
    }

    private void CheckIfColliding ()
    {
        cameraPosition.CastingStartPosition = new Vector3 (xAxis.CurrentValue, yAxis.CurrentValue, 0.0f);
        
        float allowedXValue = CheckXCollisions (transform.TransformPoint (cameraPosition.CurrentPosition));
        
        if (allowedXCoordinate != xAxis.CurrentValue)
        {
            currentSmoothingSpeed = collisionSmoothing;

            xAxis.DesiredValue = allowedXCoordinate;
        }
        
    float allowedZValue = CheckZCollisions (transform.TransformPoint (cameraPosition.CastingStartPosition), transform.TransformPoint (cameraPosition.CurrentPosition));

            if (allowedZValue != -1)
            {
                currentSmoothingSpeed = collisionSmoothing;

                zAxis.DesiredValue = allowedZValue;

                if (zAxis.DesiredValue<minDistance)
                {
                    zAxis.DesiredValue = minDistance;
                }
            }
        }
    
        private float CheckXCollisions (Vector3 to)
    {
        RaycastHit hitInfo;

        float allowedXCoordinate = xAxis.CurrentValue;

        ClipPlaneManager.ClipPlanePoints clipPlanePoints = ClipPlaneManager.ClipPlaneAtNear (to);

        Debug.DrawRay (clipPlanePoints.Left, -transform.right * 0.5f, Color.blue);
        Debug.DrawRay (clipPlanePoints.Right, transform.right * 0.5f, Color.blue);

        if (Physics.Raycast (clipPlanePoints.Left, -transform.right, out hitInfo, 0.5f) && hitInfo.collider.tag != "Player")
        {
            float distanceToWall = PointToPlaneDistance (clipPlanePoints.Left, hitInfo.point, hitInfo.normal);

            if (distanceToWall < 0.2f)
            {
                allowedXCoordinate += (0.2f - distanceToWall);
            }
        }
        if (Physics.Raycast (clipPlanePoints.Right, transform.right, out hitInfo, 0.5f) && hitInfo.collider.tag != "Player")
        {
            float distanceToWall = PointToPlaneDistance (clipPlanePoints.Right, hitInfo.point, hitInfo.normal);

            if (distanceToWall < 0.2f)
            {
                allowedXCoordinate -= (0.2f - distanceToWall);
            }
        }

        return allowedXCoordinate;
    }

    private float CheckZCollisions (Vector3 from, Vector3 to)
    {
        RaycastHit hitInfo;

        float closestZDistance = -1;

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

        // Cast box in size of near frustum to check for camera collisions.
        if (Physics.BoxCast (from, clipPlanePoints.Size, -transform.forward, out hitInfo, cameraTransform.rotation, maxDistance) && hitInfo.collider.tag != "Player")
        {
            closestZDistance = hitInfo.distance;
        }

        return closestZDistance;
    }

    private float PointToPlaneDistance (Vector3 pointPosition, Vector3 planePosition, Vector3 planeNormal)
    {
        float sb, sn, sd;

        sn = -Vector3.Dot (planeNormal, (pointPosition - planePosition));
        sd = Vector3.Dot (planeNormal, planeNormal);
        sb = sn / sd;

        Vector3 result = pointPosition + sb * planeNormal;
        return Vector3.Distance (pointPosition, result);
    }
}
*/
