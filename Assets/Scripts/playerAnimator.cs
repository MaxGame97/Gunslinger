using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimator : MonoBehaviour {

    CharacterController controller;
    Animator animator;

    AnimationEvent FootR;
    AnimationEvent FootL;

    // Use this for initialization
    void Start () {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
        // Get the Character Controller's velocity and convert it into the local space
        Vector3 motion = transform.InverseTransformDirection(controller.velocity);

        // Normalize the x and z components
        float movementLength = Mathf.Abs(motion.x) + Mathf.Abs(motion.z);
        motion.x /= movementLength;
        motion.z /= movementLength;

        // Update the animator values
        animator.SetFloat("X Movement", motion.x);
        animator.SetFloat("Y Movement", motion.y);
        animator.SetFloat("Z Movement", motion.z);
        animator.SetBool("Is Grounded", controller.isGrounded);
    }
}
