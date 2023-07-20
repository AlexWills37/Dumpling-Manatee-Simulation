using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ManateeHeavenGameController : MonoBehaviour
{
    [SerializeField]
    private int numberOfGrassNeededToContinue;
    private int numberOfGrassEaten = 0;
    private bool breathed = false;
    private bool interacted = false;
    private PlayerManager player; //hang onto a reference to the player

    [SerializeField] private TaskBar grassTask;
    [SerializeField] private TaskBar interactionTask;
    [SerializeField] private TaskBar breatheTask;
    // These following bools should be replaced by adding a way to get the current status of the task bar
    private bool breatheTaskCompleted = false;
    private bool interactionTaskCompleted = false;  

    [SerializeField] private TaskBar goToSchoolTask;

    /// <summary>
    /// At the start, get a reference to the player script and gall its getPlayerValuesEvent() method to get a reference to the event that scrip ttriggers every time is values are updated. add a listener that will be triggered when that event is invoked
    /// </summary>
    private void Start()
    { 
        player = GameObject.Find("Player").GetComponent<PlayerManager>();
        player.getPlayerValuesEvent().AddListener(checkPlayerValues);
        goToSchoolTask.gameObject.SetActive(false);
        interacted = true;
    }
    /// <summary>
    /// simple method to update this scripts information from what the player script stores. This setup was chosen rather than simplyy updating these values every frame to save checks of these variables so they are only used when the variables update
    /// </summary>
    private void checkPlayerValues()
    {
        //Debug.Log("Player value Updates Received by game controller");
        this.numberOfGrassEaten = player.ateGrassNum;
        this.breathed = player.breathed;
        checkIflevelComplete();

        UpdateTasks();
    }

    /// <summary>
    /// If the win condition of the level is complete, access the LevelExitVolume and turn it on so that when the player enters it they p[roceed to the next scene
    /// </summary>
    void checkIflevelComplete()
    {
        if (numberOfGrassEaten >= numberOfGrassNeededToContinue & breathed 
            & interacted
            )
        {
            goToSchoolTask.gameObject.SetActive(true);
            GameObject.Find("LevelExitVolume").GetComponent<LevelExitBehav>().levelComplete();
        }
    }

    private void UpdateTasks() {
        // Update the seagrass task
        grassTask.ChangeTask("Eat seagrass (" + numberOfGrassEaten + " / " + numberOfGrassNeededToContinue + ")");
        if (numberOfGrassEaten == numberOfGrassNeededToContinue) {
            grassTask.CompleteTask();
        }

        // Update the breathing task
        if (breathed && !breatheTaskCompleted) {
            breatheTaskCompleted = true;
            breatheTask.CompleteTask();
        }

        // Update the interaction task
        if (interacted && !interactionTaskCompleted) {
            interactionTaskCompleted = true;
            interactionTask.CompleteTask();
        }
    }
}
