using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewManatee : MonoBehaviour
{

    [Tooltip("How quickly the manatee should move around")]
    [SerializeField] protected float movementSpeed = 4f;

    [Tooltip("How quickly the manatee should rotate")]
    [SerializeField] protected float rotationSpeed = 10f;

    [Tooltip("Particle system to emit particles when the manatee is booped by the player")]
    [SerializeField] private ParticleSystem happyParticles;

    [Tooltip("How many seconds the manatee will take before going up to breathe air")]
    [Range(5f, 300f)]
    [SerializeField] private float breathTime = 30f;

    [Tooltip("The game object with this manatee's physical colliders and scripts")]
    [SerializeField] protected ManateePhysicalCollider physicalManatee;

    protected Rigidbody manateeRb;
    
    private Animator animator;

    // Particle variable
    private ParticleSystem.EmissionModule happyParticleSettings;

    // How the manatee knows if it is at the surface (object with the 'Air' tag)
    private bool atSurface;

    private float currentTimeWithoutBreath = 0f;    // Timer for breathing occasionally

    // The current coroutine that the manatee is executing to do something
    private IEnumerator currentAction;
    private bool currentActionActive;

    // Start is called before the first frame update
    void Start()
    {
        // Get specific components

        manateeRb = physicalManatee.gameObject.GetComponent<Rigidbody>();

        animator = this.GetComponentInChildren<Animator>();
        // manateeSound = this.GetComponent<AudioSource>();

        happyParticleSettings = happyParticles.emission;
        happyParticleSettings.rateOverTime = 0; // Stop the manatee from emitting particles
    }



    // Update is called once per frame
    void Update()
    {
        currentTimeWithoutBreath += Time.deltaTime;
        if (!currentActionActive) {
            ChooseNextAction();
        }
    }

    private void ChooseNextAction() {
        if (currentTimeWithoutBreath >= breathTime) {
            currentAction = SurfaceAndBreathe();
        }
    }


    /// <summary>
    /// Ends the current action for the system, so that the system can choose its next action.
    /// </summary>
    private void EndCurrentAction() {
        if (currentAction != null) {
            StopCoroutine(currentAction);
            currentAction = null;
        }
        currentActionActive = false;
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

        // isSwimming = true;

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
        yield return new WaitForSeconds(5f);
        animator.SetBool("isBreathing", false);

        // Swim back down to the original point
        while (this.transform.position.y > originalY)
        {
            this.transform.Translate(Vector3.down * movementSpeed * Time.deltaTime, Space.World);
            yield return null;
        }

        // End coroutine and reset the manatee's breath timer
        currentTimeWithoutBreath = 0f;
        // isSwimming = false;
        EndCurrentAction();
    }
}
