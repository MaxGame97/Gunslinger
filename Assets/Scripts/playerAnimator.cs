using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimator : MonoBehaviour {

    private PlayerController player;        // Defines the player's Player Controller
    private CharacterController controller; // Defines the player's Character Controller
    private Animator animator;              // Defines the player's Animator
    private Vector3 currentVelocity;        // Defines the player's current velocity

    // Use this for initialization
    void Start ()
    {
        // Get the attached Player Controller, Character Controller and Animator
        player = GetComponent<PlayerController>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // If no Player Controller, Character Controller of Animator was found
        if (player == null || controller == null || animator == null)
        {
            // Throw an error message and deactivate this object
            Debug.LogError("No Player Controller, Character Controller or Animator found");
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update () {
        // Get the Character Controller's velocity (in local space)
        currentVelocity = transform.InverseTransformDirection(controller.velocity);

        // Normalize the x and z components based on the defined movement speed
        currentVelocity.x /= player.MovementSpeed;
        currentVelocity.z /= player.MovementSpeed;

        // Update the Animator values
        animator.SetFloat("X Movement", currentVelocity.x);
        animator.SetFloat("Y Movement", currentVelocity.y);
        animator.SetFloat("Z Movement", currentVelocity.z);
        animator.SetBool("Is Grounded", controller.isGrounded);
    }
}
