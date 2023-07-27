using System.Collections;
using System.Collections.Generic;
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

    [Tooltip("Textbox to display when the player has eaten enough seagrass, to explain how there isn't enough")]
    [SerializeField] private GameObject notEnoughGrassTextbox;


    [Header("Interact with manatee task")]
    [Tooltip("Text to display for the 'interact with manatees' task")]
    [SerializeField] private string manateeInteractionTaskText = "Check in on your manatee friends";

    [Tooltip("Text box explaining what happened to the starving manatee friend")]
    [SerializeField] private GameObject manateeImpactInfo;
    [SerializeField] private GameObject manateeImpactInfo2;

    [Header("Mail to humans task")]
    [Tooltip("Text to display for the 'mail letter to humans' task")]
    [SerializeField] private string mailLetterTaskText = "Send a message to humans for help";

    [Tooltip("Textbox to tell the player to go to the mailbox")]
    [SerializeField] private GameObject mailLetterTextBox;

    [Tooltip("Trigger collider to send mail to the player")]
    [SerializeField] private BoxCollider mailboxTrigger;

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

    private bool readyToSendMail = false;

    private ChangeScene sceneChanger;
    

    // Start is called before the first frame update
    void Start()
    {
        // Set up the tasks
        mainTask.ChangeTask(seagrassTaskText + " (0 / " + displayedGrassRequirement + ")");
        secondaryTask.ChangeTask(manateeInteractionTaskText);
        manateeImpactInfo.SetActive(false);
        manateeImpactInfo2.SetActive(false);
        notEnoughGrassTextbox.SetActive(false);
        mailLetterTextBox.SetActive(false);
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

        Button sendLetterButton = letterForHumans.GetComponentInChildren<Button>();
        sendLetterButton.onClick.AddListener(SendLetterToHumans);

        Debug.Log("Game manager successfully initialized.");

        haptics = HapticFeedback.singleton;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P)) {
            Debug.Log("P");
            StartCoroutine(QueueTextboxDisplay(manateeImpactInfo, 5f));

        }

        if (Input.GetKeyDown(KeyCode.L)) {
            StartCoroutine(QueueTextboxDisplay(manateeImpactInfo2, 4f));
        }

    }

    private void UpdateSeagrassTask() {
        mainTask.ChangeTask(seagrassTaskText + " (" + numSeagrassEaten + " / " + displayedGrassRequirement + ")");
        if (numSeagrassEaten == actualGrassRequirement) {
            // mainTask.CompleteTask();
            StartCoroutine(QueueTextboxDisplay(notEnoughGrassTextbox, readingTime));
            CheckFirstTasks();
        }
    }


    /// <summary>
    /// Runs a coroutine to display manatee information.
    /// Displays the manatee impact info (game object set in the inspector)
    /// for the set "Reading Time" (set in inspector), and checks off the
    /// secondary task for learning about manatees and checking on your friends.
    /// </summary>
    /// <returns> IEnumerator containing this coroutine </returns>
    private IEnumerator DisplayManateeImpactInfo() {
        manateeImpactInfo.SetActive(true);
        yield return new WaitForSecondsRealtime(readingTime);

        // Complete reading task
        if (!learnedAboutManateeImpact) {
            secondaryTask.CompleteTask();
            haptics.TriggerVibrationTime(0.1f);
            learnedAboutManateeImpact = true;
            CheckFirstTasks();
        }

        manateeImpactInfo.SetActive(false);
    }

    private void CheckFirstTasks() {
        // NOTE: the seagrass text will not technically be "completed", since there will not be enough seagrass for the player to eat
        // If both tasks are complete, move on to the final task
        if (learnedAboutManateeImpact && numSeagrassEaten >= actualGrassRequirement) {
            // Remove the secondary task and change to a single new task
            secondaryTask.gameObject.SetActive(false);
            mainTask.TransitionTask(mailLetterTaskText);
            haptics.TriggerVibrationTime(0.1f);

            // Show the text box to tell the player where to go to send the letter
            StartCoroutine(QueueTextboxDisplay(mailLetterTextBox, readingTime));
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
    private IEnumerator QueueTextboxDisplay(GameObject toDisplay, float timeToDisplay) {
        
        // Ensure only 1 coroutine runs per game object
        if (coroutineTextboxes.Contains(toDisplay)) {
            // Do nothing
        } else {
            // Add the game object to display to the queue as a marker
            coroutineTextboxes.Enqueue(toDisplay);

            // Wait for the queue to reach this game object
            while (coroutineTextboxes.Peek() != toDisplay) {
                yield return null;
            }

            // Now that the next object is at the front of the queue, display it. We will not remove it from the queue until it has finished showing
            toDisplay.SetActive(true);
            yield return new WaitForSecondsRealtime(timeToDisplay);

            // The text is done showing, we can now remove it from the queue to let the next item display 
            toDisplay.SetActive(false);
            coroutineTextboxes.Dequeue();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player") && readyToSendMail) {
            readyToSendMail = false;
            Debug.LogError("Trigger entered by player!");
            letterForHumans.SetActive(true);
        }
    }
        
    private void SendLetterToHumans() {
        letterForHumans.SetActive(false);
        mainTask.CompleteTask();
        haptics.TriggerVibrationTime(0.1f);
        sceneChanger.LoadNextScene();
    }
}
