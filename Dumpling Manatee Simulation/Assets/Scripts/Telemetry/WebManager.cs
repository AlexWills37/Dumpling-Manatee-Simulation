using System.Collections;
using UnityEngine;

// Import UnityWebRequest
using UnityEngine.Networking;

/// <summary>
/// 

/// @author Ender Fluegge
/// </summary>
public class WebManager {

    /// <summary>
    /// Sends the current list of TelemetryEntries to the backend server and clears the list.
    /// Connects to the server using this game instance's sessionID
    /// </summary>
    /// <returns> IEnum representation of the coroutine </returns>
    public static IEnumerator SendPayload() {

            // Build a payload to send, including the session ID, simulation name, and data entries
            var chunk = new TelemetryChunk(TelemetryManager.session, TelemetryManager.simulationName);
            chunk.concat(TelemetryManager.entries);
            TelemetryManager.entries.Clear();   // Empty the list for future entries

            // Send the web request
            using (UnityWebRequest webRequest = new UnityWebRequest(TelemetryManager.url + "telemetry/payload")) {
                webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(chunk))); // Add the payload to the request
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.method = UnityWebRequest.kHttpVerbPOST;
                yield return webRequest.SendWebRequest();   // Send actual request to the server
            }
    }
}