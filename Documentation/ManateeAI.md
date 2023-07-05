# Manatee AI
Author: Alex Wills

Location:
- Scripts
  > `Assets/Scripts/ManateeAI/`

## Usage
Drag and drop the manatee prefab into the scene for a fully functional manatee
model that will behave independently in the scene. The scripts are extendable
for creating new actions.

### Editing the manatee's logic
The manatee generally follows this pattern:
- Choose an **action** to take
- Wait until the **action** is complete
- Repeat

The logic for the different actions can be found/modified at
[`Assets/Scripts/ManateeAI/ManateeActions.cs`](./../Dumpling%20Manatee%20Simulation/Assets/Scripts/ManateeAI/ManateeActions.cs), in the different classes.

The logic for how the manatee chooses which action to take can be found 
in [`Assets/Scripts/ManateeAI/ManateeBehavior.cs`](./../Dumpling%20Manatee%20Simulation/Assets/Scripts/ManateeAI/ManateeBehavior.cs),
in the `private void ChooseNextAction()` method.

To add behavior outside of this loop (for example, the manatee playing an animation/action 
when the player pets it with the OnCollisionEnter event), stop the current action with `currentAction.StopAction()` and begin a new action with the following code:
```
currentAction = newAction;
currentAction.StartAction();
```
(without this code, the manatee will default to choosing another action on its own)

### Creating a new manatee action
1. To create a new action, you must implement the `AbstractAction` class with the following methods (you can copy/paste this code block as a template):

    ``` 
    public class NewAction : AbstractAction {

        // If you want, you can add any new variables to the class and the constructor
        public NewAction(ManateeBehavior manatee) : base(manatee) {
            // If you are not adding anything to the constructor, 
            // you can leave this section empty
        }    

        protected override void ForceEnd {
        
            >>> Optionally write any code here to "end" the action and 
                place the manatee in a state where another action may begin <<<

        }

        protected override IEnumerator ActionCoroutine() {
            >>> Put your action code here <<<

            this.OnComplete();
        }
    }    
    ```
    > Notes:
    > - The line `this.OnComplete();` is
    >   necessary for the system to work. `OnComplete()` notifies the manatee that it
    >   should choose its next action.
    > - The existing actions can be found at [`Assets/Scripts/ManateeAI/ManateeActions.cs`](./../Dumpling%20Manatee%20Simulation/Assets/Scripts/ManateeAI/ManateeActions.cs).
    > If you are adding new actions, you can write them in this file or in a separate file.
    > - The constructor can be changed, but it must still take in a `ManateeBehavior`
    >   parameter and pass it into the base constructor (See the `ManateeSwim` action 
    >   for an example with a custom constructor).
    > - If the action should not be interrupted by the player, you do not need to write
    >   any code in the `ForceEnd()` method, but you will need to add
    >   `interruptable = false;` to the constructor (See 
    >   [`ManateeBreathe`](./../Dumpling%20Manatee%20Simulation/Assets/Scripts/ManateeAI/ManateeActions.cs) 
    >   as an example).

<br>

1. Then, add an instance of the new action to the `ManateeBehavior` script:
   > In [`Assets/Scripts/ManateeAI/ManateeBehavior.cs`](./../Dumpling%20Manatee%20Simulation/Assets/Scripts/ManateeAI/ManateeBehavior.cs):
    ```
        ...
        // Make a variable to store the new action
        // This can be of type AbstractAction or the new action type
        private AbstractAction newAction;
        ...
        void Start() {
            ...
            // Initialize the new action
            newAction = new NewAction( this );
            ...
        }
        ...
    ```

2. Finally, add the logic for the action to occur in `private void ChooseNextAction()`
   and assign the new action variable to `currentAction`:
   > Also in [`Assets/Scripts/ManateeAI/ManateeBehavior.cs`](./../Dumpling%20Manatee%20Simulation/Assets/Scripts/ManateeAI/ManateeBehavior.cs):

    ```
        ...
        private void ChooseNextAction() {
            if ( >>> condition for new action <<< ) {
                currentAction = newAction;
            }
            ...

            currentAction.StartAction();
            currentActionActive = true;
        }
        ...
    ```
    > Notes:
    > - This method is called anytime the manatee finishes an action.








It is important to note that this method will only be called when an existing action 
finishes.


## Implementation Details

### Actions
**Actions** are defined in the [`Assets/Scripts/ManateeAI/ManateeActions.cs`](./../Dumpling%20Manatee%20Simulation/Assets/Scripts/ManateeAI/ManateeActions.cs) file, which contains multiple extensions of the [`AbstractAction`](./../Dumpling%20Manatee%20Simulation/Assets/Scripts/ManateeAI/AbstractAction.cs) class.

When you construct an `AbstractAction` with the `ManateeBehavior` parameter, it
gives the **action** object access to the manatee's rigidbody, animator, and `ManateeBehavior` script.

The **action** also stores the action when it runs as a coroutine (IEnumerator), although it uses the `ManateeBehavior` script to run the coroutine with `manatee.Start/StopCoroutine();` 
(the action script is *not* a MonoBehavior script).

`action.StartAction()` is called in the `ManateeBehavior` script to begin the coroutine.
The coroutine is stored in a variable to make it possible to end the coroutine early
if needed.

`action.OnComplete()` is called internally at the end of the `ActionCoroutine()` and `StopAction()` methods to prepare the manatee for its next action using `manatee.EndCurrentAction()`.

`action.StopAction()` is called externally in the `ManateeBehavior` script to end a 
coroutine early, such as when the manatee is pet by the player. Unless
the `interruptable` variable is set to `false` in the constructor, this function ends the
coroutine, calls `action.ForceEnd`, and then calls `action.OnComplete()`.
If `interruptable` is set to `false`, this function will not stop the action.
The function returns `true` if it stopped the action, and `false` otherwise.

`action.ForceEnd()` is called by `StopAction()` to end the action early. This must
be implemented by the different actions. As an example, in `ManateeSwim`, this method
adds drag to the rigidbody and turns the `"isSwimming"` boolean on the animator to false,
as it would if the action coroutine finished normally. 

`action.ActionCoroutine()` is called internally by `action.StartAction()`, and this is 
where the action is defined. The return type `IEnumerator` is used by coroutines in Unity
to allow a function to happen across multiple frames with the `yield return ___` 
statement. This is the main function that is overridden by the different action classes.

### Manatee
The manatee's behavior can be found in [`Assets/Scripts/ManateeAI/ManateeBehavior.cs`](./../Dumpling%20Manatee%20Simulation/Assets/Scripts/ManateeAI/ManateeBehavior.cs).

This is the algorithm the manatee script uses to determine which action to take:

\>\>\> Insert algorithm, or a flow chart picture, here.


Detecting the end of an action:
> The `void Update()` method checks the manatee's `currentActionActive` bool,
> and if it is false, it will call `void ChooseNextAction()`.
>
> Once an action is chosen, `currentActionActive` is set to true to prevent another
> action from being chosen. 
>
> When the action finishes and calls `AbstractAction.OnComplete()`, it triggers
>  `ManateeAI.EndCurrentAction()`, which sets `currentActionActive` back to false.

Interacting with the manatee:
> When the player collides with the manatee, the script will attempt to stop the
> current action with `currentAction.StopAction()`. If the action is set to not
> be interrupted, this method will do nothing and return false, and the player will
> not interact with the manatee. Otherwise, the action will be stopped, and we can
> set/start the current action as the **play** action.