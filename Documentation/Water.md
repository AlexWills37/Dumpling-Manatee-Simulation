# Water Effects

## Caustics (light on the seafloor)
In the underwater scenes in the hierarchy, there is an object named "Caustics Light". This is a directional light that uses lighting cookies to create the caustics effect. It uses the script [`CausticsAnimation.cs`](./../Dumpling%20Manatee%20Simulation/Assets/CausticsAnimation.cs) to create a smooth animation.

[Light Cookies](https://docs.unity3d.com/Manual/Cookies.html) are a way of giving lights custom shapes and patterns. The images that make up the animation (30 frames) can be found in [the `Assets/Resources/Caustics/` folder](./../Dumpling%20Manatee%20Simulation/Assets/Resources/Caustics/).
I do not have the original source of the images.

The script starts by loading all of the images from the Resources folder into an array.
It then starts a coroutine that switches the light's cookie to the next array image every
1/30th of a second (or at a different speed, if FPS is changed in the inspector).

## Rays of light
The beams of light use Unity's [built-in particle system](https://docs.unity3d.com/Manual/Built-inParticleSystem.html) and a custom script.

The particle is a soft glow circle stretched to look like a beam (the streteched circle also gives the effect of the beams fading in and out as they move in/out of the player's sight).

## Depth fog
In the lighting menu (`Window -> Rendering -> Lighting`), under the Environment tab, in
the Other Settings section, we have enabled fog.

## Water surface
The water surface is taken from Unity's Standard Assets. These assets are no longer available on the Asset Store, but they can still be found at [this github repository](https://github.com/jamschutz/Unity-Standard-Assets).

In particular, I am using the water prefab from `Standard Assets/Environment/Water/Water4/Prefabs/Water4Advanced`.

One change has been made to fix a compiler error with the shader at `Standard Assets/Environment/Water/Water4/Shaders/FXWater4Advanced.shader`. Line 88 has been changed to declare the depth texture differently, following the advice from [this forum post](https://answers.unity.com/questions/1529224/opengles3-fails-to-compile-shaders-hiddenpost-fx-f.html):
> I found an answer that worked for me here https://issuetracker.unity3d.com/issues/gearvr-singlepassstereo-image-effects-are-not-rendering-properly
>
> As Arkade says, to fix any shader with this problem, swap the declaration of the sampler from:
>
> >   uniform sampler2D_float _CameraDepthTexture;
>
> to
>
> >   UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
>
> \- Posted by *mckinnonCT* · Oct 05, 2018 at 05:14 PM 


## Fish
The fish use a model created by Devin Gregg, and a script from the [Simple Boids asset by Nick Veselov](https://assetstore.unity.com/packages/3d/characters/animals/simple-boids-flocks-of-birds-fish-and-insects-164188).