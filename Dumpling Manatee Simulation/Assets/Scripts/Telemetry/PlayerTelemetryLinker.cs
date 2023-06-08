using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Link the player's transform with the static telemetry manager at the start of the scene.
/// 
/// This script should be attached to the player game object that will be moving and rotating as
/// the player controls the game. 
/// 
/// @author Alex Wills
/// Updated 2/22/2023
/// </summary>
public class PlayerTelemetryLinker : MonoBehaviour
{

    private static TelemetryManager manager = null;

    
    [Tooltip("Transform that contains player's position and rotation.")]
    [SerializeField] private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        // Default to this transform unless specified in the editor
        if(playerTransform == null)
        {
            playerTransform = this.gameObject.transform;
            Debug.Log("WARNING: Player Transform not set in PlayerTelemetryLinker. Defaulting to parent object.");
        }


        // Find the telemetry manager if it hasn't been found before
        if(manager == null)
        {
            manager = GameObject.Find("TelemetryManager").GetComponent<TelemetryManager>();
            Debug.Log("Adding manager: ", manager);
        }
        
        manager.SetPlayerTransform(this.playerTransform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
