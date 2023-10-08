using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the scene flow for the "Manatee Hell" scene, where the player revisits the Indian River Lagoon
/// after algae blooms have harmed the environment and the manatees' health.
/// 
/// @author Alex Wills
/// @date 7/15/2023
/// </summary>
public class HellGameManager : MonoBehaviour
{
    
    [SerializeField] private GUITextBox textBox;

    // Variables that are customizable in the inspector for setting up the tasks
    [Tooltip("Task bar to display seagrass task, which will transition to the 'mail letter to human' task")]
    [SerializeField] private TaskBar mainTask;

    [Tooltip("Task bar to display the 'check on friends' task, which will disappear when completed")]
    [SerializeField] private TaskBar secondaryTask;

    [Header("Seagrass Task")]
    [Tooltip("Text to display for the 'eat seagrass' task")]
    [SerializeField] private string seagrassTaskText = "Find seagrass";

    [Tooltip("Number of seagrass to display for the 'eat seagrass' task requirement")]
    [SerializeField] private int displayedGrassRequirement = 10;

    [Tooltip("Actual number of seagrass player must eat to progress the gameplay (should be the total number of seagrass possible to eat)")]
    [SerializeField] private int actualGrassRequirement = 5;

    [Tooltip("How long to wait before notifying the player of the lack of grass (seconds)")]
    [SerializeField] private float grassShortageRealizationTime = 5f;

    
    [Multiline][SerializeField] private string notEnoughGrassText;
    [SerializeField] private float notEnoughGrassTextTime;


    [Header("Interact with manatee task")]
    [Tooltip("Text to display for the 'interact with manatees' task")]
    [SerializeField] private string manateeInteractionTaskText = "Check in on your manatee friends";

    [Multiline][SerializeField] private string manateeImpactInfo;
    [SerializeField] private float manateeImpactInfoTime;


    [Header("Mail to humans task")]
    [Tooltip("Text to display for the 'mail letter to humans' task")]
    [SerializeField] private string mailLetterTaskText = "Send a message to humans for help";

    [Multiline][SerializeField] private string mailLetterText;

    [Tooltip("Trigger collider to send mail to the player")]
    [SerializeField] private BoxCollider mailboxTrigger;

    [Tooltip("Particle effect to give feedback with sending a letter")]
    [SerializeField] private ParticleSystem mailParticles;
    private ParticleSystem.EmissionModule mailParticleEmmiter;


    [Tooltip("The letter to show the player")]
    [SerializeField] private GameObject letterForHumans;
    private BoxCollider mailboxTriggerCopy; // Copy of the mailbox collider to activate this script


    private PlayerManager player;
    private HapticFeedback haptics;
    private int numSeagrassEaten = 0;   // How much seagrass the player has eaten
    private bool learnedAboutManateeImpact = false; // Whether the player has seen the text box about how manatee health has degraded
    private float readingTime = 8f;

    // Variables for displaying text boxes in a coroutine
    private Queue<GameObject> coroutineTextboxes;

    private bool mainTasksCompleted = false;
    private bool readyToSendMail = false;

    private ChangeScene sceneChanger;

    private IEnumerator queuedTextboxCoroutine = null;
    

    // Start is called before the first frame update
    void Start()
    {
        // Set up the tasks
        mainTask.ChangeTask(seagrassTaskText + " (0 / " + displayedGrassRequirement + ")");
        secondaryTask.ChangeTask(manateeInteractionTaskText);
        letterForHumans.SetActive(false);
        coroutineTextboxes = new Queue<GameObject>();
        sceneChanger = this.gameObject.AddComponent<ChangeScene>();


        player = GameObject.FindObjectOfType<PlayerManager>();
        if (player == null) {
            Debug.LogError("Couldnt find player");
        }

        // When the player interacts with a manatee, activate the ManateeInfo coroutine
        player.onManateeInteraction.AddListener( () => { 
            this.StartCoroutine(DisplayManateeImpactInfo()); 
        });

        // When the player eats seagrass, update the seagrass task
        player.onGrassEaten.AddListener( () => {
            numSeagrassEaten = player.ateGrassNum;
            UpdateSeagrassTask();
        });

        mailboxTrigger.enabled = false;
        mailParticleEmmiter = mailParticles.emission;
        mailParticleEmmiter.rateOverTime = 0;

        Button sendLetterButton = letterForHumans.GetComponentInChildren<Button>();
        sendLetterButton.onClick.AddListener(SendLetterToHumans);

        Debug.Log("Game manager successfully initialized.");

        haptics = HapticFeedback.singleton;
    }

    // Update is called once per frame
    void Update()
    {

        // if (Input.GetKeyDown(KeyCode.P)) {
        //     Debug.Log("P");
        //     StartCoroutine(QueueTextboxDisplay(manateeImpactInfo, 5f));

        // }

        // if (Input.GetKeyDown(KeyCode.L)) {
        //     StartCoroutine(QueueTextboxDisplay(manateeImpactInfo2, 4f));
        // }

    }

    private void UpdateSeagrassTask() {
        mainTask.ChangeTask(seagrassTaskText + " (" + numSeagrassEaten + " / " + displayedGrassRequirement + ")");
        if (numSeagrassEaten == actualGrassRequirement) {
            
            // The task is now complete, but we will delay user feedback (and inform them why, later)
            // since the user does not yet know the task is complete.
            StartCoroutine(CompleteSeagrassTask());
        }
    }

