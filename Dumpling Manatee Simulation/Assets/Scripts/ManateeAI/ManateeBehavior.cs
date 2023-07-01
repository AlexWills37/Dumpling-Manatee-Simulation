using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Defines the manatee's AI/behavior.
/// The manatee AI generally:
///     - Choose an action to take
///     - Do the action
///     - Repeat
/// And some actions can be interrupted:
///     - If the player touches the manatee, try to interrupt the action
///     - If action is interruptable, do the ManateePlay action
///     - Else, do nothing
/// 
/// @author Alex Wills
/// @date 6/20/2023
/// </summary>
[RequireComponent(typeof(Rigidbody))]
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


    protected Rigidbody manateeRb;
    

    private Animator animator;

    // Particle variable
    private ParticleSystem.EmissionModule happyParticleSettings;

    // How the manatee knows if it is at the surface (object with the 'Air' tag)
    public bool atSurface {get; private set;} = false;
    // How the manatee knows if it is too close to the player (trigger collider with 'PersonalSpace' tag)
    public bool inPersonalSpace {get; private set;} = false;

    private float currentTimeWithoutBreath = 0f;    // Timer for breathing occasionally

    private bool currentActionActive = false;

    private AbstractAction swim, breathe, rest, play, turnAround;
    private AbstractAction currentAction = null;


    // Start is called before the first frame update
    void Start()
    {
        // Get specific components

        manateeRb = this.GetComponent<Rigidbody>();

        animator = this.GetComponentInChildren<Animator>();
        // manateeSound = this.GetComponent<AudioSource>();

        happyParticleSettings = happyParticles.emission;
        happyParticleSettings.rateOverTime = 0; // Stop the manatee from emitting particles

        swim = new ManateeSwim(this, movementSpeed, rotationSpeed);
        rest = new ManateeWait(this);
        breathe = new ManateeBreathe(this, movementSpeed);
        play = new ManateePlay(this, happyParticleSettings);
        turnAround = new ManateeChangeDirection(this, rotationSpeed);
    }



    // Update is called once per frame
    void Update()
    {
        currentTimeWithoutBreath += Time.deltaTime;
        // Choose next action when the current action ends, and the manatee is not in personal space.
        if (!currentActionActive && !inPersonalSpace) {
            ChooseNextAction();
        }

        if (Input.GetMouseButton(0)) {
            PlayerInteraction();
        }

    }

    private void ChooseNextAction() {
        if (currentTimeWithoutBreath >= breathTime) {
            currentAction = breathe;
            Debug.Log("Starting breathe");
        }
        else {
            
            RaycastHit hit;
            Ray ray = new Ray(this.transform.position, this.transform.forward);


            if (Physics.Raycast(ray, out hit, 20) && hit.distance < 10f) {
                currentAction = turnAround;
                Debug.Log("Changing Direction.");
            } else {

                int randomNum = (int)(Random.Range(0, 2));
                // int randomNum = 1;  // No swimming yet. Only rest and breathe
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
                
            }




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

    public void RefillBreath() {
        this.currentTimeWithoutBreath = 0;
    }

    private void PlayerInteraction() {
        // If we are able to interrupt the current action, play with the player
        bool actionStopped = currentAction.StopAction();
        if (actionStopped) {
            currentAction = play;
            currentAction.StartAction();
            currentActionActive = true;
        }
    }

    private void OnTriggerEnter(Collider other) {
        switch (other.gameObject.tag) {
            case "Air":
                atSurface = true;
                break;
            case "PersonalSpace":
                inPersonalSpace = true;
                Debug.Log("manatee in personal space");
                break;
            case "Player":
                // Player-manatee interaction
                this.PlayerInteraction();
                break;
            default:
                break;
        }
    }

    private void OnTriggerExit(Collider other) {
        switch (other.gameObject.tag) {
            case "Air":
                atSurface = false;
                break;
            case "PersonalSpace":
                inPersonalSpace = false;
                Debug.Log("Manatee left personal space");
                break;
            default:
                break;
        }
    }
}
