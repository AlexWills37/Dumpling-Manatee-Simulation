using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines how the manatees should behave.
/// 
/// Requirements:
/// ManateeVisionTrigger and ManateePhysicalCollider scripts to control the
/// followingPlayer and inPlayersSpace booleans respectively.
/// 
/// This script should be attached to the parent with the rigidbody for this manatee.
/// 
//  Hierarchy (order does not matter):
/// 
/// > Manatee (attach this script here)
/// | > Mesh render
/// | > Large trigger collider to detect the player (attach ManateeVisionTrigger here)
/// | > Small collider (not trigger) to define the manatee's bounds (attach ManateePhysicalCollider here)
/// 
/// Ensure that you drag the object with this manatee script into the two collider scripts, and
/// ensure that you drag the player's personal space collider into the PhysicalCollider script.
/// 
/// @author Alex Wills
/// Updated 6/22/22
/// </summary>
public class ManateeBehavior : MonoBehaviour
{
    [Tooltip("How quickly the manatee should move around.")]
    [SerializeField] protected float movementSpeed = 4f;

    [Tooltip("Particle system to emit particles when the manatee is booped by the player.")]
    [SerializeField] private ParticleSystem happyParticles;

    [Tooltip("How many seconds the manatee will take before going up to breathe air.")]
    [Range(5f, 300f)]
    [SerializeField] private float breathTime = 30f;

    private Animator animator;

    // This is how the manatee knows when to stop moving forward
    private bool inPlayersSpace = false;

    // This is how the manatee knows when to move towards the player
    private bool followingPlayer = false;

    // This is how the manatee knows if it is at the surface
    private bool atSurface = false;

    // This is how the manatee knows if the SwimForward coroutine is active
    private bool isSwimming = false;

    // This is how the manatee knows if the RotateSlowly coroutine is active
    protected bool isRotating = false;
    private float rotationSpeed = 10f;

    [SerializeField] protected ManateePhysicalCollider physicalManatee;

    protected Rigidbody manateeRb;


    private ParticleSystem.EmissionModule happyParticleSettings;

    // How long the manatee has been underwater
    private float currentTimeWithoutBreath = 0f;

    private AudioSource manateeSound;

    private IEnumerator swimmingForward;

    // Start is called before the first frame update
    protected void Start()
    {
        // Get specific components

        // manateeRb = physicalManatee.gameObject.GetComponent<Rigidbody>();
        manateeRb = this.GetComponent<Rigidbody>();
        animator = this.GetComponentInChildren<Animator>();
        manateeSound = this.GetComponent<AudioSource>();

        happyParticleSettings = happyParticles.emission;
        happyParticleSettings.rateOverTime = 0; // Stop the manatee from emitting particles

    }

    // Update is called once per frame
    virtual protected void Update()
    {
        currentTimeWithoutBreath += Time.deltaTime;

        // Swim at set intervals
        if (!isSwimming)
        {
            // Go up to breathe if enough time has passed. This takes priority over other actions.
            if(currentTimeWithoutBreath >= breathTime)
            {
                StartCoroutine(SurfaceAndBreathe());
            }

            // Move toward the player if they are in range, between this manatee's vision and the player's
            // personal space
            else if (followingPlayer && !inPlayersSpace)
            {
                // NOTE: This commented out code makes the manatee swim towards the player if they are close.
                // It is commented out to stop the manatee from swimming towards the player, which could be unsettling.
                // This code could be rewritten to slowly rotate and move towards the player in a less jarring way.

                // Move a bit below the player's camera (so that the manatee isn't directly staring at you)
                // animator.SetBool("isSwimming", true);
                // this.transform.LookAt((player.transform.position - new Vector3(0, -1, 0)), Vector3.up);
                // this.transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime, Space.Self);

            // If not moving towards the player and not in their space, swim around slowly
            } else if (!inPlayersSpace)
            {

                swimmingForward = SwimForward();
               
                StartCoroutine(swimmingForward);
            }

            // If the manatee is in the player's space and not out of breath, it will stay still
        }

        // Rotate at set intervals
        if (!isRotating)
        {
            StartCoroutine(RotateSlowly());

        }
    }

    /// <summary>
    /// Increase the player's health, play a happy animation, and display particles for the manatee to respond
    /// to interaction.
    /// </summary>
    public void InteractWithManatee()
    {
        // Do not start the animation if the animation is already occuring
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Rolling"))
        {
            animator.SetBool("isRolling", true);
        }

        // Stop swimming if we are swimming
        if (isSwimming) {
            StartCoroutine(StopSwimming());
        }

        TelemetryManager.entries.Add(
            new TelemetryEntry("manateeInteraction")
        );

        // Do show particles and increase player health regardless
        StartCoroutine(HappyParticleCoroutine());
        // TODO:: THIS IS BROKEN AT THE MOMENT
        // PlayerScript.currentHealth += 2;
        // PlayerScript.interactedWithManatee = true;  // This line lets the PlayerScript know about the interaction, which will let the TaskList know about the interaction

        // Give haptic and audio feedback
        HapticFeedback.singleton.TriggerVibrationTime(0.05f);
        if (!manateeSound.isPlaying)    // Do not replay audio if it is already playing
        {
            manateeSound.Play();
        }
    }

