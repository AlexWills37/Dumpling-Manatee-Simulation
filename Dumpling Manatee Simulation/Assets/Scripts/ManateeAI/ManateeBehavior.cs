using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManateeBehavior : MonoBehaviour
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
    private bool atSurface = false;

    private float currentTimeWithoutBreath = 0f;    // Timer for breathing occasionally

    private bool currentActionActive = false;

    private AbstractAction swim, breathe, rest;
    private AbstractAction currentAction = null;

    // Start is called before the first frame update
    void Start()
    {
        // Get specific components

        manateeRb = physicalManatee.gameObject.GetComponent<Rigidbody>();

        animator = this.GetComponentInChildren<Animator>();
        // manateeSound = this.GetComponent<AudioSource>();

        happyParticleSettings = happyParticles.emission;
        happyParticleSettings.rateOverTime = 0; // Stop the manatee from emitting particles

        swim = new ManateeSwim(this, movementSpeed);
        rest = new ManateeWait(this);

    }



    // Update is called once per frame
    void Update()
    {
        currentTimeWithoutBreath += Time.deltaTime;
        if (!currentActionActive) {
            ChooseNextAction();
        }

        if (Input.GetMouseButton(0)) {
            currentAction.ForceEnd();
        }
    }

    private void ChooseNextAction() {
        // if (currentTimeWithoutBreath >= breathTime) {
        //     currentAction = SurfaceAndBreathe();
        // }
        int randomNum = (int)(Random.Range(0, 2));
        switch (randomNum) {
            case 0:
                currentAction = swim;
                Debug.Log("Starting swim.");
                break;
            case 1:
                currentAction = rest;
                // rest.StartAction();
                Debug.Log("Starting rest");
                break;
            default:
                Debug.LogError("Error: No action chosen.");
                break;
        }

        currentAction.StartAction();
        // Prevent this method from being called again until the action is finished
        currentActionActive = true; 
    }


    /// <summary>
    /// Ends the current action for the system, so that the system can choose its next action.
    /// </summary>
    public void EndCurrentAction() {
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


}
