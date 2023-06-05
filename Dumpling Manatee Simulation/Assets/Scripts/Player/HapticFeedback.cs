    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//DOCUMENTATION:
//https://www.youtube.com/watch?v=9KJqZBoc8m4
//https://developer.oculus.com/documentation/native/pc/dg-input-touch-haptic/
//https://developer.oculus.com/documentation/unity/unity-haptics/

///<summary>
/// This script provides haptic feedback for the oculus touch controllers.
/// You need to call the TriggerVibration method from another script wherever
/// you want to trigger haptic feedback. You can change the iteration, frequency, 
/// strength, and which controller you want to buzz.
///
/// @author Sami Cemek
/// Updated: 07/22/21
/// 
/// </summary>


public class HapticFeedback : MonoBehaviour
{
    // Static instance allows for haptic feedback to be activated from any context
    public static HapticFeedback singleton;

    private bool rumbling = false;
    private float timeElapsed = 0f;
    private float rumblingTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (singleton && singleton != this)
            Destroy(this);
        else
            singleton = this;
    }

    private void Update()
    {
        // Continous rumble specified from TriggerVibrationTime
        if (rumbling)
        {
            TriggerVibration(40, 2, 255, OVRInput.Controller.LTouch);
            TriggerVibration(40, 2, 255, OVRInput.Controller.RTouch);

            // Stop rumbling after the specified time has passed
            timeElapsed += Time.deltaTime;
            if(timeElapsed > rumblingTime)
            {
                rumbling = false;
            }
        }
    }

    //Use this if you have an audio clip that is triggered with haptic feedback
    public void TriggerVibration(AudioClip vibrationAudio, OVRInput.Controller controller)
    {
        OVRHapticsClip clip = new OVRHapticsClip(vibrationAudio);

        if(controller == OVRInput.Controller.LTouch)
        {
            //Trigger On Left Controller
            OVRHaptics.LeftChannel.Preempt(clip);
        }
        else if(controller == OVRInput.Controller.RTouch)
        {
            //Trigger On Right Controller
            OVRHaptics.RightChannel.Preempt(clip);
        }
    }

    //Use this if you want to have custom iteration, frequency, strength, and which controller you want to buzz
    public void TriggerVibration(int iteration, int frequency, int strength, OVRInput.Controller controller)
    {
        OVRHapticsClip clip = new OVRHapticsClip();

        for (int i = 0; i < iteration; i++)
        {
            clip.WriteSample(i % frequency == 0 ? (byte)strength : (byte)0);
        }

        if (controller == OVRInput.Controller.LTouch)
        {
            //Trigger On Left Controller
            OVRHaptics.LeftChannel.Preempt(clip);
        }
        else if (controller == OVRInput.Controller.RTouch)
        {
            //Trigger On Right Controller
            OVRHaptics.RightChannel.Preempt(clip);
        }
    }

    /// <summary>
    /// Rumble both controllers for a given amount of time.
    /// </summary>
    /// <param name="duration"> Time (in seconds) to vibrate the controllers </param>
    public void TriggerVibrationTime(float duration)
    {
        rumblingTime = duration;
        timeElapsed = 0;
        rumbling = true;
    }
}



//ADD THIS PART TO ANOTHER SCRIPT
//HapticFeedback.singleton.TriggerVibration(40, 2, 255, ovrGrabbable.grabbbedBy.GetController());