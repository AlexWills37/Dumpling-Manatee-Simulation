using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Defines the behavior for the button that makes a manatee sound.
/// When the button is clicked, it will play the manatee sound.
/// 
/// This button is in the first scene, on the boat, during the information presentation.
/// When the slide with this button is reached, it will stop the slide show from continuing
/// until the button is pressed.
/// 
/// @author Alex Wills 
/// @date 7/5/2023
/// </summary>

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(AudioSource))]
public class ManateeSoundButton : MonoBehaviour
{
    [Tooltip("The slide deck this button is a part of")]
    [SerializeField] private SlideDeck slideDeck;

    private Button soundButton; // The button to start the sound

    private AudioSource manateeSound;   // The sound to play

    // Start is called before the first frame update
    void Start()
    {
        // Get the button and connect it to this script
        soundButton = this.GetComponent<Button>();
        soundButton.onClick.AddListener(StartSound);

        // Get the manatee sound
        manateeSound = this.GetComponent<AudioSource>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
            soundButton.onClick.Invoke();
        }
    }

    /// <summary>
    /// Starts the coroutine to play the manatee sound.
    /// </summary>
    private void StartSound() {
        StartCoroutine(PlayButtonSound());
    }

    /// <summary>
    /// Plays the manatee sound. While the sound is playing, the button
    /// will be disabled to prevent the player from playing the sound multiple
    /// times at once. When the sound is finished, the button will be reenabled,
    /// and the slide deck control will be returned to the player.
    /// </summary>
    /// <returns> This coroutine as an IEnumerator </returns>
    private IEnumerator PlayButtonSound() {
        // Disable the button until the sound is finished
        soundButton.interactable = false;
        TextMeshProUGUI buttonText = soundButton.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        string originalText = buttonText.text;
        buttonText.SetText("Playing sound...");

        // Play sound and wait until it is finished
        manateeSound.Play();

        while (manateeSound.isPlaying) {
            yield return null;
        } // Audio has finished playing

        // Reenable the button and allow the player to go to the next slide
        soundButton.interactable = true;
        buttonText.SetText(originalText);
        slideDeck.SetButtonActive(true);
    }


    /// <summary>
    /// When this slide is reached, this script will be enabled and this function will be called.
    /// Takes slide deck control away from the player and directs them to this button
    /// to play the sound. The player will be able to continue the slide deck after
    /// clicking the button and calling the PlayButtonSound() coroutine.
    /// </summary>
    private void OnEnable() {
        slideDeck.SetButtonActive(false);
        slideDeck.SetButtonText("<---");
    }
}
