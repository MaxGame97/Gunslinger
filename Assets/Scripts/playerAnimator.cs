using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    [SerializeField] private Vector3 revolverPositionOffset;    // Defines the revolver target's position offset
    [SerializeField] private Vector3 revolverRotationOffset;    // Defines the revolver target's rotation offset
    [SerializeField] private float smoothing;                   // Deines the amount of aim smoothing

    private PlayerController player;                            // Defines the player's Player Controller
    private CharacterController controller;                     // Defines the player's Character Controller
    private Animator animator;                                  // Defines the player's Animator
    private CameraController cameraController;                  // Defines the player's Camera Controller
    private Vector3 currentVelocity;                            // Defines the player's current velocity
    private Quaternion currentAimingRotation;                   // Defines the player's current aiming rotation

    private GameObject lookTarget;                              // Defines the player's look IK target
    private GameObject leftHandTarget;                          // Defines the player's left hand IK target
    private GameObject rightHandTarget;                         // Defines the player's right hand IK target
    private GameObject leftFootTarget;                          // Defines the player's left foot IK target
    private GameObject rightFootTarget;                         // Defines the player's right foot IK target

    // Use this for initialization
    void Start ()
    {
        // Get the attached Player Controller, Character Controller, Animator and Camera Controller
        player = GetComponent<PlayerController>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cameraController = GetComponentInChildren<CameraController>();

        // If no Player Controller, Character Controller, Animator or Camera Controller was found
        if (player == null || controller == null || animator == null)
        {
            // Throw an error message and deactivate this object
            Debug.LogError("No Player Controller, Character Controller, Animator or Camera Controller found");
            this.enabled = false;
        }

        // Get the attached IK targets
        lookTarget = transform.GetChild(2).GetChild(0).gameObject;
        leftHandTarget = transform.GetChild(2).GetChild(1).gameObject;
        rightHandTarget = transform.GetChild(2).GetChild(2).gameObject;
        leftFootTarget = transform.GetChild(2).GetChild(3).gameObject;
        rightFootTarget = transform.GetChild(2).GetChild(4).gameObject;
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


        Vector3 offsetPoint = transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).position;

        // Lerp between the current aiming rotation and the desired aiming rotation
        currentAimingRotation = Quaternion.Lerp(currentAimingRotation, Quaternion.LookRotation(cameraController.AimingPosition - offsetPoint), smoothing);

        // Rotate the weapon rig based on the current aiming position
        transform.GetChild(1).rotation = currentAimingRotation;

        RaycastHit hit;

        if(Physics.Raycast(offsetPoint, transform.GetChild(1).forward, out hit, Mathf.Infinity))
        {
            Debug.DrawLine(offsetPoint, hit.point, Color.yellow);
        }


        // Update the look IK target based on the aiming position
        SetNewTarget(0, cameraController.AimingPosition);

        // Update the right hand IK target based on the position of the revolver
        SetNewTarget(2, transform.GetChild(1).GetChild(0).position + transform.GetChild(1).GetChild(0).rotation * revolverPositionOffset, Quaternion.Euler(transform.GetChild(1).GetChild(0).eulerAngles + revolverRotationOffset));
    }

    // Updates the position of a given IK target
    public void SetNewTarget(int index, Vector3 position)
    {
        switch (index)
        {
            // Look target
            case 0:
                lookTarget.transform.position = position;
                break;

            // Left hand target
            case 1:
                leftHandTarget.transform.position = position;
                break;

            // Right hand target
            case 2:
                rightHandTarget.transform.position = position;
                break;

            // Left foot target
            case 3:
                leftFootTarget.transform.position = position;
                break;

            // Right foot target
            case 4:
                rightFootTarget.transform.position = position;
                break;

            default:
                // If index is not between 0 and 4, throw a warning
                Debug.LogWarning(index + "is not a valid target index");
                break;
        }
    }

    // Updates the rotation of a given IK target
    public void SetNewTarget(int index, Quaternion rotation)
    {
        switch (index)
        {
            // Look target
            case 0:
                lookTarget.transform.rotation = rotation;
                break;

            // Left hand target
            case 1:
                leftHandTarget.transform.rotation = rotation;
                break;

            // Right hand target
            case 2:
                rightHandTarget.transform.rotation = rotation;
                break;

            // Left foot target
            case 3:
                leftFootTarget.transform.rotation = rotation;
                break;

            // Right foot target
            case 4:
                rightFootTarget.transform.rotation = rotation;
                break;

            default:
                // If index is not between 0 and 4, throw a warning
                Debug.LogWarning(index + "is not a valid target index");
                break;
        }
    }

    // Updates the position and rotation of a given IK target
    public void SetNewTarget(int index, Vector3 position, Quaternion rotation)
    {
        switch (index)
        {
            // Look target
            case 0:
                lookTarget.transform.position = position;
                lookTarget.transform.rotation = rotation;
                break;

            // Left hand target
            case 1:
                leftHandTarget.transform.position = position;
                leftHandTarget.transform.rotation = rotation;
                break;

            // Right hand target
            case 2:
                rightHandTarget.transform.position = position;
                rightHandTarget.transform.rotation = rotation;
                break;

            // Left foot target
            case 3:
                leftFootTarget.transform.position = position;
                leftFootTarget.transform.rotation = rotation;
                break;

            // Right foot target
            case 4:
                rightFootTarget.transform.position = position;
                rightFootTarget.transform.rotation = rotation;
                break;

            default:
                // If index is not between 0 and 4, throw a warning
                Debug.LogWarning(index + "is not a valid target index");
                break;
        }
    }
}
