using UnityEngine;
using UnityEngine.Events;

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
    public bool breathed { get; private set; } = false;

    // Whether or not the player has interacted with a manatee
    public bool interactedWithManatee {get; private set;} = false;

    private HapticFeedback haptics;



    [Tooltip("Bar to display the player's health")]
    [SerializeField] private ScoreBar healthBar;

    [Tooltip("Bar to display the player's breath")]
    [SerializeField] private ScoreBar breathBar;

    [Tooltip("Whether the breath meter should decrease over time")]
    [SerializeField] private bool breathDecreasing = true;

    private float timeSinceTelemetry = 0f;  // Used to send an update on the player's scores every 10 seconds

    public static UnityEvent playerValuesUpdated = new UnityEvent();

    public UnityEvent onGrassEaten;
    public UnityEvent onManateeInteraction;
    
    // Start is called before the first frame update
    void Start()
    {
        // Set initial values (40% health, max breath)
		currentHealth = maxHealth * 0.4f;
        currentBreath = maxBreath;
        ateGrassNum = 0;
        breathed = false;
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
        breathed = true;
        playerValuesUpdated.Invoke();
        currentBreath = Mathf.Clamp(currentBreath + 12 * Time.deltaTime, 0, maxBreath);
    }

    public void OnGrassEaten() {
        ateGrassNum++;
        playerValuesUpdated.Invoke();
        this.currentHealth += 10;
        haptics.TriggerVibrationTime(0.1f);
        onGrassEaten.Invoke();
    }

    public UnityEvent getPlayerValuesEvent()
    {
        return playerValuesUpdated;
    }

    /// <summary>
    /// Changes the player's breath level (for use in the tutorial scene).
    /// </summary>
    /// <param name="newBreath"> the new breath level </param>
    public void SetBreath(int newBreath) {
        this.currentBreath = newBreath;
    }

    /// <summary>
    /// Updates the player to interact with the manatee. 
    /// This function is called from the ManateeBehavior script whenever the player collides
    /// with a manatee (as long as the manatee's action is interruptable, and
    /// the manatee is able to play its interaction animation)
    /// </summary>
    public void InteractWithManatee() {
        interactedWithManatee = true;
        haptics.TriggerVibrationTime(0.1f);
        onManateeInteraction.Invoke();
    }
}
