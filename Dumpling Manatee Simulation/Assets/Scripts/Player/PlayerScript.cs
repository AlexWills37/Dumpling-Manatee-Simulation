using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// 
/// This script is using variables and methods of Healtbar.cs script.
/// It sets the health and breath to the max in the beginning. Health 
/// and breath decrease over time.
///
/// @author Sami Cemek
/// @author Alex Wills
/// Updated: 07/17/2022
/// 
/// </summary>


public class PlayerScript : MonoBehaviour
{
    [Tooltip("Maximum health for the player")]
	public float maxHealth = 100;

    [Tooltip("Player's current health")]
    public static float currentHealth;

    [Tooltip("How much food the player has eaten")]
    public static int ateGrassNum;

    [Tooltip("Whether or not the player has interacted with a manatee")]
    public static bool interactedWithManatee;

    [Tooltip("Maximum breath for the player")]
    public float maxBreath = 180;

    [Tooltip("The player's current breath level")]
    public static float currentBreath;

    [Tooltip("Bar to display the player's health")]
    public HealthBar healthBar;

    [Tooltip("Bar to display the player's breath")]
    public HealthBar breathBar;

    [Tooltip("Whether the breath meter should decrease over time")]
    public bool breathDecreasing = true;

    private float timeSinceTelemetry = 0f;  // Used to send an update on the player's scores every 10 seconds

    //private float camXPos, camZPos;
    //public float camYPos;

    // Start is called before the first frame update
    void Start()
    {
        // Set initial values (10 health, max breath)
		currentHealth = 10;
		if(healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
        ateGrassNum = 0;
        interactedWithManatee = false;

        if(breathBar != null)
        {
            currentBreath = maxBreath;
            breathBar.SetMaxHealth(maxBreath);
        }
        

       // transform.position = new Vector3 (camXPos, camYPos, camZPos);
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

            // Health/points are modified externally with other scripts, so it is important
            // To ensure that health is not above the max
            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            healthBar.SetHealth(currentHealth);
            breathBar.SetBreath(currentBreath);

        }

        // Send telemetry updates for the player's breath and health
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

    private void OnTriggerStay(Collider other)
    {
        // If we are colliding with air, increase our breath bar
        if (other.gameObject.CompareTag("Air"))
        {
            currentBreath = Mathf.Clamp(currentBreath + 12 * Time.deltaTime, 0, maxBreath);
            breathBar.SetBreath(currentBreath);
        }
    }
}
