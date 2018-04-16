using UnityEngine;

public class PlayerSoundManager : MonoBehaviour {

    [SerializeField] private AudioClip[] footstepSounds;    // Contains all foot step sounds
    [SerializeField] private float footstepTolerance;       // Footsteps will not be played when the curve value is below this tolerance

    private Animator animator;                              // Defines the player's Animator
    private float previousCurveValue;                       // Defines the previous value of the Animation Curve used for footsteps
    private float currentCurveValue;                        // Defines the current value of the Animation Curve used for footsteps

    // Use this for initialization
    void Start()
    {
        // Get the attached Animator
        animator = GetComponent<Animator>();

        // If no Animator was found
        if(animator == null)
        {
            // Throw an error message and deactivate this object
            Debug.LogError("No Animator found");
            enabled = false;
        }

        // Prepare the curve values
        previousCurveValue = 0f;
        currentCurveValue = animator.GetFloat("Foot Curve");
    }
	
    // Update is called once per frame
    void Update()
    {
        // Update the current curve value
        currentCurveValue = animator.GetFloat("Foot Curve");

        // If the player is on the ground
        if (animator.GetBool("Is Grounded"))
        {
            // Compare the previous curve value with the current curve value
            // If the sign between them is different
            if (Mathf.Sign(previousCurveValue) != Mathf.Sign(currentCurveValue))
            {
                // If the current curve value has exceeded the tolerance
                if(Mathf.Abs(currentCurveValue) > footstepTolerance)
                {
                    // Play a footstep sound
                    Footstep();

                    // Update the previous curve value
                    previousCurveValue = currentCurveValue;
                }
            }
        }
    }

    // Plays a random footstep sound
    void Footstep()
    {
        // If there are assigned footstep sounds
        if (footstepSounds.Length > 0)
            // Play a random footstep sound
            CreateSound(footstepSounds[Random.Range(0, footstepSounds.Length)]);
    }

    // --------------------------------------------------------------------
    // --- DEBUG - Change this based on how it is to be handled by FMOD ---
    // --------------------------------------------------------------------
    // Creates a sound based on the given audio clip
    void CreateSound(AudioClip audioClip)
    {
        // Create a new gameobject and move it to the player
        GameObject sound = new GameObject("Sound Source");
        sound.transform.position = transform.position;

        // Add an audio source to the game object, then set and play the given audio clip
        AudioSource audioSource = sound.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();

        // Set the gameobject to be destroyed when the audio clip has finished playing
        Destroy(sound, audioClip.length);
    }
}
