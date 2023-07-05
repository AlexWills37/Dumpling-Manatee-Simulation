using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// This script manages the player's gamified behavior (losing breath, gaining health, etc.).
/// For the script that manages the player's movement, see PlayerController.cs.
/// This script is using variables and methods of ScoreBar.cs script.
/// It sets the health and breath to the max in the beginning. Health 
/// and breath decrease over time.
///
/// @author Sami Cemek
/// @author Alex Wills
/// Updated: 07/17/2022
/// 
/// </summary>


public class PlayerManager : MonoBehaviour
{
    [Tooltip("Maximum health for the player")]
	[SerializeField] private float maxHealth = 100;

    [Tooltip("Maximum breath for the player")]
    [SerializeField] private float maxBreath = 100;

    /// <summary>
    /// The player's current health
    /// </summary>
    public float currentHealth {get; private set;}
    
    /// <summary>
    /// The player's current breath
    /// </summary>
    public float currentBreath {get; private set;}

    /// <summary>
    /// How many seagrass the player has eaten
    /// </summary>
    public int ateGrassNum {get; private set;} = 0;

    // Whether or not the player has interacted with a manatee
    private bool interactedWithManatee;

    private HapticFeedback haptics;



    [Tooltip("Bar to display the player's health")]
    [SerializeField] private ScoreBar healthBar;

    [Tooltip("Bar to display the player's breath")]
    [SerializeField] private ScoreBar breathBar;

    [Tooltip("Whether the breath meter should decrease over time")]
    [SerializeField] private bool breathDecreasing = true;

    private float timeSinceTelemetry = 0f;  // Used to send an update on the player's scores every 10 seconds


    // Start is called before the first frame update
    void Start()
    {
        // Set initial values (10 health, max breath)
		currentHealth = 10;
        currentBreath = maxBreath;
        ateGrassNum = 0;
        interactedWithManatee = false;
		
        if(healthBar != null)
        {
            healthBar.SetBarMaxValue(maxHealth);
            healthBar.SetBarValue(currentHealth);
        }

        if(breathBar != null)
        {
            breathBar.SetBarMaxValue(maxBreath);
            breathBar.SetBarValue(currentBreath);
        }

        haptics = this.GetComponentInChildren<HapticFeedback>();

        
    }

    // Update is called once per frame
    void Update()
    {

        //currentHealth -= 2 * Time.deltaTime;

        if(healthBar != null && breathBar != null)
        {
            // Lower breath over time
            if (breathDecreasing && currentBreath > 0)
            {
                currentBreath -= 1 * Time.deltaTime;

                // When breath is low, decrease health
            } else if (currentBreath <= 0)
            {
                Mathf.Max(0, currentHealth -= 1 * Time.deltaTime);
            }

            // Ensure health is not above the maximum
            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            healthBar.SetBarValue(currentHealth);
            breathBar.SetBarValue(currentBreath);

        }

        // Send telemetry updates for the player's breath and health after 10 seconds
        timeSinceTelemetry += Time.deltaTime;
        // Every 10 seconds, add a telemetry entry for the player's scores
        if (timeSinceTelemetry >= 10)
        {
            TelemetryManager.entries.Add(
                new TelemetryEntry("playerHealth", ((int) currentHealth))
            );
            TelemetryManager.entries.Add(
                new TelemetryEntry("playerBreath", ((int) currentBreath))
            );

            timeSinceTelemetry = 0f;
        }
    }

    public void Breathe()
    {
        currentBreath = Mathf.Clamp(currentBreath + 12 * Time.deltaTime, 0, maxBreath);
    }

    public void OnGrassEaten() {
        this.currentHealth += 10;
        haptics.TriggerVibrationTime(0.1f);
    }
}
