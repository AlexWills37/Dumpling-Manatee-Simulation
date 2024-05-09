# Organization of the Assets folder

In future iterations of the project, feel free to reorganize the assets in whatever manner makes the most sense.

> This list is organized in order of relevance for becoming familiar with the projet structure.

## Assets (not in a subfolder)

In the assets window, you will notice many folders and 2 unorganized assets:

- Boat Guard (material)
  > This is the material used for the cube on the boat in the first scene. This cube stops the player from being able to walk off the boat.
- CausticsAnimation.cs
  > This script is attached to the **Caustics Light** object in all of the underwater scenes.
  >
  > See [the caustics documentation](./Water.md#caustics-light-on-the-seafloor) for more information.

These assets should probably be organized into subfolders.

## Envrionmental

This folder contains all of the assets (scripts, models, prefabs, etc.) for:

- Algae particles
  - Particle system
- Fish
  - Model
  - Behavior script
- Mangrove trees
  - Model
- Seabed
  - Model
  - Textures
- Seagrass (every kind EXCEPT for edible seagrass)
  - Models
  - Scripts (for waving seagrass)
  > Edible seagrass components are in the *Gameplay* folder.
- Small boat that drives in the distance above water
  - Model
  - Animation components

## Gameplay

This folder contains:

- Edible Sea Grass
  - Models
  - Particle system
  - Script
- Manatee
  - Models
  - Animation components
  > Manatee behavior scripts are in the *Scripts/ManateeAI* folder.

## Prefabs

- Caustics Light
  > A directional light that creates the effect of underwater lighting.
- Fish System (deprecated)
  > A boids-based fish system imported from the asset store.
  >
  > This project uses a separate script in the *Environmental/Fish* folder instead of this system.
- Flipper
  > The positioned model + colliders for the player's manatee flipper hands.
- LevelExitVolume
  > The bus stop and its trigger collider for moving the game between scenes.
- Manatee
  > Contains the manatee model and its rigidbody, colliders, behavior script, and particle system.
  - Manatee Named
    > A variant of **Manatee** with an added nametag object.
    - Manatee Named, Thin Variant
      > A variant of **Manatee Named**, using a different model and a different particle system.
- Player
  > The player's underwater character, with manatee flippers for hands, swimming movement, a HUD, and personal space.
- Popup
  > Area-based popup text.
  >
  > From a distance, this object appears as a small text bubble with "..." inside.
  >
  > When the player enters its trigger collider, it expands to a larger textbox with information.
  >
  > When the player leaves, it will shrink back down again.
- Score Bar
  > A canvas element group that shows the player a certain score through a meter and color.
  >
  > This is currently used for the player's breath and health.
- Spotted SeaTrout
  > Spotted Sea Trout.
- Sun Rays
  > Particle system for the sun ray effect underwater. To use, set the *To Follow* object as the player.
- Task Panel
  > A group of canvas elements to represent a player task. The task can be completed, checking off a box and pulsing the text's color, or it can be transitioned into a new task.

## Scripts

This is where a majority of the game's scripts are.

### gameControllers

This folder is intended to contain the scripts that progress each scene.

Currently, it only has:

- ManateeHeavenGameController.cs
  > Progresses the game as the player completes tasks in the Manatee Heaven scene (2 - Manatee Life)

The other controllers for scene logic are not organized into this folder. They can be found just in the *Scripts/* folder.

### ManateeAI

Contains the scripts that control the manatee's behavior.
> See [ManateeAI documentation](./ManateeAI.md) for detailed information on how these scripts work.

- AbstractAction.cs
  > Outlines an abstract "Action" object for the manatee to do something.
- ManateeActions.cs
  > Contains all of the manatee's current actions (as implementations of **AbstractAction**).
- ManateeBehavior.cs
  > Controls the manatee at a high level, deciding which action to take and when.
- BabyManateeBehavior.cs (deprecated)
  > Script from Twizzlers simulation for a baby manatee to follow its parent.
- ManateeBehaviorOLD.cs (deprecated)
  > Script from Twizzlers simulation for the adult manatees.
- SwimTo.cs (deptrecated)
  > The first (unsuccessful) attempt at making a modular manatee behavior system.

### Player

Scripts to control the player functionality (see [Player documentation](./Player.md)).

- BreatheAir.cs
  > Increments the player's breath levels if they are at the water's surface.
- HapticFeedback.cs
  > Rumbles the player's controllers.
- HUDBehavior.cs
  > Rotates the player's HUD to always be in the line of sight.
- PlayerController.cs
  > Gives the player swimming controls.
- PlayerManager.cs
  > Connects the player's scores (health and breath).
- ScoreBar.cs
  > Controls the visual score bars (health and breath).
- BreathAlarm.cs (deprecated)
  > Creates a pulsing vignette effect in the player's vision when their breath is low.
  >
  > No longer functional because of changes to Unity's post processing API (it could be fixed).
- HearbeatEffect.cs (deprecated)
  > Uses post processing to create a pulsing vision effect (see BreathAlarm.cs).
  >
  > Broken by changes to Unity.

### Telemetry

These scripts handle the game's data collection.

For more information, see [the Telemetry documentation](./Telemetry.md).

- TelemetryManager.cs
  > Collects and processes gameplay data.
- WebManager.cs
  > Sends data to the backend server.
- LocalDataManager.cs
  > Stores data in CSV files on the headset locally.
- api
  > Contains the magic that helps Unity send data over the internet.

### Scripts (not in a subfolder)

Unorganized in the *Scripts* folder, we have:

#### Scene Managers

- BoatSceneManager.cs
  > 0 - Boat Scene
- TutorialGameManager.cs
  > 1 - Underwater Tutorial
- ManateeSchoolManager.cs
  > 3 - ManateeSchool
- HellGameManager.cs
  > 4 - V2 ManateeHell

#### Manatee Nametag Scripts

- InsertManateeName.cs
  > Adds a chosen manatee name to a text field.
- ManateeNameChoose.cs
  > Handles the player's chosen names.
- ManateeNametag.cs
  > Displays the manatee's name chosen by the player.

#### Helpful utility scripts

- ChangeScene.cs
- FollowObject.cs
  > Useful when you want an object to only match part of another object's position/rotation (instead of making the object a child, which matches all movement).
- FPS.cs
  > Calculates and displays the game's FPS over a continous 10 frame sample.
- LevelExitBehavior.cs
  > Transitions to the next scene when the player enters the object's trigger collider.
- RecenterOVR.cs
  > Resets the OVR camera's rotation the frame after the scene loads.
- SlideDeck.cs
  > Manages an interactive slide deck.
  >
  > (See [Slide Deck documentation](./SlideDeck.md)).
- TimerBehavior.cs
  > Creates a controllable TMPro timer.

#### Specialized scripts

- GUITextBox.cs
  > Used in scene 4 - V2 ManateeHell under **GUI Text Box** to display information without creating overlapping text boxes.
- ManateeSoundButton.cs
  > Used in scene 0 - Boat Scene.
  >
  > Plays sound on a button press.
- Popup.cs
  > Used in scene 2 - Manatee Life.
  >
  > Displays a textbox when the player approaches.
- TaskBar.cs
  > Controls the player's tasks on the GUI.

## Audio

Contains the game's sound files.

## Models

Contains some of the models used in this project.

## Particles

Contains the game's particles.

## Resources

Contains files that will be loaded alongisde the game.

These files include the caustics frames, which are accessed in code in the `CausticsAnimation.cs` script.

```C#
private Texture2D[] causticFrames;  // List of textures for the caustics effect
...
// Start is called before the first frame update
void Start()
{
    // Load all Texture2D assets from /Assets/Resources/Caustics
    causticFrames = Resources.LoadAll<Texture2D>("Caustics");
    ...
}
```

## Scenes

Contains the game's scene objects. The scenes used in the game begin with their index.

## Sprites

Contains the image files used in the game.

> Images used for particles are found in the *Particles* folder.

**The following folders contain imported or generated assets.**

## #NVJOB Boids

Contains (unused) imported asset for controlling fish.

> We stopped using this asset because the fish were swimming above water in the air.

## Free Stylized Garden Asset

Contains the mailbox.

## Palm tree

Palm tree.

## _TerrainAutoUpgrade

Generated by Unity's terrain system.

## Oculus

Oculus integration package.

## Plugins

Generated by Unity.

## Standard Assets

Contains water surface asset.

## TextMeshPro

TMPro files.

## XR

Generated by Unity.