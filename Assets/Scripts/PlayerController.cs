using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ------------------------
    // --- Public variables ---
    // ------------------------

    public float MovementSpeed { get { return movementSpeed; } }


    [SerializeField] private float movementSpeed = 6.0f;    // Defines the player's movement speed
    [SerializeField] private float jumpSpeed = 8.0f;        // Defines the player's jumping speed
    [SerializeField] private float gravity = 20.0f;         // Defines the gravity that affect the player
    [SerializeField] private float smoothing = 0.2f;        // Defines the amount of smoothing that affect the player

    private CharacterController controller;                 // Defines the player's Character Controller
    private Vector3 desiredVelocity = Vector3.zero;         // Definfes the player's current velocity
    private Vector3 currentVelocity = Vector3.zero;         // Definfes the player's current velocity

    // Use this for initialization
    void Start () {
        // Get the attached Character Controller
        controller = GetComponent<CharacterController>();

        // If no Character Controller was found
        if(controller == null)
        {
            // Throw an error message and deactivate this object
            Debug.LogError("No Character Controller found");
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // If the Character Controller is grounded
        if (controller.isGrounded)
        {
            // Get the desired velocity from input
            desiredVelocity = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            // Calculate the magnitude of the desired velocity
            float magnitude = Mathf.Abs(desiredVelocity.x) + Mathf.Abs(desiredVelocity.z);

            // Normalize the desired velocity when necessary
            if (magnitude > 1f)
            {
                desiredVelocity /= magnitude;
            }
            
            // Apply movement to the desired velocity (in world space)
            desiredVelocity = transform.TransformDirection(desiredVelocity);
            desiredVelocity *= movementSpeed;

            // --------------------------------------------------------------------------
            // --- DEBUG - Double the desired velocity when the sprint key is pressed ---
            // --------------------------------------------------------------------------
            if (Input.GetButton("Fire3"))
                desiredVelocity *= 2.0f;

            // Lerp between the current velocity and the desired velocity
            currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, smoothing);

            // If the jump buttom was pressed
            if (Input.GetButtonDown("Jump"))
            {
                // Apply jumping to the current velocity
                currentVelocity.y = jumpSpeed;
            }
        }

        // Apply gravity to the current velocity
        currentVelocity.y -= gravity * Time.deltaTime;

        // Move the Character Controller based on the player's current velocity
        controller.Move(currentVelocity * Time.deltaTime);
    }
}
