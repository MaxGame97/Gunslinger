using UnityEngine;

public class PlayerIKController : MonoBehaviour
{
    // ----------------------------------------------------------------------
    // --- DEBUG - Used to offset IK goals if they are incorrectly placed ---
    // ----------------------------------------------------------------------
    public Vector3 positionOffset = Vector3.zero;

    public bool iKActive = false;               // Defines whether IK is active

    public Transform lookTarget = null;         // Defines the look IK target
    public Transform leftHandTarget = null;     // Defines the left hand IK target
    public Transform rightHandTarget = null;    // Defines the right hand IK target
    public Transform leftFootTarget = null;     // Defines the left foot IK target
    public Transform rightFootTarget = null;    // Defines the right foot IK target

    private Animator animator;                  // Defines the player's Animator

    // Use this for initialization
    void Start()
    {
        // Get the attached Animator
        animator = GetComponent<Animator>();

        // If no Animator was found
        if (animator == null)
        {
            // Throw an error message and deactivate this object
            Debug.LogError("No Animator found");
            this.enabled = false;
        }
    }

    // OnAnimatorIK is called when IK calculations are made by the Animator
    void OnAnimatorIK()
    {
        if (animator)
        {
            // If the IK is active, calculate IK positions for the head, hands and feet based on the assigned targets
            if (iKActive)
            {
                // Set the look target position, if one has been assigned
                if (lookTarget != null)
                {
                    animator.SetLookAtWeight(1f);
                    animator.SetLookAtPosition(lookTarget.position + positionOffset);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position + positionOffset);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
                }

                // Set the left hand target position and rotation, if one has been assigned
                if (leftHandTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position + positionOffset);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
                }

                // Set the right foot target position and rotation, if one has been assigned
                if (rightFootTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTarget.position + positionOffset);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootTarget.rotation);
                }

                // Set the left foot target position and rotation, if one has been assigned
                if (leftFootTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTarget.position + positionOffset);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootTarget.rotation);
                }
            }

            // If IK is not active, reset the head, hands and feet back to their original position
            else
            {
                animator.SetLookAtWeight(0f);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0f);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0f);
            }
        }
    }
}