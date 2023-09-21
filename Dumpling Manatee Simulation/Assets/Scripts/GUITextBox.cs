using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GUITextBox : MonoBehaviour
{

    private TextMeshProUGUI text;

    private IEnumerator currentMessageCoroutine = null;

    private CanvasRenderer textbox;

    private float activeTransparency;

    public bool textActive {private set; get;}

    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponentInChildren<TextMeshProUGUI>();
        text.SetText("");  
        textActive = false; 

        textbox = this.GetComponent<CanvasRenderer>();
        activeTransparency = textbox.GetAlpha();
        textbox.SetAlpha(0f);

    }


    /// <summary>
    /// Sets the text box to show a new string for a certain amount of time before deactivating.
    /// If the text box is already displaying a message, it will be replaced.
    /// </summary>
    /// <param name="message"> The message to display </param>
    /// <param name="time"> The time (in seconds) to show the message before disappearing </param>
    public void DisplayMessage(string message, float time) {
        
        // Interrupt the current coroutine to stop the previous timer, if it exists
        if (currentMessageCoroutine != null) {
            StopCoroutine(currentMessageCoroutine);
        }

        // Start the new coroutine
        currentMessageCoroutine = MessageCoroutine(message, time);
        StartCoroutine(currentMessageCoroutine);
    }



    /// <summary>
    /// Coroutine function to handle the logic and timing of displaying a message.
    /// Updates the Text Mesh Pro object, displays the textbox, waits for time to pass,
    /// and hides the textbox again.
    /// </summary>
    /// <param name="message"> The message to display </param>
    /// <param name="time"> The time (in seconds) to show the message for </param>
    /// <returns></returns>
    private IEnumerator MessageCoroutine(string message, float time) {
        // Set the text in the text box
        text.SetText(message);

        // Show the textbox if it is currently invisible
        textbox.SetAlpha(activeTransparency);
        textActive = true;

        // Wait for the player to read the text
        yield return new WaitForSeconds(time);

        // Hide the textbox
        textbox.SetAlpha(0f);
        text.SetText("");
        textActive = false;

        // Finish the coroutine
        currentMessageCoroutine = null;
    }
}
