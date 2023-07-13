using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the UI and backend interaction for allowing the player to choose the names of the game's manatees.
/// 
/// @author Alex Wills
/// @date 7/12/2023
/// </summary>
public class ManateeNameChooser : MonoBehaviour
{

    // The player's chosen names
    public static string[] chosenNames { get; private set; }

    [Tooltip("Buttons for the manatee names. The button must have a TMPro with the name to choose")]
    [SerializeField] private Button[] nameChoiceButtons;


    [Tooltip("The text fields to display the currently selected names")]
    [SerializeField] private TextMeshProUGUI[] chosenNameFields;

    [Tooltip("Slide deck to pause to wait for the player to choose names")]
    [SerializeField] private SlideDeck slideDeck = null;

    // The arrow icon that appears next to whichever name is currently being selected
    private GameObject[] chosenNameSelectors;   // All of the chosenNameFields should have a child containing an Image component for the arrow

    private int currentManatee = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the static list of chosen names
        chosenNames = new string[chosenNameFields.Length];

        // Get the selector images for each chosenNameField
        chosenNameSelectors = new GameObject[chosenNameFields.Length];
        for (int i = 0; i < chosenNameSelectors.Length; i++) {
            // Verify that the fields are setup correctly in the hierarchy
            if ((chosenNameFields[i].transform.childCount == 0 ) ||
                (chosenNameFields[i].transform.GetChild(0).gameObject.GetComponent<Image>() == null)) {
                Debug.LogError(chosenNameFields[i].name + " is set as a name field for the name selector, but is not set up properly.");
                Debug.LogError(chosenNameFields[i].name + " should have a child object with an Image component to show when this field is being chosen by the player.");
            } else {
                // Add the selector to the array and set it to be inactive
                chosenNameSelectors[i] = chosenNameFields[i].transform.GetChild(0).gameObject;
                chosenNameSelectors[i].SetActive(i == 0);   // Keep the first selector active, and the rest inactive
            }
        }

        // Connect the buttons with this script to choose names
        for (int i = 0; i < nameChoiceButtons.Length; i++) {
            TextMeshProUGUI buttonText;
            buttonText = nameChoiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText == null) {
                Debug.LogError(nameChoiceButtons[i].name + " is set as a button for choosing a manatee name, but it does not have a Text Mesh Pro component in its child objects.");
            } else if (buttonText.text == "") {
                Debug.LogError(nameChoiceButtons[i].name + " has an empty string as its button text. The text should be the player's choice for a manatee name.");
            } else {
                // When the button is clicked, call the ChooseName method with the button's text
                nameChoiceButtons[i].onClick.AddListener( () => { ChooseName(buttonText.text); } );
            }
        }
    }

    /// <summary>
    /// When this script is enabled by the SlideDeck, pause the SlideDeck to wait for the player to choose names
    /// </summary>
    private void OnEnable() {
        if (slideDeck != null) {
            Debug.LogWarning("Enabling name chooser!");
            slideDeck.SetButtonActive(false);
            slideDeck.SetButtonText("Click the names you want");
        }
    }


    private void ChooseName(string nameToChoose) {
        // Add the current name to the list of selections
        chosenNames[currentManatee] = nameToChoose;

        // Update the text to reflect this choice
        chosenNameFields[currentManatee].SetText("Manatee " + (currentManatee + 1) + ": " + nameToChoose);

        // Move to the next manatee to name
        chosenNameSelectors[currentManatee].SetActive(false);
        currentManatee = (currentManatee + 1) % chosenNames.Length;
        chosenNameSelectors[currentManatee].SetActive(true);

        // Unlock the slide deck if the player has chosen all of their names
        if (slideDeck != null && currentManatee == 0 && chosenNames[0] != "") {
            slideDeck.SetButtonActive(true);
        }
    }
}
