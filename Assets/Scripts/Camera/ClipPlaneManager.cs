using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClipPlaneManager
{
    // A struct to hold the coordinates to corners of clipping plane.
    public struct ClipPlanePoints
    {
        public Vector3 Position;
        public Vector3 Size;
        public Vector3 Left;
        public Vector3 Right;
        public Vector3 UpLeft;
        public Vector3 UpRight;
        public Vector3 DownLeft;
        public Vector3 DownRight;
    }

    public static ClipPlanePoints ClipPlaneAtNear (Vector3 position)
    {
        var clipPlanePoints = new ClipPlanePoints ();

        if (Camera.main == null)
        {
            return clipPlanePoints;
        }

        // References the main camera.
        Transform transform = Camera.main.transform;

        // Calculates the angle of half the field of view.
        float halfFOV = (Camera.main.fieldOfView / 2) * Mathf.Deg2Rad;

        // Gets the current aspect ratio of the camera.
        float aspectRatio = Camera.main.aspect;

        // Gets the distance to the near clipping plane.
        float distance = Camera.main.nearClipPlane;
        
        // Calculates the height from center of the clipping plane.
        float height = distance * Mathf.Tan (halfFOV);

        // Calculates the width from center of the clipping plane.
        float width = height * aspectRatio;

        // Position of center of near frustum plane.
        clipPlanePoints.Position += transform.forward * distance; 

        // Size of near frustum plane.
        clipPlanePoints.Size = new Vector3 (width, height, 0.1f);

        // Calculate the position of the right and left edges.
        clipPlanePoints.Left = position - transform.right * width;
        clipPlanePoints.Left += transform.forward * distance;

        clipPlanePoints.Right = position + transform.right * width;
        clipPlanePoints.Right += transform.forward * distance;

        // Positions of frustum corners, for visualization.
        clipPlanePoints.UpLeft = position - transform.right * width;
        clipPlanePoints.UpLeft += transform.up * height;
        clipPlanePoints.UpLeft += transform.forward * distance;

        clipPlanePoints.UpRight = position + transform.right * width;
        clipPlanePoints.UpRight += transform.up * height;
        clipPlanePoints.UpRight += transform.forward * distance;

        clipPlanePoints.DownLeft = position - transform.right * width;
        clipPlanePoints.DownLeft -= transform.up * height;
        clipPlanePoints.DownLeft += transform.forward * distance;

        clipPlanePoints.DownRight = position + transform.right * width;
        clipPlanePoints.DownRight -= transform.up * height;
        clipPlanePoints.DownRight += transform.forward * distance;


        return clipPlanePoints;
    }
}


/*
public static ClipPlanePoints ClipPlaneAtNear (Vector3 position)
    {
        var clipPlanePoints = new ClipPlanePoints ();

        if (Camera.main == null)
        {
            return clipPlanePoints;
        }

        // References the main camera.
        Transform transform = Camera.main.transform;

        // Calculates the angle of half the field of view.
        float halfFOV = (Camera.main.fieldOfView / 2) * Mathf.Deg2Rad;

        // Gets the current aspect ratio of the camera.
        float aspectRatio = Camera.main.aspect;

        // Gets the distance to the near clipping plane.
        float distance = Camera.main.nearClipPlane;
        
        // Calculates the height from center of the clipping plane.
        float height = distance * Mathf.Tan (halfFOV);

        // Calculates the width from center of the clipping plane.
        float width = height * aspectRatio;

        // Position of center of near frustum plane.
        clipPlanePoints.Position += transform.forward * distance; 

        // Size of near frustum plane.
        clipPlanePoints.Size = new Vector3 (width, height, 0.1f);

        // Calculate the position of the right and left edges.
        clipPlanePoints.Left = position - transform.right * width;
        clipPlanePoints.Left += transform.forward * distance;

        clipPlanePoints.Right = position + transform.right * width;
        clipPlanePoints.Right += transform.forward * distance;

        // Positions of frustum corners, for visualization.
        clipPlanePoints.UpLeft = position - transform.right * width;
        clipPlanePoints.UpLeft += transform.up * height;
        clipPlanePoints.UpLeft += transform.forward * distance;

        clipPlanePoints.UpRight = position + transform.right * width;
        clipPlanePoints.UpRight += transform.up * height;
        clipPlanePoints.UpRight += transform.forward * distance;

        clipPlanePoints.DownLeft = position - transform.right * width;
        clipPlanePoints.DownLeft -= transform.up * height;
        clipPlanePoints.DownLeft += transform.forward * distance;

        clipPlanePoints.DownRight = position + transform.right * width;
        clipPlanePoints.DownRight -= transform.up * height;
        clipPlanePoints.DownRight += transform.forward * distance;


        return clipPlanePoints;
    } 
*/