    /// <summary>
    /// Release some particles for a second
    /// </summary>
    /// <returns> Coroutine </returns>
    private IEnumerator HappyParticleCoroutine()
    {
        happyParticleSettings.rateOverTime = 15;
        yield return new WaitForSeconds(1f);
        happyParticleSettings.rateOverTime = 0;
    }

    /// <summary>
    /// Slowly rotate the manatee in a new direction
    /// </summary>
    /// <returns> Coroutine </returns>
    protected IEnumerator RotateSlowly()
    {
        isRotating = true;

        // Choose the direction to rotate
        float totalRotation = Random.Range(-180f, 180f);
        Debug.Log("ROTATING: " + totalRotation);
        float elapsedRotation = 0f; // This is how much we have rotated so far
        int negativeMultiplier = 1;
        if(totalRotation < 0)
        {
            negativeMultiplier = -1;
        }

        // Rotate over multiple frames
        while(elapsedRotation < Mathf.Abs(totalRotation))
        {
            this.transform.Rotate(Vector3.up, rotationSpeed * negativeMultiplier * Time.deltaTime, Space.World);
            elapsedRotation += rotationSpeed * Time.deltaTime;

            // Update velocity to match new rotation
            manateeRb.velocity = manateeRb.velocity.magnitude * this.transform.forward;

            yield return null;
        }

        // Wait a random amount of time before rotating again
        yield return new WaitForSeconds(Random.Range(1, 5));

        isRotating = false;
    }

    /// <summary>
    /// Coroutine to swim forward for a bit.
    /// </summary>
    /// <returns> Coroutine </returns>
    private IEnumerator SwimForward()
    {
        animator.SetBool("isSwimming", true);
        isSwimming = true;

        // Set velocity forward for a bit of time
        manateeRb.velocity = this.transform.forward * movementSpeed;
        manateeRb.drag = 0;

        // Swim for a random amount of time
        yield return new WaitForSeconds(Random.Range(1, 5));

        // Come to a slow stop by adding drag for a bit of time
        StartCoroutine(StopSwimming());
    }

    private IEnumerator StopSwimming()
    {


        
        animator.SetBool("isSwimming", false);
        // Add drag to slow the manatee down
        manateeRb.drag = 1;
        yield return new WaitForSeconds(4);

        // Add random delay to make behavior less choppy
        yield return new WaitForSeconds(Random.Range(0, 2));
        isSwimming = false;
    }

    /// <summary>
    /// Moves the manatee to the surface, waits a little bit and goes back down.
    /// </summary>
    /// <returns> IEnumerator representing Coroutine to surface and breathe. </returns>
    private IEnumerator SurfaceAndBreathe()
    {

        //// Add a telemetry entry for breathing
        //TelemetryManager.entries.Add(
        //    new TelemetryEntry("manateeBreathing")
        //);

        isSwimming = true;

        // Record the original Y so that we can float back down to this point
        manateeRb.velocity = Vector3.zero;
        float originalY = this.transform.position.y;

        // Swim upwards
        // Debug.Log("Going up");
        while (!atSurface)
        {
            this.transform.Translate(Vector3.up * movementSpeed * Time.deltaTime, Space.World);
            yield return null;
        }

        // Wait to take a breath
        animator.SetBool("isBreathing", true);
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1){
            yield return null;
        }
        // yield return new WaitForSeconds(5f);
        animator.SetBool("isBreathing", false);

        // Swim back down to the original point
        while (this.transform.position.y > originalY)
        {
            this.transform.Translate(Vector3.down * movementSpeed * Time.deltaTime, Space.World);
            yield return null;
        }

        // End coroutine and reset the manatee's breath timer
        currentTimeWithoutBreath = 0f;
        isSwimming = false;
    }


    // Methods to set different status-representing booleans. Call these methods from specific collider interactions.

    /// <summary>
    /// Set this manatee's status for following the player.
    /// To be used by the collider that detects objects around the manatee.
    /// </summary>
    /// <param name="isFollowingPlayer"> whether or not the manatee is following the player. </param>
    public void SetPlayerFollow(bool isFollowingPlayer)
    {
        this.followingPlayer = isFollowingPlayer;
        Debug.LogWarning("Manatee is " + (isFollowingPlayer ? "" : "not") + " following player!");
    }

    /// <summary>
    /// Set this manatee's status for being in the player's personal space.
    /// To be used by the collider that acts as the manatee's outline.
    /// </summary>
    /// <param name="isInPlayerSpace"> whether or not the manatee is in the player's personal space. </param>
    public void SetInPlayerSpace(bool isInPlayerSpace)
    {
        this.inPlayersSpace = isInPlayerSpace;
        Debug.LogWarning("Manatee is " + (isInPlayerSpace ? "" : "not") + " in the player's space!");

        // If we enter the player's space, stop the manatee from moving forward
        if (inPlayersSpace)
        {
            if(swimmingForward != null)
            {
                // Interrupt swimming forward and skip to the part where the manatee slows down
                StopCoroutine(swimmingForward);
                StartCoroutine(StopSwimming());
            }
        }
    }

    /// <summary>
    /// Sets whether this manatee is at the surface or not.
    /// This method should be called by the physical collider that the manatee uses.
    /// </summary>
    /// <param name="isAtSurface"> whether or not this manatee is at the surface. </param>
    public void SetAtSurface(bool isAtSurface)
    {
        this.atSurface = isAtSurface;
        Debug.LogWarning("Manatee is " + (isAtSurface ? "" : "not") + " at the surface!");
    }


}
