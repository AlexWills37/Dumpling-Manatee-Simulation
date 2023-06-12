# Water Effects

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