    /// <summary>
    /// Completes the seagrass task with some delay.
    /// The player is tasked with finding more seagrass than is available. Once they
    /// find as much seagrass as they can, this coroutine waits to let them wonder how to find more seagrass
    /// before showing them a message explaining the seagrass shortage and completing the task.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CompleteSeagrassTask() {
        // Now that the player has eaten all of the available seagrass, the task is complete
        // In the game, the player will think that they need to find more seagrass.
        // We should let them think this way for a short amount of time before realizing there is not enough grass.
        yield return new WaitForSeconds(grassShortageRealizationTime);

        // Now inform the player what is wrong, and complete the task.
        textBox.DisplayMessage(notEnoughGrassText, notEnoughGrassTextTime);
        haptics.TriggerVibrationTime(0.1f);
        CheckFirstTasks();
    }


    /// <summary>
    /// Runs a coroutine to display manatee information.
    /// Displays the manatee impact info (game object set in the inspector)
    /// for the set "Reading Time" (set in inspector), and checks off the
    /// secondary task for learning about manatees and checking on your friends.
    /// </summary>
    /// <returns> IEnumerator containing this coroutine </returns>
    private IEnumerator DisplayManateeImpactInfo() {
        // manateeImpactInfo.SetActive(true);
        // only show info once, the first time
        if (!learnedAboutManateeImpact) {
            learnedAboutManateeImpact = true;
            textBox.DisplayMessage(manateeImpactInfo, manateeImpactInfoTime);
            yield return new WaitForSecondsRealtime(manateeImpactInfoTime);

            // Complete reading task after the time passes
            if (!mainTasksCompleted) {
                secondaryTask.CompleteTask();
                haptics.TriggerVibrationTime(0.1f);
                CheckFirstTasks();
            }   // If mainTasksCompleted (if the player completes the seagrass task before the manatee info box is finished showing, thus advancing to the final task)
                // then we do not want to give any feedback when the timer completes (at this point, the player has mentally moved on from this task, so feedback would be out of place)
        }

        // manateeImpactInfo.SetActive(false);
    }

    /// <summary>
    /// If the player has completed both tasks, guide the player to the final task: send a letter to humans
    /// to learn how to help.
    /// </summary>
    private void CheckFirstTasks() {
        // NOTE: the seagrass text will not technically be "completed", since there will not be enough seagrass for the player to eat
        // If both tasks are complete, move on to the final task
        if (learnedAboutManateeImpact && numSeagrassEaten >= actualGrassRequirement) {
            mainTasksCompleted = true;  // Set this bool so that if the manateeInfo is still showing, nothing will happen when it finishes showing.


            // Remove the secondary task and change to a single new task
            secondaryTask.gameObject.SetActive(false);
            mainTask.TransitionTask(mailLetterTaskText);
            haptics.TriggerVibrationTime(0.1f);

            // Show the text box to tell the player where to go to send the letter
            queuedTextboxCoroutine = QueueTextboxDisplay(mailLetterText, readingTime);
            StartCoroutine(queuedTextboxCoroutine);
            // mailboxTriggerCopy = this.gameObject.AddComponent<BoxCollider>();

            // Copy the original trigger's bounds
            // mailboxTriggerCopy.center = mailboxTrigger.center + mailboxTrigger.gameObject.transform.position - this.transform.position;
            // this.transform.rotation = mailboxTrigger.gameObject.transform.rotation;
            // mailboxTriggerCopy.size = Vector3.Scale(mailboxTrigger.size, mailboxTrigger.transform.lossyScale);
            

            
            // mailboxTriggerCopy.isTrigger = true;
            readyToSendMail = true;
            mailboxTrigger.enabled = true;
            mailboxTrigger.transform.SetParent(this.transform);
        }
    }


    /// <summary>
    /// Waits for any current textbox coroutine to finish before displaying a new textbox
    /// for a certain amount of time for the player to read
    /// </summary>
    /// <param name="toDisplay"></param>
    /// <param name="timeToDisplay"></param>
    /// <returns></returns>
    private IEnumerator QueueTextboxDisplay(string toDisplay, float timeToDisplay) {
        yield return null;  // Wait a frame, just in case this frame is displaying a new text message
        // Wait for the current message to end
        while (textBox.textActive) {
            yield return null;
        }
        
        // Display the next message
        textBox.DisplayMessage(toDisplay, timeToDisplay);
    }

    /// <summary>
    /// Displays the letter for the player to send to humans, which
    /// has the button to progress to the final scene.
    /// This function should be called when the user interacts with the mailbox collider, if
    /// the mail is ready to be sent (main tasks complete). To do this, only call this method
    /// if the bool readyToSenMail is true.
    /// </summary>
    private void ShowPlayerLetter() {

        // Activate mail particles
        mailParticleEmmiter.rateOverTime = 2;

        // Stop any future text from showing (may be redundant)
        if (queuedTextboxCoroutine != null) {
            StopCoroutine(queuedTextboxCoroutine);
        }

        // Hide the text box, which might have text showing/queued to show
        textBox.gameObject.SetActive(false);
        readyToSendMail = false;    // Prevent this method from happening again (check this bool before calling the method)

        // Display the letter, activating the send button
        letterForHumans.SetActive(true);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player") && readyToSendMail) {
            ShowPlayerLetter();
        }
    }

    private void OnTriggerStay(Collider other) {
        if(other.CompareTag("Player") && readyToSendMail) {
            ShowPlayerLetter();
        }
    }
        
    /// <summary>
    /// Completes the level by "sending the letter to the humans".
    /// </summary>
    private void SendLetterToHumans() {
        letterForHumans.SetActive(false);
        mainTask.CompleteTask();
        haptics.TriggerVibrationTime(0.1f);
        sceneChanger.LoadNextScene();
    }
}
