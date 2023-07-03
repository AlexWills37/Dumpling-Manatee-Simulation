using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DebugText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private string[] messages;
    private int logLength = 5;
    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponent<TextMeshProUGUI>();
        messages = new string[5];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Log(string newMessage) {
        // Shift every message down
        for (int i = logLength - 1; i >= 1; i--) {
            messages[i] = messages[i - 1];
        }

        // Add message to the top
        messages[0] = newMessage;
        this.DisplayMessages();
    }

    public void LogWarning(string newWarning) {
        this.Log("Warning: " + newWarning);
    }

    public void LogError(string newError) {
        this.Log("ERROR: " + newError);
    }

    private void DisplayMessages() {
        string fullText = "";
        for (int i = logLength - 1; i>=0; i--) {
            fullText += messages[i] + "\n";
        }

        text.SetText(fullText);
    }
}
