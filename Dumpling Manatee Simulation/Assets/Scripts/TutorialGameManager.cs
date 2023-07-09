using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script controls the unique progression of the tutorial scene.
/// It connects with the Slide Deck script, a Task Bar, an edible seagrass object,
/// and the Player Manager to guide the player through the game's main controls.
/// 
/// When the player completes the current task, it will progress to the next slide in the
/// Slide Deck and prepare the components for the next task.
/// 
/// Scene requirements:
///     Slide 0: Move around task
///     Slide 1: Eat seagrass task
///     Slide 2: Surface to breathe
///     Slide 3: Swim down
///     Slide 4: Transition to full game
/// 
/// Inspector requirements:
///     Set the Task Bar to a valid bar to display the current task
///     Set the Tasks 0-3 to describe the following tasks:
///         0: Move around with the thumbstick
///         1: Swim to seagrass to eat it
///         2: Swim to surface to breathe
///         3: Swim back down
///     Set the Tutorial Seagrass to the seagrass that the player can eat
///     Set the Air Collider to the trigger collider that is above the water surface
/// 
/// @author Alex Wills
/// @date 7/8/2023
/// </summary>
[RequireComponent(typeof(SlideDeck))]
public class TutorialGameManager : MonoBehaviour
{
    [Tooltip("Task bar that indicates what the player should do with their controller")]
    [SerializeField] private TaskBar taskBar;

    [Tooltip("Tasks to display for each slide in the task bar")]
    [SerializeField] private string[] tasks;

    private SlideDeck tutorialSlides;   // Slides with the information to dispaly on the player's main HUD
    private int currentTask = 0;    // The current task/slide the player is currently on (see document header for what each index should be)

    private HapticFeedback haptics; // Haptic feedback script

    private PlayerManager player;   // Player manager for connecting to the seagrass event
    private Rigidbody playerRb;     // Player Rigidbody for disabling vertical movement at the start

    // Task 0 - Movement
    private SphereCollider playerMovementDetector;  // Used to detect when the player swims out of the range for the first task

    // Task 1 - Eat seagrass (set the edible seagrass in the inspector)
    [Tooltip("Edible seagrass object to hide until the task for eating seagrass appears")]
    [SerializeField] private GameObject tutorialSeagrass;

    // Task 2, 3 - Swim vertically (set the air collider (trigger) in the inspector)
    [Tooltip("Air collider to detect when the player reaches the surface")]
    [SerializeField] private BoxCollider airCollider;
    private BoxCollider airColliderCopy;   // Copy of air collider to detect the player in this script for swimming vertically
    // By creating a copy of the collider, this script can detect when the player reaches/leaves the surface


    // Start is called before the first frame update
    void Start()
    {
        // Load the first slide
        tutorialSlides = this.GetComponent<SlideDeck>();
        taskBar.ChangeTask(tasks[0]);

        // Get the haptic feedback component
        haptics = HapticFeedback.singleton;

        // Get the player and the player's rigidbody
        player = GameObject.FindObjectOfType<PlayerManager>();
        if (player == null) {
            Debug.LogError("PlayerManager script was not found.");
        }
        playerRb = player.gameObject.GetComponentInChildren<Rigidbody>();
        if (playerRb == null) {
            Debug.LogError("Player's Rigidbody not found in the PlayerManager's children");   
        }

        // Freeze vertical position of the player by adding the Y position constraint (disable swimming up/down)
        playerRb.constraints |= RigidbodyConstraints.FreezePositionY;

        // Set up the first task: moving around (move outside of this sphere trigger)
        playerMovementDetector = this.gameObject.AddComponent<SphereCollider>();
        playerMovementDetector.center = playerRb.transform.position - this.transform.position;
        playerMovementDetector.radius = 0.5f;
        playerMovementDetector.isTrigger = true;

        // Set up the second task: eat grass (detect when the player gains health)
        player.getPlayerValuesEvent().AddListener(EatSeagrass);
        if (tutorialSeagrass == null) {
            Debug.LogError("Seagrass for the TutorialGameManager is not set in the inspector.");
        }
        tutorialSeagrass.SetActive(false);

        // Third and fourth tasks are set up when the second task is complete (in EatSeagrass())
        // Log an error if the air collider is not set
        if (airCollider == null) {
            Debug.LogError("Air Collider for the TutorialGameManager is not set in the inspector.");
        } else if (!airCollider.isTrigger) {
            Debug.LogWarning("Air Collider for the TutorialGameManager is not a trigger collider. It may be the wrong collider.");
        }

        
    }

    /// <summary>
    /// Transitions from the current task to the next task.
    /// Increases the currentTask variables, moves to the next slide,
    /// triggers some haptic feedback, and updates the Task Bar.
    /// </summary>
    private void MoveToNextTask() {
        tutorialSlides.NextSlide();
        currentTask++;
        haptics.TriggerVibrationTime(0.4f);
        
        // If there is another task to display, transition to it.
        if (currentTask < tasks.Length) {
            taskBar.TransitionTask(tasks[currentTask]);
        } else {
            // If there are no more tasks, just complete the current one
            taskBar.CompleteTask();
        }
    }

    /// <summary>
    /// Detect when the player eats seagrass for the first time in the tutorial.
    /// This function is added to the player's playerValuesUpdated event, which triggers
    /// whenever the player eats seagrass or breathes. Since the player will be unable to surface
    /// until after this task, we can be sure that the playerValuesUpdated event indicates
    /// the player eating the tutorial seagrass, and not breathing at the surface.
    /// </summary>
    private void EatSeagrass() {
        // Remove this function from the player event so that it does not call this method again
        player.getPlayerValuesEvent().RemoveListener(EatSeagrass);
        
        // Move to next task (Task 2)
        MoveToNextTask();

        // Allow the player to move up/down by removing the Freeze Y Position constraint with bitwise operations
        // ~ negates the constraint so that FreezePositionY is the only bit with a 0
        // &= with the result keeps the original bits as either 0 or 1 everywhere except for the FreezePositionY bit, which becomes 0 (x & 1 = x, x & 0 = 0)
        playerRb.constraints &= ~RigidbodyConstraints.FreezePositionY;  

        // Lower the player's breath
        player.SetBreath(50);

        // Copy the air collider to detect surfacing/swimming back down
        airColliderCopy = this.gameObject.AddComponent<BoxCollider>();
        airColliderCopy.center = airCollider.center;
        airColliderCopy.size = airCollider.size;
        airColliderCopy.isTrigger = true;
        this.transform.position = airCollider.transform.position;
        this.transform.rotation = airCollider.transform.rotation;

    }

    /// <summary>
    /// Detects the player entering this object's collider for Task 2,
    /// when this object has a copy of the Air Collider.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player") && currentTask == 2) {
            // Player enters the air collider during task 2
            MoveToNextTask();
            // Nothing to setup for task 3. Just wait for player to exit the collider
        }
    }

    /// <summary>
    /// Detects the player exiting this object's collider, which is a small sphere
    /// where the player starts for Task 1, and the air collider for Task 3.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            
            if (currentTask == 0) { // Task 0: Move outside of starting area
                Destroy(playerMovementDetector);
                MoveToNextTask();
                tutorialSeagrass.SetActive(true);   // Prepare for the next task
            
            } else if (currentTask == 3) {  // Task 3: Swim down (out of the air collider)
                Destroy(airColliderCopy);
                MoveToNextTask();
            }
        }
    }
}
