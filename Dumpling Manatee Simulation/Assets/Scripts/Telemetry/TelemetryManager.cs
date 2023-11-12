using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor.EditorTools;


/// <summary>
/// Persistent script (transfers between scenes) to:
/// - initially connect with the backend server
/// - track what the player is looking at
/// - collect telemetry entries
/// - periodically send data to the backend server (or locally)
/// </summary>
/// 
/// If possible, this script will connect to a backend server to send data.
///     Data is sent online with WebManager.cs
///     The backend server can be found at: https://github.com/AlexWills37/Dumpling-Backend-Server
///     and the URL to connect to this server must be set in this file.
/// Otherwise, the player can click a button to enable data saving locally.
///     Data is saved locally with LocalDataManager.cs
/// 
/// Tha MongoDB cluster is organized into databases for each simulation.
/// Within each database is a collection for each platform data comes from.
/// 
/// Credits go to Ender Fluegge for the backend server and telemetry scripting.
/// 
/// Possible future tasks:
///     - Separate tracking the player's gaze into a new file (for modularity)
///     - Separate the status indicators into a new file (for modularity) 
/// 
/// @author Ender Fluegge
/// @author Alex Wills
/// @date 11 November 2023
public class TelemetryManager : MonoBehaviour {
    public static string url = "http://localhost/";     // The address of the backend server to connect to (see file header)

    public static string simulationName = "Dumpling";   // String to organize database by simulation


    [Header("Status indication")]

    [Tooltip("The color of this image will change to indicate the telemetry status.")]
    [SerializeField] private Image telemetryStatusIcon;

    [Tooltip("Text to display the telemetry status (connecting, online, not connecte, saving locally).")]
    [SerializeField] private TextMeshProUGUI telemetryStatusText;

    [Tooltip("If the game cannot connect to the server, this button will appear to allow the user to switch to saving data locally. Othewrwise, data will not be saved.")]
    [SerializeField] private Button enableLocalTelemetryButton;
     
    public static TelemetryManager instance;    // Singleton instance to ensure only 1 connection per game

    private LayerMask objectsToIgnore;   // Layer mask for telling the raycast what to ignore

                                        
    public static List<TelemetryEntry> entries = new List<TelemetryEntry>();    // Buffer of telemetry entries to send to the server
    public static string session = "";  // Session ID



    private Transform playerTransform;  // Reference to the player's transform (for detecting what the player is looking at)
                                        // This will change between scenes, since the player changes between scenes
    private string lookingAtTarget = "";    // Name of the current object the player is looking at (that is being tracked)
    private float lookingAtTime = 0f;   // Timer to store how long the player is looking at the samed object (that is being tracked)
    private int frameCount = 0; // Counter for sending data periodically to the server

    /// <summary>
    /// Where telemetry data should be sent.
    /// Online: send data to TelemetryManager.url with the WebManager script.
    /// Local: save data on the device with the LocalDataManager script.
    /// Delete: do not store data anywhere.
    /// </summary>
    enum TelemetryDestination {Online, Local, Delete};
    private TelemetryDestination telemetryDestination = TelemetryDestination.Delete;    // By default, do not save data

    /// <summary>
    /// Before anything else, initialize this script and connect to the server
    /// </summary>
    private void Awake() {
        
        // Ensure there is only one instance of the telemetry manager between scenes
        if (instance != null) {
            Destroy(this);
        } else {
            instance = this;
            StartCoroutine(Initialize());   // Connect to the server (or try to)
            DontDestroyOnLoad(gameObject);  // Preserve this script and game object between scenes
        }

        // Create the layer mask for detecting player's gaze
        this.objectsToIgnore = ~LayerMask.NameToLayer("RecordPlayerLookingAt"); // Ignore every layer except for the one our objects to track are in

        // Add overhead to SceneChange events to reconfigure for new scenes.
        SceneManager.sceneUnloaded += OnSceneUnload;
        SceneManager.sceneLoaded += OnSceneLoad;

        // Find the player's camera
        playerTransform = GameObject.Find("LeftEyeAnchor").transform;
        if(playerTransform == null) {
            Debug.LogError("Could not find the player's camera (LeftEyeAnchor).");
        }
    }

