# Manatee AI
Author: Alex Wills

Location:
- Scripts
  > `Assets/Scripts/ManateeAI/`

## Usage
Drag and drop the manatee prefab into the scene for a fully functional manatee
model that will behave independently in the scene.


## Implementation
The manatee generally follows this pattern:
- Choose an **action** to take
- Wait until the **action** is complete
- Repeat

**Actions** are defined in the `insert filename here` file, which contains implementations
of the `AbstractAction` class. To create an action, you must use the constructor with
the manatee's script as a parameter. This can be seen in the main behaviour script:
```
// The manatee's possible actions
private AbstractAction swim, breathe;
...
void Start() {
    ...
    swim = new ManateeSwim( this );
}
```

This connects the action to the manatee, giving the action access to the manatee's rigidbody and animator.


### Creating a new manatee action
To create a new action, you must implement the `AbstractAction` class with the following methods:

``` 
public class NewAction : AbstractAction {

    public NewAction(ManateeAI manatee) : base(manatee) {}    

    public override void ForceEnd {

    }

    protected override IEnumerator ActionCoroutine() {
        >>> Put your action code here <<<

        // You must include the OnComplete() method so the manatee
        // chooses a next action to take, once this one is finished.
        this.OnComplete();
    }
}    
```



Then, in the main script, you need a way to call the action.
First, store the action in a member variable and initialize it in the `void Start()` method:
```
// The manatee's possible actions
private AbstractAction newAction;
...
void Start() {
    ...
    newAction = new newAction( this );
}
```

Then you must write the condition for the action to occur in the `private void ChooseNextAction()`
method:

```
private void ChooseNextAction() {

    if ( >>> insert some code to determine when to call the action <<< ) {
        newAction.StartAction();
    }

    currentActionActive = true;
}
```

It is important to note that this method will only be called when an existing action 
finishes.
> The `void Update()` method checks the manatee's `currentActionActive` bool,
> and if it is false, it will call `void ChooseNextAction()`.
>
> Once an action is chosen, `currentActionActive` is set to true to prevent another
> action from being chosen. 
>
> When the action finishes and calls `AbstractAction.OnComplete()`, it triggers
>  `ManateeAI.EndCurrentAction`, which sets `currentActionActive` back to false.