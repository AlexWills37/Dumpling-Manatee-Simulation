using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Inserts a manatee's name, as chosen by the player, into the text.
/// @author Alex Wills
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class InsertManateeName : MonoBehaviour
{
    [Tooltip("The index of the name to insert (0 or 1)")]
    [SerializeField] private int chosenNameIndex; 

    [Tooltip("The string in the textbox to replace with the manatee's name")]
    [SerializeField] private string namePlaceholder = "[manatee name]";

    // Start is called before the first frame update
    void Start()
    {
        TextMeshProUGUI text = this.GetComponent<TextMeshProUGUI>();

        string manateeName = "Noodle";  // Default name, if something goes wrong

        // Try to access the name chosen by the player
        if (ManateeNameChooser.chosenNames == null) {
            Debug.LogError(this.name + " cannot access the list of manatee names, since the ManateeNameChooser script has not yet run.");
        }
        else if (chosenNameIndex < 0 || chosenNameIndex >= ManateeNameChooser.chosenNames.Length) {
            Debug.Log(this.name + " has the ManateeNametag script, but the name's chosen index (" + chosenNameIndex + ")");
        } else {
            manateeName = ManateeNameChooser.chosenNames[chosenNameIndex];
        }

        // Replace the placeholder text in the TMPro object with the manatee's name
        text.SetText( text.text.Replace(namePlaceholder, manateeName));
    }
}
