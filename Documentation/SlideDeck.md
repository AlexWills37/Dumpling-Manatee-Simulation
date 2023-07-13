# Slide Deck (Presentation)
Author
> Alex Wills

Location
> Insert location

This script/system is used for creating presentations that the user
can progress at their own pace.

It works by setting a list of game objects in the inspector, and then
only having one object enabled at a time. When the user clicks a button
to move to the next slide, it disables the current object, enables the next one,
and calls any methods tied to the slide.

## Usage

1. Add the `SlideDeck.cs` script to any game object.
2. Set up a canvas with a button that that the player can click.
3. In the inspector for the **Slide Deck**, drag and drop the **Button** into the
   *Next Slide Button* field.
4. Create **slides**.
   > Slides are really just groups of game objects with a single parent game object.
   >
   > Slides should be separated into separate game objects. These game objects
   > can have multiple children, as long as there is exactly 1 game object per
   > slide that can be enabled/disabled to enable/disable the slide.
   >
   > Example:
   ```
   Presentation (with SlideDeck component)
   |> Next Slide Button (with Button component)
   |> Slide Background
   |  |> Any elements/objects that will be on all Slides
   |> Slides
   |  |> Slide 0
   |  |  |> Title text
   |  |  |> Picture
   |  |  |> Text box
   |  |> Slide 1
   |  |> Slide 2 
   ```
5. In the inspector for the **Slide Deck**, drag and drop each **slide** into the *Slides*
   list. The list can be as long or as short as needed.

### Controlling the Next Slide Button
By default, when the player clicks the button, the script will move to the next slide
and disable the button for a small amount of time to prevent the user from skipping
through slides too quickly.

You may wish to stop the user from moving to the next slide until they complete a certain
action (like choosing their manatee name). To do this, you can write a script that
calls the **Slide Deck**'s `void SetButtonActive(bool active)` method, which will
make it so the player is unable to click the button until it is reactivated.

In a custom script:

1. Get a reference to the **Slide Deck** component.
   > This implementation depends on the scene setup. Here are a couple of
   > possible implementations:
   > - `GameObject.FindObjectOfType<SlideDeck>()` if there is only 1 **Slide Deck** in the scene
   > - Add a `[SerializeField] private SlideDeck slideDeck;` to the script, then drag and
   >   drop the **Slide Deck** in the inspector
   > - `GameObject.Find(name).GetComponent<SlideDeck>()` if you know the game object's name
   
2. Call `slideDeck.SetButtonActive(false)` to make the button unclickable, so that the player
   is unable to progress to the next slide.

   > To change the text displayed in the button to something other than the default,
   > call `slideDeck.SetButtonText(string text)` *after* calling `slideDeck.SetButtonActive(bool active)`.


3. a) When ready to move to the next slide, call `slideDeck.SetButtonActive(true)` to 
   allow the player to move to the next slide.

   b) Alternatively, to automatically go to the next slide, call 
   `slideDeck.OnButtonClick()`, which will move to the next slide. The button will
   automatically reenable itself after some time passes, like it would when normally
   switching slides.

### Calling functions when reaching a certain slide
You may wish to activate a script's function when a certain slide is reached (like
calling `slideDeck.SetButtonActive(false)` when reaching the slide where the player
chooses their name). 

There are two ways to call functions when a certain slide is reached, and it depends on
the context.

**If the script is attached to the slide or an object in the slide:**

Call the code in the script's `private void OnEnable()` function.

Since the `SlideDeck.cs` script disables all of the slide objects in the `Awake()` function, 
*any scripts attached to the slides (other than the first slide) will be disabled before they can run*
This means that both `Start()` and `OnEnable()` will only run when the slide is activated.

> **SlideDeck.cs** is currently set in the Project Settings to have an earlier execution time
> than other scripts. This means that even the other script's `Awake()` functions will
> be disabled before they can run. To change this:
> 
> Go to *Edit > Project Settings > Script Execution Order*. *SlideDeck* should be listed
> before *Default Time*, or before the script attached to the slide.



**If the script is not attached to the slide (it will not be disabled/enabled):**

Subscribe a function to a certain slide with the **Slide Deck**'s `void AddEventOnSlideActivate(int slideIndex, UnityAction call)` or `void AddEventOnFinalSlide(UnityAction call)`.

This may be useful if you have an external Game Manager script that should activate
some function on a specific slide.

In the custom script that is not attached to a slide:

1. Write a function with no parameters to call when we reach a certain slide
   (for a function *with* parameters, see [this section below](#subscribing-a-function-with-parameters-to-a-unityevent)).
2. Get a reference to the **Slide Deck** component (see the previous section 
   for possible implementations).
3. In the script's `void Start()` method, call 
   `slideDeck.AddEventOnSlideActivate(int slideIndex, UnityAction call)`
   > `slideIndex` corresponds to the index in the **Slide Deck**'s *Slides* list.
   > The method will be called when the specified slide is loaded.
   >
   > Slide 0 will not call any methods, since that slide is loaded when the scene
   > starts.
   >
   > `call` is the name of the function to call when the slide is reached.
   >
   > The function should not have any parameters, and in this method, it does not
   > need any parentheses (see the example below).

Example:
```
public class CoolScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SlideDeck slides = GameObject.Find("Presentation").GetComponent<SlideDeck>();
        slides.AddEventOnSlideActivate(2, PrintMessage);    
    }

    private void PrintMessage() {
        Debug.Log("We made it to the third slide!");
    }
}
```
> In this example, when slide 2 is loaded (the third slide), it will call 
> `PrintMessage()`, which prints a message to the console.
>
> A script can add multiple functions to a single slide, or different functions
> for different slides, or the same function for different slides.
>

#### Subscribing a function with parameters to a UnityEvent
Answers taken from [here on the Unity Forums](https://discussions.unity.com/t/button-onclick-addlistener-how-to-pass-parameter-or-get-which-button-was-clicked-in-handler-method/179151/2).

If you have a function that requires parameters, such as the following code from
line 82 of [ManateeNameChooser.cs]:
```
   private void ChooseName(string nameToChoose) {
      chosenNames[currentManatee] = nameToChoose;
      ...
   }
```
> In this case, each button adds this method to its onClick event, with the
> text displayed on the button as the parameter, so each button passes a different
> name to this function.

you can use a [lambda (anonymous) function](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions) that doesn't have any parameters.
> These are functions that do not have any name, and are defined explicitly
>
> `(input-parameters) => { <sequence-of-statements> }`

With the example from the ManateeNameChooser script, a separate lambda function can be 
subscribed to each button to call the ChooseName method with a different string ([starting at line 56 of ManateeNameChooser.cs]):

```
   for (int i = 0; i < nameChoiceButtons.Length; i++) {
      TextMeshProUGUI buttonText;
      buttonText = nameChoiceButtons[i].GetComponentInChildren<TextMeshProUGUI>();
      
      ... // Code to validate the buttonText has been omitted

      // When the button is clicked, call the ChooseName method with the button's text
      nameChoiceButtons[i].onClick.AddListener( () => { ChooseName(buttonText.text); } );
   }
```
> Here, each button adds the **UnityAction** `() => { ChooseName(buttonText.text); }`,
> which is a lambda function without parameters that calls the `ChooseName` method with
> the button's unique text.
>
> This same process can be used for any kind of **UnityEvent**, including the events
> in this **Slide Deck** script.