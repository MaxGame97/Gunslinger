using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOffset : MonoBehaviour
{
    private Vector3 cameraOffset;


    private void Awake ()
    {
        cameraOffset = GetComponentInParent<CameraController> ().offset;
        transform.localPosition = new Vector3 (cameraOffset.x, cameraOffset.y, cameraOffset.z);
    }
}
