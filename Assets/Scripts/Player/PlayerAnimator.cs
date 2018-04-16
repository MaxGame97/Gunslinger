using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    [Header("Animation settings")]
    [SerializeField] private Vector3 rightHandPositionOffset;   // Offsets the right hand's position (To align with weapon)
    [SerializeField] private Vector3 rightHandRotationOffset;   // Offsets the right hand's rotation (To align with weapon)
    [SerializeField] private Vector3 disabledDirection;         // The direction that the weapon will point towards when aiming is disabled
    [SerializeField] private float smoothing;                   // Defines the amount of aim smoothing
    [SerializeField] private LayerMask layerMask;               // Defines what layers that can be raycasted

    [Header("Aiming restriction settings")]
    [SerializeField] private float maxAngle;                    // The max angle that the player is able to aim towards
    [SerializeField] private float minWeaponDepth;              // How far the weapon is able to be retracted
    [SerializeField] private float maxWeaponDepth;              // How far the weapon is able to be extended
    [SerializeField] private float weaponDepthCorrection;       // Corrects the raycast so the weapon doesn't extend into walls

    private CharacterController controller;                     // Defines the player's Character Controller
    private Animator animator;                                  // Defines the player's Animator
    private PlayerController player;                            // Defines the player's Player Controller
    private CameraController cameraController;                  // Defines the player's Camera Controller

    private Transform weaponRig;                                // Defines the player's weapon rig
    private Transform weapon;                                   // Defines the player's weapon

    private Transform lookTarget;                               // Defines the player's look IK target
    private Transform leftHandTarget;                           // Defines the player's left hand IK target
    private Transform rightHandTarget;                          // Defines the player's right hand IK target
    private Transform leftFootTarget;                           // Defines the player's left foot IK target
    private Transform rightFootTarget;                          // Defines the player's right foot IK target


    private Vector3 currentVelocity;                            // Defines the player's current velocity
    private Quaternion currentAimingRotation;                   // Defines the player's current aiming rotation
    private bool aimingEnabled;                                 // Specifies whether the player is able to aim or not

    // Use this for initialization
    void Start ()
    {
        // Get the attached Character Controller,  Animator, Player Controller and Camera Controller
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerController>();
        cameraController = GetComponentInChildren<CameraController>();

        // If no Character Controller,  Animator, Player Controller or Camera Controller was found
        if (player == null || controller == null || animator == null)
        {
            // Throw an error message and deactivate this object
            Debug.LogError("No Character Controller,  Animator, Player Controller or Camera Controller found");
            this.enabled = false;
        }

        // Get the attached weapon and weapon rig
        weaponRig = transform.GetChild(1);
        weapon = transform.GetChild(1).GetChild(0);

        // If no weapon rig or weapon was found
        if(weaponRig == null || weapon == null)
        {
            // Throw a warning
            Debug.LogWarning("No weapon rig or weapon was found");
        }

        // Get the attached IK targets
        lookTarget = transform.GetChild(2).GetChild(0);
        leftHandTarget = transform.GetChild(2).GetChild(1);
        rightHandTarget = transform.GetChild(2).GetChild(2);
        leftFootTarget = transform.GetChild(2).GetChild(3);
        rightFootTarget = transform.GetChild(2).GetChild(4);

        // If some IK targets were not found
        if(lookTarget == null || leftHandTarget == null || rightHandTarget == null || leftFootTarget == null || rightFootTarget == null)
        {
            // Throw a warning
            Debug.LogWarning("Some IK targets were not found");
        }


        // Set the current Aiming rotation to the current rotation
        currentAimingRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update ()
    {
        OffsetWeapon();
        AimWeapon();
        UpdateIKPositions();
        
        
        // -----------------------------------------
        // --- Update Animator Controller values ---
        // -----------------------------------------

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
        animator.SetBool("Aiming Enabled", aimingEnabled);
    }

    // Offsets the weapon towards where the player is pointing
    private void OffsetWeapon()
    {
        // If the angle towards the aiming position is too large
        if (Vector3.Angle(cameraController.transform.forward, cameraController.AimingPosition - weaponRig.position) > maxAngle)
        {
            // Offset the weapon position (Minimum offset)
            weapon.localPosition = new Vector3(0f, 0f, minWeaponDepth);

            // Set aiming to be disabled
            aimingEnabled = false;

            return;
        }
        
        RaycastHit hit;

        Vector3 origin = weaponRig.position;
        Vector3 direction = (cameraController.AimingPosition - weaponRig.position).normalized;
        float distance = maxWeaponDepth + weaponDepthCorrection;

        // CASE 1 - No obstruction (Weapon is far from a wall)
        if (!Physics.Raycast(origin, direction, out hit, distance, layerMask))
        {
            // Draw a debug line indicating no obstruction
            Debug.DrawLine(origin, weaponRig.position + direction * distance, Color.green);

            // Offset the weapon position (Maximum offset)
            weapon.localPosition = new Vector3(0f, 0f, maxWeaponDepth);

            // Set aiming to be enabled
            aimingEnabled = true;
        }
        // CASE 2 - Obstruction (Weapon is close to a wall)
        else if (hit.distance - weaponDepthCorrection >= minWeaponDepth)
        {
            // Draw a debug line indicating obstruction
            Debug.DrawLine(origin, hit.point, Color.yellow);

            // Offset the weapon position (Intermediate offset)
            weapon.localPosition = new Vector3(0f, 0f, hit.distance - weaponDepthCorrection);

            // Set aiming to be enabled
            aimingEnabled = true;
        }
        // CASE 3 - Intersection (Weapon is too close to a wall)
        else
        {
            // Draw a debug line indicating intersection
            Debug.DrawLine(origin, hit.point, Color.red);

            // Offset the weapon position (Minimum offset)
            weapon.localPosition = new Vector3(0f, 0f, minWeaponDepth);

            // Set aiming to be disabled
            aimingEnabled = false;
        }
    }

    // Aims the weapon towards where the player is pointing
    void AimWeapon()
    {
        if (aimingEnabled)
        {
            // Interpolate between the current aiming rotation and the desired aiming rotation
            currentAimingRotation = Quaternion.Lerp(currentAimingRotation, Quaternion.LookRotation(cameraController.AimingPosition - weapon.GetChild(0).position), smoothing);
        }
        else
        {
            // Interpolate between the current aiming rotation and the disabled aiming rotation
            currentAimingRotation = Quaternion.Lerp(currentAimingRotation, Quaternion.LookRotation(transform.TransformDirection(disabledDirection.normalized)), smoothing);
        }

        weaponRig.rotation = currentAimingRotation;
    }

    // Updates the positions of the IK targets
    void UpdateIKPositions()
    {
        // Update the look IK target based on the aiming position
        SetNewTarget(0, cameraController.AimingPosition);

        // Update the right hand IK target based on the position of the weapon
        SetNewTarget(2, weapon.position + weapon.rotation * rightHandPositionOffset, Quaternion.Euler(weapon.eulerAngles + rightHandRotationOffset));
    }

    // Updates the position of a given IK target
    public void SetNewTarget(int index, Vector3 position)
    {
        switch (index)
        {
            // Look target
            case 0:
                lookTarget.position = position;
                break;

            // Left hand target
            case 1:
                leftHandTarget.position = position;
                break;

            // Right hand target
            case 2:
                rightHandTarget.position = position;
                break;

            // Left foot target
            case 3:
                leftFootTarget.position = position;
                break;

            // Right foot target
            case 4:
                rightFootTarget.position = position;
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
                lookTarget.rotation = rotation;
                break;

            // Left hand target
            case 1:
                leftHandTarget.rotation = rotation;
                break;

            // Right hand target
            case 2:
                rightHandTarget.rotation = rotation;
                break;

            // Left foot target
            case 3:
                leftFootTarget.rotation = rotation;
                break;

            // Right foot target
            case 4:
                rightFootTarget.rotation = rotation;
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
                lookTarget.position = position;
                lookTarget.rotation = rotation;
                break;

            // Left hand target
            case 1:
                leftHandTarget.position = position;
                leftHandTarget.rotation = rotation;
                break;

            // Right hand target
            case 2:
                rightHandTarget.position = position;
                rightHandTarget.rotation = rotation;
                break;

            // Left foot target
            case 3:
                leftFootTarget.position = position;
                leftFootTarget.rotation = rotation;
                break;

            // Right foot target
            case 4:
                rightFootTarget.position = position;
                rightFootTarget.rotation = rotation;
                break;

            default:
                // If index is not between 0 and 4, throw a warning
                Debug.LogWarning(index + "is not a valid target index");
                break;
        }
    }
}
