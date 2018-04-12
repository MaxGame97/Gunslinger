using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController_Magnus : MonoBehaviour {

    public GameObject target;
    public float rotateSpeed = 5;
    public float clamp = 90;
    public Vector3 offset;

    // Use this for initialization
    void Start()
    {
        transform.position += offset;
    }

    // LateUpdate is called at the end of each frame
    void LateUpdate()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
        float vertical = -Input.GetAxis("Mouse Y") * rotateSpeed;
        target.transform.parent.transform.Rotate(0, horizontal, 0);
        target.transform.Rotate(vertical, 0, 0);
    }
}
