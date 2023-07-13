using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Manatee script for nametag behavior. This script connects to the ManateeNameChooser
/// to set the manatee's name to the names chosen by the player.
/// It also forces the nametag to face the player so that it is always readable.
/// Customize the settings in the inspector.
/// 
/// @author Alex Wills
/// @date 7/12/2023
/// </summary>
public class ManateeNametag : MonoBehaviour
{

    [Tooltip("Whether the name is chosen by the player in the Manatee Name Chooser script")]
    [SerializeField] private bool usePlayerChosenName = false;

    [Tooltip("If usePlayerChosenName is true, which index should the manatee choose its name from?")]
    [SerializeField] private int playerChosenNameIndex = 0;

    [Tooltip("If usePlayerChosenName is false, what name should the nametag display?")]
    [SerializeField] private string setName = "Manatee";

    [Tooltip("The object to rotate so that the nametag faces the player")]
    [SerializeField] private Transform nametagTransform;

    private Transform playerCamera; // The transform for this nametag to face for the player to read


    // Start is called before the first frame update
    void Start()
    {

        // Find the player's position with the OVR Camera Rig
        GameObject eyeAnchor = GameObject.Find("CenterEyeAnchor");
        if (eyeAnchor == null) {
            Debug.LogError(this.name + " could not find the Player's 'CenterEyeAnchor' object for the nametag to face.");
        }
        playerCamera = eyeAnchor.transform;

        // Find the canvas, which should have the nameplate
        Canvas canvas = nametagTransform.gameObject.GetComponentInChildren<Canvas>();
        if (canvas == null) {
            Debug.LogError(this.name + " has the ManateeNametag script but could not find the nametag. " + nametagTransform.name + " should have a canvas " +
                "with a text object for the nametag, as a child Game Object.");
        }

        // Get the name with the text object
        TextMeshProUGUI nameText = canvas.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        if (nameText == null) {
            Debug.LogError(this.name + " has the ManateeNametag script, but could not find the TextMeshPro object under the nametag's canvas.");
        }

        // Change the name if required
        if (usePlayerChosenName) {
            if (ManateeNameChooser.chosenNames == null) {
                Debug.LogError(this.name + " cannot access the list of manatee names, since the ManateeNameChooser script has not yet run.");
            }
            else if (playerChosenNameIndex < 0 || playerChosenNameIndex >= ManateeNameChooser.chosenNames.Length) {
                Debug.Log(this.name + " has the ManateeNametag script, but the name's chosen index (" + playerChosenNameIndex + ")");
            } else {
                setName = ManateeNameChooser.chosenNames[playerChosenNameIndex];
            }
        }

        nameText.SetText(setName);  // Update the nametag
    }

    // Update is called once per frame
    void Update()
    {
        // Always face the player
        nametagTransform.LookAt(playerCamera, Vector3.up);
    }
}
