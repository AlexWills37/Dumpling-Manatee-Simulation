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

### Next Slide Behavior
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

Since the `SlideDeck.cs` script is configured in the project settings to run before
most other scripts, *any scripts attached to the slides (other than the first slide) will be disabled before they can run*. This includes the scripts' `Start()` and `OnEnable()` functions.

This means that both `Start()` and `OnEnable()` will only run when the slide is activated.

> If the scripts are running when the scene is loaded, before the **Slide Deck** disables the slides,
> check to verify that `SlideDeck.cs` has an earlier execution time.
>
> Go to *Edit > Project Settings > Script Execution Order*. *SlideDeck* should be listed
> before *Default Time*, or before the script attached to the slide.



**If the script is not attached to the slide (it will not be disabled/enabled):**

Subscribe a function to a certain slide with the **Slide Deck**'s `void AddEventOnSlideActivate(int slideIndex, UnityAction call)`.

This may be useful if you have an external Game Manager script that should activate
some function on a specific slide.

In the custom script that is not attached to a slide:

1. Write a function with no parameters to call when we reach a certain slide.
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