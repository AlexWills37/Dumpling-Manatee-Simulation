using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CausticsAnimation : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Frames per second for the caustics effect")]
    private float FPS = 30;
    private float timeBetweenFrames;    // Time in seconds between animation frames (not the same as the game's framerate)

    private Light lightSource;  // The directional light that is used for the caustics effect
    private Texture2D[] causticFrames;  // List of textures for the caustics effect
    private int currentFrame = 0;   // Current index in the texture list (this will loop through the entire list on repeat)

    // Start is called before the first frame update
    void Start()
    {
        // Load all Texture2D assets from /Assets/Resources/Caustics
        causticFrames = Resources.LoadAll<Texture2D>("Caustics");
        lightSource = GameObject.Find("Caustics Light").GetComponent<Light>();
        timeBetweenFrames = 1.0f / FPS;

        // Start the caustics effect (this will continue for the script's lifetime)
        StartCoroutine(CausticsCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Infinitely running coroutine to update the caustics effect at a consistent rate.
    /// </summary>
    private IEnumerator CausticsCoroutine() {

        while (true) {
            // Move to the next texture for the caustic effect
            currentFrame = (currentFrame + 1) % causticFrames.Length;
            lightSource.cookie = causticFrames[currentFrame];

            // Wait until the next animation frame
            yield return new WaitForSecondsRealtime(timeBetweenFrames);
        }
    
    }
}
