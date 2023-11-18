# Telemetry
Author: Alex Wills & Ender Fluegge
> Code primarily written by Ender
>
> Documented with slight modifications by Alex

Location:
- Scripts
  > `Assets/Scripts/Telemetry/`

## Usage
Create an empty game object and attach the [`TelemetryManager.cs` script](./../Dumpling%20Manatee%20Simulation/Assets/Scripts/Telemetry/TelemetryManager.cs) in the first scene.

Optionally, create an interactable canvas with:
- Image (the color of the image will be changed to indicate the connection status)
- TextMeshPro (indicate the telemetry status)
- Button (to switch to local data collection if the server fails)

and connect these to the `TelemetryManager` in the inspector. These should all be in
the first scene, before the game starts.

**To track what objects the player is looking at**, the following rules should be used:
- Objects to track must have a collider (can be a trigger collider)
- Objects must be part of the layer `RecordPlayerLookingAt`
  > if the layer does not exist, create it, making sure the spelling is the same
- Objects should have a meaningful name (for analyzing data later)

### For online data collection:
Host the [backend server](https://github.com/AlexWills37/Dumpling-Backend-Server) for 
sending telemetry data to a MongoDB database.

Ensure the connection URL ***to the backend server*** (not the database) is set in [`TelemetryManager.cs`](./../Dumpling%20Manatee%20Simulation/Assets/Scripts/Telemetry/TelemetryManager.cs)

Use the backend server commands for exporting session data to CSVs.

### For offline data collection
If the game cannot connect to the server, AND the optional status indicators are configured in the inspector,
the player will be able to click a button in-game to switch to local data collection.

Local data will be stored as csv files for each session in the persistent data path at

`<persistentDataPath>/SessionDataLogs/[sessionID].csv`

To locate the peristent data path, see [the Unity documentation](https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html).
On Quest 2, it should be `Android/data/com.NCF.DumplingManateeSimulation/files/SessionDataLogs/[sessionID].csv`


## Functionality

### Collecting Data
This system collects data on a few events in-game (like manatee interaction and scene changes),
as well as which objects the player is looking at.

Data is stored as [`TelemetryEntry`](../Dumpling%20Manatee%20Simulation/Assets/Scripts/Telemetry/api/TelemetryEntry.cs) objects in a list called `entries`, part of the `TelemetryManager`. They are added to the list as they occur, with a name, time of creation,
and optionally other relevant data.

Every 600 frames, the **Telemetry Entries** are cleared out of the `entries` list and sent
to the backend server, saved locally, or deleted (see [Sending Data](#sending-data)).



### Sending Data
If possible, the game will send data to a backend server/database for future analysis.
> This happens if the game is able to receive a session ID from the backend server
> at the start of `TelemetryManager`'s initialization. 
>
> Data will be sent to the server using the [`WebManager`](../Dumpling%20Manatee%20Simulation/Assets/Scripts/Telemetry/WebManager.cs).
>
> Data can be retrieved using the backend server and its commands.

Otherwise, if the player-interactable button is set in the inspector, that button will appear
for the player to switch to local data collection. If switched, data will be stored
locally on the device.
> The user must manually switch to this mode with a button in-game.
> 
> Data will be saved using the [`LocalDataManager`](../Dumpling%20Manatee%20Simulation/Assets/Scripts/Telemetry/LocalDataManager.cs).
>
> Data can be found at `Android/data/com.NCF.DumplingManateeSimulation/files/SessionDataLogs/[sessionID].csv`

If the button is not pressed, data will be ignored.
> Instead of sending data anywhere, it will just be cleared from memory.

### Interpreting Data
Either from the backend server or local storage, you should have a CSV file for each
session that was recorded.
The CSV has the following columns:

**name**
- a descriptor for the type of entry this is.

**time**
- timestamp of when this entry was created, based on the system time.
- this is how to sort entries chronologically.
- included in every entry.

**vec, textContent, intContent**
- depending on the type of entry this is, different additional information is helpful.
- vec is vector information, used for identifying positions.

Here is a list of the different possible data entries, organized by their **name**:

#### sceneCompleted
Indicates the end of an in-game scene.
- textContent: name of the scene that was just sceneCompleted
- intContent: time it took to complete the scene (in seconds)
> Recorded by subscribing to the `SceneManager.sceneUnloaded` event and looking
> at the `timeSinceLevelLoad`.

#### seagrassEaten
Indicates when the player eats seagrass.
> Only contains the time seagrass is eaten.
>
> Logged in the `EdibleGrassBehavior.cs` script (link needed) whenever the seagrass
> is eaten.

#### manateeInteraction
Indicates when the player interacts with a manatee by petting it.
> Only contains the time the player interacts with a manatee.
>
> Logged in [`ManateeBehavior.cs`](../Dumpling%20Manatee%20Simulation/Assets/Scripts/ManateeAI/ManateeBehavior.cs) in the `PlayerInteraction()` function.
>
> Will only log if the manatee responds to player interaction (so if the manatee
> cannot interact, as is the case when it is breathing, the manatee interaction
> will not happen and this event will not be called. Also, if the player boops the
> manatee multiple times, the manatee will only interact a second time if the previous animation is finished).

#### manateeNameSelected
In the introduction scene, this event occurs when the player clicks a button to choose
the name of their manatee friends.
- textContent: name that the player chose for their friend
> The last 2 names chosen by the player are the names that the game will assign to the manatees.
> > The player is able to click the buttons multiple times to change their friends' names.
>
> Logged in [`ManateeNameChooser.cs`](../Dumpling%20Manatee%20Simulation/Assets/Scripts/ManateeNameChooser.cs) in the `ChooseName()` function.

#### tutorialTaskCompleted
In the tutorial scene, this event occurs when the player completes each task. This can
be used to see how long players spend on each task.
- textContent: the task the player just completed.
> Logged in [`TutorialGameManager.cs`](../Dumpling%20Manatee%20Simulation/Assets/Scripts/TutorialGameManager.cs) in the `MoveToNextTask()` funciton.
>
> textContent comes from the array of tasks specified in the inspector.
>
> To determine the time spent on a task, subtract the **time** of the previous
> task completion from the **time** of this task completion (for the first task,
> use the previous **sceneCompleted** event).

#### taskCompleted
In the Eutrophication scene, this event occurs to mark when each task is completed.
- textContent: the task the player just completed:
  - **sceneBeginsNow** - marks the beginning of the scene for easier time comparisons
  - **eatAllSeagrass** - marks when the seagrass task is completed -- after the player eats all of the seagrass and 5 seconds passes, and the textbox informs the player there is not enough seagrass
  - **checkOnManateeFriend\*** - marks when the textbox displaying info about malnourished manatees (which shows up after petting a manatee) disappears
  - **seeLetterAtMailbox** - marks when the player approaches the mailbox and begins to read the letter to humans

> Logged in [`HellGameManager.cs`](../Dumpling%20Manatee%20Simulation/Assets/Scripts/HellGameManager.cs) when different events occur.
> 
> \* the **checkOnManateeFriend** event may not occur. If the player pets a manatee,
> then finishes the seagrass task before the manatee task textbox dissapears on its own,
> this entry will not be added. 
>
> To know when the player interacts with a manatee, use the first **manateeInteraction** entry in this scene.
>
> To know when the player eats all of the seagrass, use the last **seagrassEaten** entry
> in this scene.
>
> To know when the player sends the letter to the humans, use this scene's **sceneCompleted** entry.

#### lookingAt
- textContent: name of the object the player was looking at
- intContent: time spent looking at the object (in milliseconds)

> Recorded by doing a raycast from the player's position on every frame.
>
> This event is only affected by objects in the `RecordPlayerLookingAt` layer.
>
> The entry is created when the player looks at a *new* object in the tracking layer.
> > This means that if the player is looking at nothing, the timers will pause.
> >
> > Example: if the player looks at a manatee for 5 seconds, then looks at the sky
> > for 3 seconds, then looks at the same manatee for 2 seconds, that will be recorded
> > as the player looking at the manatee for 7 seconds.
>
> For a list of objects that are tracked in the `RecordPlayerLookingAt` layer, see [this section].


### lookingAt objects
Below is a list of objects that are currently being tracked by the **lookingAt** telemetry
entry, organized by the scene they appear in.
#### Intro on Boat
- Slide 0 - Introduction
- Slide 1 - Manatee Info
- Slide 2 - Manatee Social Behavior
- Slide 3 - Manatee Diet/Sound
- Slide 4 - Manatee Anatomy
- Slide 5 - Choose Manatee Names
- Slide 6 - Become a Manatee
#### Tutorıal
- Tutorial Grass (Tasty)
- Health Bar
- Breath Bar
#### Manatee Heaven
- Manatees
- Health Bar
- Breath Bar
- Bus Stop
- Mangrove Info Box
- Seagrass Info Box
- Fish Info Box
- Seagrass(Tasty)
#### Manatee School
- Slide 0 - Welcome
- Slide 1 - Eutrophication Definition
- Slide 2 - Pollution Sources
- Slide 3 - Algae Blooms
- Slide 4 - Seagrass Loss
- Slide 5 - Manatee Mortality
- Slide 6 - Human Help Is Needed
- Slide 7 - Keep an Eye Out for Pollution
- Slide 8 - End of Class
- Next Slide Button
- Manatees
- Bus Stop
#### Manatee Hell
- Manatees
- Health Bar
- Breath Bar
- Seagrass(Tasty)
- Mailbox
#### Boat Conclusion
- Waving Manatee
- Conclusion Text