    /// <summary>
    /// Attempts to connect to the backend server. Success/error is printed to the debug console, as
    /// well as the status indicators, if configured in the inspector. If the server connection fails,
    /// activate the button for the player to choose to save data locally instead. 
    /// </summary>
    /// <returns> IEnum representation of this coroutine </returns>
    private IEnumerator Initialize() {
        Debug.Log("Initilizing telemetry");
        
        // Display status message if possible
        telemetryStatusText?.SetText("Connecting to server...");
        enableLocalTelemetryButton?.gameObject.SetActive(false);
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(TelemetryManager.url + "session/new")) {    // Create a web request
            yield return webRequest.SendWebRequest();   // Send the web request, waiting for the response
            
            // After receiving response, validate it
            try
            {
                // Parse the json response
                var jsonResponse = JsonUtility.FromJson<CreateSessionResponse>(webRequest.downloadHandler.text);
                Debug.Log("jsonResponse: " + jsonResponse.data.session);
                
                // Set the session id
                TelemetryManager.session = jsonResponse.data.session;
                Debug.Log("Telemetry successfully initialized");

                telemetryDestination = TelemetryDestination.Online; // Set flag to indicate successful connection

                
                // Display status if possible
                telemetryStatusText?.SetText("Connected to server!");
                if (telemetryStatusIcon != null) {
                    telemetryStatusIcon.color = Color.green;
                }

            // If something goes wrong, print an error and switch to saving data locally (if the player chooses to)
            } catch
            {
                Debug.LogError("An error occured with telemetry. Either could not connect with server, or the server's response could not be processed correctly");

                // Display status if possible
                telemetryStatusText?.SetText("Could not connect to server.");
                if (telemetryStatusIcon != null) {
                    telemetryStatusIcon.color = Color.red;
                }

                // Enable the backup button for the user to decide to save data locally
                if (enableLocalTelemetryButton != null) {
                    enableLocalTelemetryButton.gameObject.SetActive(true);
                    
                    /// <summary>
                    /// When the button is pressed, switch to saving data locally.
                    /// </summary>
                    enableLocalTelemetryButton.onClick.AddListener( () => {

                        // Destroy button to prevent further clicks
                        Destroy(enableLocalTelemetryButton.gameObject);

                        // Initialize the new local session
                        LocalDataManager.CreateLocalSession();
                        telemetryDestination = TelemetryDestination.Local; // Enable flag to direct where telemetry will be sent

                        // Update display status
                        telemetryStatusText?.SetText("Saving data locally.");
                        if (telemetryStatusIcon != null) {
                            telemetryStatusIcon.color = Color.green;
                        }
                    });
                } // End of backup button use
            } // End of try-catch
        } // Delete the web request object
    }

    /// <summary>
    /// Processes telemetry every frame. 
    /// </summary>
    private void Update() {

        // For debugging
        if (Input.GetKeyDown(KeyCode.Z)) {
            int sceneNumber = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
            SceneManager.LoadScene(sceneNumber);
        }


        frameCount++;   // Update the frame counter (we will not send data on every frame)

        /* Raycast to find what the user is looking at */ {

            // Make sure the player transform is active before doing the raycast
            if(playerTransform != null)
            {
                // Create a ray from the player's gaze.
                Ray ray = new Ray(playerTransform.position, playerTransform.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, 100, objectsToIgnore)) {   // The ray will travel 100 units and ignore all objects not in the "RecordPlayerLookingAt" layer
                    
                    if (hit.transform.gameObject.name == lookingAtTarget) { // If this is the same object from the last frame, add to the timer
                        lookingAtTime += Time.deltaTime;
                    } else {

                        // If we are looking at a new object, record how long we were looking at the previous object
                        TelemetryManager.entries.Add(
                            new TelemetryEntry("lookingAt", lookingAtTarget, ((int)(lookingAtTime * 1000)) )
                        );  // Record the time the player was looking at the game object, in milliseconds

                        // Reset the target and timer to start recording the next object we are looking at
                        lookingAtTarget = hit.transform.gameObject.name;
                        lookingAtTime = 0f;

                    }
                }
                // If the player is not looking at an object being tracked, the script "pauses" the timer.
                // This means that if the player looks at a textbox for 5 seconds, then nothing for 3 seconds, and the same textbox for 2 seconds,
                // a single entry will be sent: lookingAt the textbox for 7 seconds.
            }
        }

        // Every 600 frames, send the current TelemetryEntries either online, locally, or nowhere.
        if (frameCount % 600 == 0)
            switch (telemetryDestination) {
                case TelemetryDestination.Online:
                    StartCoroutine(WebManager.SendPayload());
                    break;
                case TelemetryDestination.Local:
                    LocalDataManager.SendPayload();
                    break;
                case TelemetryDestination.Delete:
                    entries.Clear();
                    break;
            }
    }


    /// <summary>
    /// Creates a telemetry entry for the completion of a scene.
    /// This depends on Unity unloading the current scene before loading the next one (so that timeSinceLevelLoad is accurate).
    /// Subscribe this function to SceneManager.sceneUnloaded.
    /// </summary>
    /// <param name="sceneEnding"> The scene being unloaded, completed by the player </param>
    private void OnSceneUnload(Scene sceneEnding) {
        // Add a telemetry entry for the scene change
        entries.Add( new TelemetryEntry("sceneCompleted", sceneEnding.name, (int)Time.timeSinceLevelLoad) );
    }

    /// <summary>
    /// Finds the player's camera in the new scene for tracking the player's gaze.
    /// </summary>
    /// <param name="newScene"> The new scene being loaded (not used) </param>
    /// <param name="mode"> Scene load mode (not used) </param>
    private void OnSceneLoad(Scene newScene, LoadSceneMode mode) {
        playerTransform = GameObject.Find("LeftEyeAnchor").transform;
        if(playerTransform == null) {
            Debug.LogError("Could not find the player's camera (LeftEyeAnchor).");
        }
    }

}