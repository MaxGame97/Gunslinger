using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // Private class used to keep track of speed multipliers
    private class SpeedMultiplier
    {
        // The multipliers are stored in two dimensional arrays
        // Index 0 defines the actual multiplier
        // Index 1 defines the time when the multiplier runs out

        // Contains the multiplier arrays
        private List<float[]> multipliers = new List<float[]>();
        
        // Adds a new multiplier
        public void NewMultiplier(float multiplier, float time)
        {
            multipliers.Add(new float[2] {multiplier, Time.time + time});
        }

        // Clears all multipliers
        public void ClearMultiplier()
        {
            multipliers.Clear();
        }

        // Gets the current multiplier
        public float GetMultiplier()
        {
            // The default multiplier is 1f
            float multiplier = 1f;

            // Iterate through all multipliers
            for(int i = 0; i < multipliers.Count; i++)
            {
                // If the current multiplier's time has run out
                if (Time.time > multipliers[i][1])
                {
                    // Remove the current multiplier and continue iterating
                    multipliers.RemoveAt(i);
                    i--;
                }
                else
                {
                    // Apply the current multiplier
                    multiplier *= multipliers[i][0];
                }
            }

            // Return the calculated multiplier
            return multiplier;
        }
    }

    // ------------------------
    // --- Public variables ---
    // ------------------------

    public float MovementSpeed { get { return movementSpeed; } }


    [SerializeField] private float movementSpeed = 6.0f;    // Defines the player's movement speed
    [SerializeField] private float jumpSpeed = 8.0f;        // Defines the player's jumping speed
    [SerializeField] private float gravity = 20.0f;         // Defines the gravity that affect the player
    [SerializeField] private float smoothing = 0.2f;        // Defines the amount of smoothing that affect the player

    private CharacterController controller;                 // Defines the player's Character Controller
    private Vector3 desiredVelocity = Vector3.zero;         // Defines the player's current velocity
    private Vector3 currentVelocity = Vector3.zero;         // Defines the player's current velocity
    private SpeedMultiplier speedMultiplier;                // Defines the player's current speed multiplier

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

        // Construct the speed multiplier
        speedMultiplier = new SpeedMultiplier();
    }

    // Update is called once per frame
    void Update()
    {
        // If the Character Controller is grounded
        if (controller.isGrounded)
        {
            // Set the vertical movement to zero
            currentVelocity.y = 0f;

            // Get the desired velocity from input
            desiredVelocity = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            // Normalize the desired velocity when necessary
            float magnitude = Mathf.Abs(desiredVelocity.x) + Mathf.Abs(desiredVelocity.z);
            if (magnitude > 1f)
            {
                desiredVelocity /= magnitude;
            }
            
            // Apply speed multipliers to the desired velocity
            desiredVelocity *= speedMultiplier.GetMultiplier();
            if (Input.GetButton("Fire3"))
                desiredVelocity *= 0.5f;

            // Apply movement to the desired velocity (in world space)
            desiredVelocity = transform.TransformDirection(desiredVelocity);
            desiredVelocity *= movementSpeed;

            // Lerp between the current velocity and the desired velocity
            currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, smoothing);

            /*
            // If the jump buttom was pressed
            if (Input.GetButtonDown("Jump"))
            {
                // Apply jumping to the current velocity
                currentVelocity.y = jumpSpeed;
            }
            */
        }

        // Apply gravity to the current velocity
        currentVelocity.y -= gravity * Time.deltaTime;

        // Move the Character Controller based on the player's current velocity
        controller.Move(currentVelocity * Time.deltaTime);
    }

    // Public method to apply speed multipliers
    public void NewSpeedMultiplier(float multiplier, float time)
    {

    }
}
