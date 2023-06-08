using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Alerts the player when their breath is low and creates a sense of urgency.
/// 
/// 
/// @author Alex Wills
/// Updated 7/22/2022
/// </summary>
public class BreathAlarm : MonoBehaviour
{
    [Tooltip("Game object to blink (like a canvas object) ")]
    [SerializeField] private GameObject breatheWarning;

    [Tooltip("Breath value to start the alarm at")]
    [SerializeField] private float breathAlarmLimit = 15.75f;
    // Right now, the breath bar goes red at 31.5%. With a max breath of 50, that is 15.75 when the bar turns red
    // 

    private bool warningActive = false;


    private HeartbeatEffect heartbeat;

    // Start is called before the first frame update
    void Start()
    {
        // Get the heartbeat effect
        heartbeat = GameObject.Find("Heartbeat Effect").GetComponent<HeartbeatEffect>();

        breatheWarning.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // If the player is low on breath, turn on the warning.
        if(!warningActive && PlayerScript.currentBreath <= breathAlarmLimit)
        {
            heartbeat.StartHeartbeat();
            warningActive = true;
            TelemetryManager.entries.Add(
                new TelemetryEntry("breathAlarmStart")
            );
            StartCoroutine(WarningAlarm());
        }

        // If the player is no longer low on breath, turn off the warning.
        if(warningActive && PlayerScript.currentBreath > breathAlarmLimit)
        {
            TelemetryManager.entries.Add(
                new TelemetryEntry("breathAlarmEnd")
            );
            heartbeat.StopHeartbeat();
            warningActive = false;
        }
    }


    /// <summary>
    /// Flashes the warning text to breathe now. This coroutine will run until the warningActive boolean is set to false.
    /// This coroutine must be started after warningActive is set to true, otherwise nothing will happen.
    /// </summary>
    /// <returns> IEnumerator representing the coroutine </returns>
    private IEnumerator WarningAlarm()
    {
        while (warningActive)
        {
            // Turn the warning on
            breatheWarning.SetActive(true);

            yield return new WaitForSeconds(1f);

            // Turn the warning off
            breatheWarning.SetActive(false);

            yield return new WaitForSeconds(1f);
        }
    }
}
