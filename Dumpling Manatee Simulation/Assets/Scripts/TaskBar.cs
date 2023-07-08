using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


/// <summary>
/// A bar to display a checkbox and a task.
/// When a task is completed or reset, the text will go through a color gradient to indicate the status.
/// The text will remain the color at the end of the gradient until it is Completed/Reset again.
/// 
/// USAGE:
///     By default, the task will appear as incopmlete.
///     call CompleteTask() to visually complete the task. 
///     call ResetTask() to visually reset the task.
///     call TransitionTask(string newTask) to visually complete the current task, update the text, and reset the task.
///     call ChangeTask(string newTask) to change the task's text without changing the color or checkmark.
/// 
/// @author Alex Wills
/// @date 7/8/2023
/// </summary>
public class TaskBar : MonoBehaviour
{

    [Tooltip("The Text Mesh Pro to display the current task")]
    [SerializeField] private TextMeshProUGUI taskText;

    [Tooltip("The checkmark image to hide/show based on the task status")]
    [SerializeField] private GameObject checkmark;

    [Tooltip("The colors the text will transition through upon completion")]
    [SerializeField] private Gradient completionGradient;

    [Tooltip("Time (seconds) to transition through the gradient above")]
    [SerializeField] private float completionTime = 1f;

    [Tooltip("The colors that the text will transition through upon resetting the task")]
    [SerializeField] private Gradient resetGradient;

    [Tooltip("Time (seconds) to transition through the gradient above")]
    [SerializeField] private float resetTime = 0.5f;

    private IEnumerator coroutine;  // Store the current color transition coroutine to detect when it is finished


    // Start is called before the first frame update
    void Start()
    {
        checkmark.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) {
            TransitionTask("Super cool new task.");
        }
    }

    /// <summary>
    /// Sets the task to complete, playing a small animation.
    /// </summary>
    public void CompleteTask() {
        if (coroutine != null) {
            StopCoroutine(coroutine);
        }
        coroutine = TransitionColor(completionGradient, completionTime);
        StartCoroutine(coroutine);
        checkmark.SetActive(true);
    }

    /// <summary>
    /// Sets the task to incomplete, ready to be checked off again.
    /// </summary>
    public void ResetTask() {
        if (coroutine != null) {
            StopCoroutine(coroutine);
        }
        coroutine = TransitionColor(resetGradient, resetTime);
        StartCoroutine(coroutine);
        checkmark.SetActive(false);
    }


    /// <summary>
    /// Updates the task with different text.
    /// </summary>
    /// <param name="newTask"> The new text to display for this task </param>
    public void ChangeTask(string newTask) {
        taskText.SetText(newTask);
    }


    /// <summary>
    /// Completes the current task and resets the checkmark to
    /// display the next task.
    /// </summary>
    /// <param name="newTask"></param>
    public void TransitionTask(string newTask) {
        StartCoroutine(TransitionTaskCoroutine(newTask));
    }

    /// <summary>
    /// Creates a coroutine that changes the text's color through a gradient over time.
    /// </summary>
    /// <param name="transition"> the colors to transition through </param>
    /// <param name="transitionTime"> time (seconds) to go through the gradient </param>
    /// <returns> IEnumerator representation of the coroutine </returns>
    private IEnumerator TransitionColor(Gradient transition, float transitionTime) {
        
        // Change the color across the gradient over time
        for (float time = 0; time < 1; time += (Time.deltaTime / transitionTime)) {
            taskText.color = transition.Evaluate(time);
            yield return null;
        }

        // Set the text color to the final color
        taskText.color = transition.Evaluate(1);
        coroutine = null;   // Coroutine is finished, so set it to null
    }

    /// <summary>
    /// Creates a coroutine that transitions from one task to another.
    /// (Calls this.CompleteTask(), then this.ChangeTask(), then this.ResetTask())
    /// </summary>
    /// <param name="newTask"> the new task to display </param>
    /// <returns> IEnumerator representation of the coroutine </returns>
    private IEnumerator TransitionTaskCoroutine(string newTask) {
        // Copmlete the current task
        this.CompleteTask();

        // Wait for the animation to finish
        while (coroutine != null) {
            yield return null;
        }
        
        // Change the task
        this.ChangeTask(newTask);

        // Reset the task
        this.ResetTask();
    }
}
