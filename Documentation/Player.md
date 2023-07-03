# Player
Author
> Alex Wills

Location
> \>Insert location<

## Usage
Drag and drop the `Player` prefab into the scene, and delete any other cameras.

In the inspector, you can set the player's maximum health and breath levels.
*Health Bar* and *Breath Bar* should already be set in the prefab, but if it isn't,
the `ScoreBar` objects can be dragged from `Player -> HUD (Center anchor) -> HUD Canvas
-> Health/Breath Bar` with their matching names.

You can also clear *Breath Decreasing* if the player's breath should not decrease over time
(in a tutorial, for example).

## Health/Breath Bars
These bars are primarily Unity's [slider component](https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/script-Slider.html) 
with the interactable components removed. They also use the `ScoreBar.cs` script to
control the slider's value through other scripts, changing the color of the bar
based on the colors set in the inspector.

The `PlayerManager.cs` script, attached to the **Player** object, uses the `ScoreBar.cs` 
script of the two bars to lower them over time.

## HUD
Some of the HUD's behavior is explained below at the bottom of the [Hierarchy Structure](#hierarchy-structure). There is also an explanation in the header of the [`HUDBehavior.cs` script](./../Dumpling%20Manatee%20Simulation/Assets/Scripts/Player/HUDBehavior.cs).

## Movement
The player's velocity is set based on the input from the Quest controllers.

## Hierarchy Structure
In the hierarchy,
- PhysicalPlayer
  > This is the object with the colliders and the main rigidbody.
  > 
  > It also has the `PlayerController.cs` script, which uses inputs from the Oculus
  > controllers to move the player freely underwater, without gravity. In the inspector,
  > you must set up the Camera Rig and the Forward Direction.
  >
  > *Camera Rig* should be set to the *OVRCameraRig* object in this hierarchy (
  > `Player -> OVRCameraRig`).
  >
  > *Forward Direction* should be set to *CenterEyeAnchor* in `OVRCameraRig -> TrackingSpace
  > -> CenterEyeAnchor`.
  >
  > You can also customize the buttons for swimming up/down, and the speed of swimming.
  - Personal Space
    > This object can be used as a general collider for the manatees to detect the player.
    > If a manatee swims into this collider, it will stop swimming to avoid swimming into
    > the player.
- OVRCameraRig
  > This is Meta's prefab (with some modifications) for connecting to the headset's 
  > hand and head tracking.
  >
  > In the inspector for the `OVR Manager` component, *Tracking Origin Type* can be:
  > - Eye Level
  >   
  >   The game object's position is starting at eye level, so the user's cameras
  >   will start wherever the game object is.
  >
  > - Floor Level
  >   
  >   The game object's position is starting at floor level, so the user's cameras
  >   will be moved up to match their height.
  >
  > The `PlayerController.cs` script moves the entire camera rig every frame to match the
  > physical player's location.
  - TrackingSpace
    > These objects move with the player's movement.
    -  LeftEyeAnchor
    -  CenterEyeAnchor
       > This object should be assigned to *Forward Direction* in the PhysicalPlayer's 
       > `PlayerController.cs` script.
    - RightEyeAnchor
    - TrackerAnchor
    - LeftHandAnchor
      - LeftControllerAnchor
        > Here is where we attach the manatee flipper to change how the player's hands
        > look.
        - Flipper
          > This was added to the prefab for our simulation.
    - RightHandAnchor
      - RightControllerAnchor
        - Flipper
          > Same as the Left side
- Vibration Manager
  > Uses the `HapticFeedback.cs` script to rumble the player's controllers for
  > dynamic feedback.
- HUD (Center anchor)
  > This object contains the graphical information that follows the player (like 
  > their health and breath meters).
  >
  > The `HUDBehavior.cs` script rotates this object to follow the player's Y-rotation.
  > This is to act similar to the Quest's own menus. 
  >
  > It does not follow X or Z rotation
  > so that it stays upright and doesn't force the player to look at it.
  >
  > It does not directly follow Y-rotation so that it never feels "stuck" on the
  > player's vision.
  >
  > *Angle Window* is the angle (in degrees) away from the center that the player
  > can look away from before it rotates the HUD.
  >
  > *Time To Recenter* is the time (in seconds) for the HUD to "catch up" with the player
  > once they look far enough away.
  >
  > *Rotation Curve* defines how the HUD will rotate to catch up to the player. The curve
  > should start at (0, 0) (time = 0, rotation = starting rotation), and it should
  > end at (1, 1) (time = 1, rotation = player's rotation).
  - HUD Canvas
    > The canvas oject for displaying UI elements. This object is offset from it's parent
    > so that it appears in front of the player, and when the HUD rotates, this canvas
    > rotates "around" the player to stay in front of their vision.
    - Health Bar
      > Displays the player's health.
    - Breath Bar
      > Displays the player's breath. 