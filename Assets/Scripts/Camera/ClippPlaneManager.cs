﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClippPlaneManager
{
    // A struct to hold the coordinates to corners of clipping plane.
    public struct ClipPlanePoints
    {
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
        var height = distance * Mathf.Tan (halfFOV);

        // Calculates the width from center of the clipping plane.
        var width = height * aspectRatio;

        // Calculates the position of the four corners of the clipping plane.
        clipPlanePoints.DownRight = position + transform.right * width;
        clipPlanePoints.DownRight -= transform.up * height;
        clipPlanePoints.DownRight += transform.forward * distance;

        clipPlanePoints.DownLeft = position - transform.right * width;
        clipPlanePoints.DownLeft -= transform.up * height;
        clipPlanePoints.DownLeft += transform.forward * distance;

        clipPlanePoints.UpRight = position + transform.right * width;
        clipPlanePoints.UpRight += transform.up * height;
        clipPlanePoints.UpRight += transform.forward * distance;

        clipPlanePoints.UpLeft = position - transform.right * width;
        clipPlanePoints.UpLeft += transform.up * height;
        clipPlanePoints.UpLeft += transform.forward * distance;

        return clipPlanePoints;
    }
}