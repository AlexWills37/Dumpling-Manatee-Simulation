using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Interact with manatee task")]
    [Tooltip("Text to display for the 'interact with manatees' task")]
    [SerializeField] private string manateeInteractionTaskText = "Check in on your manatee friends";

    [Tooltip("Text box explaining what happened to the starving manatee friend")]
    [SerializeField] private GameObject manateeImpactInfo;

    [Header("Mail to humans task")]
    [Tooltip("Text to display for the 'mail letter to humans' task")]
    [SerializeField] private string mailLetterTaskText = "Send a message to humans for help";



    private PlayerManager player;
    private int numSeagrassEaten = 0;   // How much seagrass the player has eaten
    private bool learnedAboutManateeImpact = false; // Whether the player has seen the text box about how manatee health has degraded
    private float readingTime = 5f;
    

    // Start is called before the first frame update
    void Start()
    {
        // Set up the tasks
        mainTask.ChangeTask(seagrassTaskText + " (0 / " + displayedGrassRequirement + ")");
        secondaryTask.ChangeTask(manateeInteractionTaskText);
        manateeImpactInfo.SetActive(false);


        player = GameObject.FindObjectOfType<PlayerManager>();

    }

    // Update is called once per frame
    void Update()
    {
        if (numSeagrassEaten != player.ateGrassNum) {
            numSeagrassEaten = player.ateGrassNum;
            UpdateSeagrassTask();
        }
    }

    private void UpdateSeagrassTask() {
        mainTask.ChangeTask(seagrassTaskText + " (" + numSeagrassEaten + " / " + displayedGrassRequirement + ")");
        if (numSeagrassEaten == actualGrassRequirement) {
            // mainTask.CompleteTask();
            CheckFirstTasks();
        }
    }


    private IEnumerator DisplayManateeImpactInfo() {
        manateeImpactInfo.SetActive(true);
        yield return new WaitForSecondsRealtime(readingTime);

        // Complete reading task
        secondaryTask.CompleteTask();
        learnedAboutManateeImpact = true;
        CheckFirstTasks();

        manateeImpactInfo.SetActive(false);
    }

    private void CheckFirstTasks() {
        // NOTE: the seagrass text will not technically be "completed", since there will not be enough seagrass for the player to eat
        // If both tasks are complete, move on to the final task
        if (learnedAboutManateeImpact && numSeagrassEaten >= actualGrassRequirement) {
            // Remove the secondary task and change to a single new task
            secondaryTask.gameObject.SetActive(false);
            mainTask.TransitionTask(mailLetterTaskText);

            // Show the text box to tell the player where to go to send the letter
        }
    }
}
