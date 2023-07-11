using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatSceneManager : MonoBehaviour
{
    [Tooltip("The slide deck that teaches the player about manatees")]
    [SerializeField] private SlideDeck slideDeck;

    [Tooltip("The index of the slide where the player chooses their name")]
    [SerializeField] private int nameChoosingSlideIndex = 4;

    // Start is called before the first frame update
    void Start()
    {
        // Call an event on the final slide to change the button text
        slideDeck.AddEventOnSlideActivate(slideDeck.GetNumSlides() - 1, OnFinalSlide);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void MoveToNextScene() {
        Debug.Log("Boat scene complete!");
    }

    /// <summary>
    /// On the final slide, change the button text to indicate
    /// the move to a new scene.
    /// </summary>
    private void OnFinalSlide() {
        slideDeck.SetButtonActive(true);
        slideDeck.SetButtonText("Become a manatee!");
    }
}
