using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that handles most of the player camera behaviour.

public class CameraController : MonoBehaviour
{
    private Vector2 mouseInput;
    private Vector2 smoothV;
    private Vector2 mouseLook;

    [Header ("Settings")]
    public Vector3 offset;
    [SerializeField]
    private float sensitivity;
    [SerializeField]
    private float damping;
    [SerializeField]
    private float minAngle;
    [SerializeField]
    private float maxAngle;


    private void LateUpdate ()
    {
        mouseInput = new Vector2 (Input.GetAxisRaw ("Mouse X"), Input.GetAxisRaw ("Mouse Y"));
        mouseInput = Vector2.Scale (mouseInput, new Vector2 (sensitivity * damping, sensitivity * damping));

        // Rotation.
        smoothV.x = Mathf.Lerp (smoothV.x, mouseInput.x, 1.0f / damping);
        smoothV.y = Mathf.Lerp (smoothV.y, mouseInput.y, 1.0f / damping);

        mouseLook += smoothV;
        mouseLook.y = Mathf.Clamp (mouseLook.y, minAngle, maxAngle);

        transform.localRotation = Quaternion.AngleAxis (-mouseLook.y, Vector3.right);
        transform.parent.localRotation = Quaternion.AngleAxis (mouseLook.x, transform.parent.up);
    }
}
