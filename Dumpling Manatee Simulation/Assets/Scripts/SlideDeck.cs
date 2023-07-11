using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Defines the behavior for a button that controls a slide show.
/// When the player clicks the button, it will switch to the next slide.
/// 
/// To trigger a script when a specific slide is activated, get a reference
/// to this SlideDeck when the script is initialized and use the method
/// `AddEventOnSlideActivate(int index, UnityAction call)` to connect
/// a function to a certain slide. See the documentation for a more detailed
/// explanation.
/// 
/// @author Alex Wills
/// @date 7/1/2023
/// </summary>
public class SlideDeck : MonoBehaviour
{

    [Tooltip("The button that the user clicks to advance the slides")]
    [SerializeField] private Button nextSlideButton;

    [Tooltip("List of game objects representing the slides")]
    [SerializeField] private GameObject[] slides;

    [Tooltip("Called after the final slide is complete")]
    public UnityEvent OnPresentationComplete;
    
    private TextMeshProUGUI buttonText;
    private string defaultActiveText;   // The button's default text will be whatever is in the text when the scene is loaded.
    
    [Tooltip("Text to display on the button when it is inactive")]
    [SerializeField] private string defaultInactiveText = "...";

    private UnityEvent[] slideActivationEvents;

    [SerializeField][Tooltip("does the slide show go back to the start after it is completed")]
    private bool slideShowRepeats;


    private IEnumerator reactivationTimer;  // Coroutine to temporarily disable the button


    private int currentSlide = 0;

    // Start is called before the first frame update
    void Start()
    {

        // Create a unity event for each slide
        slideActivationEvents = new UnityEvent[slides.Length];
        for (int i = 0; i < slideActivationEvents.Length; i++) {
            slideActivationEvents[i] = new UnityEvent();
        }

        // Connect to the button for transitioning slides
        if (nextSlideButton == null) {
            Debug.LogWarning(this.gameObject.name + " has a slide deck, but no button selected in the inspector. Calling OnButtonClick() will create errors.");
        } else {
            // Get the button text, as well as the default text to display when the button is active
            buttonText = nextSlideButton.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            defaultActiveText = buttonText.text;
            nextSlideButton.onClick.AddListener(OnButtonClick);
            nextSlideButton.interactable = true;
        }

        // Initialize the slides
        if (slides.Length == 0) {
            Debug.LogError(this.gameObject.name + " has a slide deck, but no slides are set in the inspector.");
        } else {
            // Deactivate every slide except for the first
            for (int i = 0; i < slides.Length; i++){
                // Make sure the slide exists first
                if (slides[i] != null) {
                    if (i == 0) {
                        slides[i].SetActive(true);
                    } else {
                        slides[i].SetActive(false);
                    }

                } else {
                    Debug.LogError(this.gameObject.name + " has a slide deck, but slide " + i + " is not set in the inspector.");
                }
            }
        } // All slides initialized
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N) && nextSlideButton.interactable)
        {
            nextSlideButton.onClick.Invoke();
        } else if (Input.GetKeyDown(KeyCode.L)) {
            SetButtonActive(!nextSlideButton.interactable);
        }
    }

    /// <summary>
    /// Adds an event to trigger when a certain slide is reached.
    /// Example:
    ///     in another script,
    ///     slideDeck.AddEventOnSlideActiver(3, StartTimer)
    ///     calls the script's method this.StartTimer() when slide 3 is reached.
    /// 
    /// </summary>
    /// <param name="slideIndex"> The index of the slide that will call a function </param>
    /// <param name="call"> The function (without parameters) to call when the slide is activated </param>
    public void AddEventOnSlideActivate(int slideIndex, UnityAction call) {
        if (slideIndex < slides.Length) {
            slideActivationEvents[slideIndex].AddListener(call);
        }
    }

    
    public void SetButtonActive(bool active) {

        // Disable or enable the button
        nextSlideButton.interactable = active;
        buttonText.SetText( (active ? defaultActiveText : defaultInactiveText) );

        // Stop the reactivation coroutine
        if (reactivationTimer != null) {
            StopCoroutine(reactivationTimer);
            reactivationTimer = null;
        }
    }


    /// <summary>
    /// This function is called when the user presses the button attached to this game object.
    /// When the user presses the button, transition to the next slide and briefly disable the button.
    /// </summary>
    public void OnButtonClick() {
        // Deactivate the button
        nextSlideButton.interactable = false;
        buttonText.SetText(defaultInactiveText);

        // Start the timer to reactivate the button
        if (reactivationTimer != null) {
            StopCoroutine(reactivationTimer);
        }
        reactivationTimer = ReactivateButton(3f);
        StartCoroutine(reactivationTimer);

        // Progress to the next slide
        this.NextSlide();
    }

    public void SetButtonText(string text) {
        buttonText.SetText(text);
    }

    public int GetNumSlides() {
        return slides.Length;
    }

    /// <summary>
    /// Moves the presentation to the next slide.
    /// </summary>
    public void NextSlide() {
        // Transition slides if possible
        if(slides[currentSlide] != null)
        {
            slides[currentSlide].SetActive(false);
        }

        // Progress to the next slide, looping to the beginning if we surpass the last slide
        if (slideShowRepeats)
        {
            currentSlide = (currentSlide + 1) % slides.Length;
        }
        else
        {
            currentSlide += 1;
        }
        
        if(slides[currentSlide] != null)
        {
            slides[currentSlide].SetActive(true);
            slideActivationEvents[currentSlide].Invoke();
        }

        // Check if we are at the last slide
        if (currentSlide >= slides.Length - 1)
        {
            OnPresentationComplete.Invoke();
        }

        // If we are at the first slide again, the presentation has been completed
        if (currentSlide == 0) {
            OnPresentationComplete.Invoke();
        }
    }

    /// <summary>
    /// Coroutine to wait a specified amount of time before reactivating the button.
    /// </summary>
    /// <param name="delay"> time (in seconds) to wait before reactivating the button. </param>
    /// <returns></returns>
    private IEnumerator ReactivateButton(float delay)
    {

        yield return new WaitForSeconds(delay);


        // Bring text back and make button interactable again
        nextSlideButton.interactable = true;
        buttonText.SetText(defaultActiveText);

        // Clear the script's coroutine to indicate it has finished
        reactivationTimer = null;
    }
}
