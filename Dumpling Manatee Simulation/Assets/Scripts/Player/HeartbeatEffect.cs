using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Creates a vignette effect that pulses like a heartbeat.
/// 
/// Scripting post processing effects: https://docs.unity3d.com/Packages/com.unity.postprocessing@3.0/manual/Manipulating-the-Stack.html
///
/// THIS SCRIPT IS BROKEN IN UNITY 2021.3
/// 
/// @author Alex Wills
/// Updated 7/22/2022
/// </summary>
public class HeartbeatEffect : MonoBehaviour
{
    // private Vignette vignette;
    // private PostProcessVolume volume;
    // private PostProcessProfile profile;

    [Tooltip("The intensity of the vignette to fade into before fading out.")]
    [Range(0f, 1f)]
    [SerializeField] private float maxIntensity = 0.7f;

    [Tooltip("How many seconds it takes to fade the vignette in")]
    [SerializeField] private float fadeInTime = 0.4f;

    [Tooltip("How many seconds it takes to fade the vignette out")]
    [SerializeField] private float fadeOutTime = 0.8f;
  
   
    private bool fading = false; // This bool is used to ensure only one fade occurs at a time
    private bool heartBeating = false; // This bool controls whether or not the effect is active

    // Start is called before the first frame update
    void Start()
    {
        // // Get the vignette component, which is inside the profile, which is inside the volume
        // volume = this.GetComponent<PostProcessVolume>();
        // profile = volume.profile;
        // vignette = profile.GetSetting<Vignette>();

        // // Start the vignette hidden
        // vignette.intensity.Override(0);

    }

    // Update is called once per frame
    void Update()
    {
        if(heartBeating && !fading)
        {
            StartCoroutine(PulseVignette(fadeInTime, fadeOutTime));
        }
    }

    /// <summary>
    /// Start the heartbeat effect (the effect will go on continously until stopped).
    /// </summary>
    public void StartHeartbeat()
    {
        heartBeating = true;
    }

    /// <summary>
    /// Stop the heartbeat effect. The current heartbeat will finish.
    /// </summary>
    public void StopHeartbeat()
    {
        heartBeating = false;
    }

    /// <summary>
    /// Pulses the vignette in and out.
    /// </summary>
    /// <param name="fadeInDuration"> The time to fade in </param>
    /// <param name="fadeOutDuration"> The time to fade out </param>
    /// <returns> IEnumerator representing this coroutine </returns>
    private IEnumerator PulseVignette(float fadeInDuration, float fadeOutDuration)
    {
        // fading = true;  // Make sure that only one pulse happens at a time with this boolean

        // float timeElapsed = 0f;
        
        // // Fade in
        // while(vignette.intensity < maxIntensity)
        // {
        //     timeElapsed += Time.deltaTime;
        //     vignette.intensity.Override( Mathf.Clamp01( (timeElapsed / fadeInDuration) * maxIntensity) );
        //     yield return null;
        // }

        // // Fade out
        // timeElapsed = 0f;
        // while(vignette.intensity > 0)
        // {
        //     timeElapsed += Time.deltaTime;
        //     vignette.intensity.Override( Mathf.Clamp01( (1f - (timeElapsed / fadeOutDuration)) * maxIntensity ) );
        //     yield return null;
        // }

        // fading = false; // Allow the next pulse to happen
        yield return null;
    }


}
