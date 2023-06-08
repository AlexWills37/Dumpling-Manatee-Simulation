using System.Collections;
using UnityEngine;

// Import UnityWebRequest
using UnityEngine.Networking;

public class WebManager {
    public static IEnumerator SendPayload() {

        if (TelemetryApi.isInternetConnected()) {
            var chunk = new TelemetryChunk(TelemetryManager.session, "Twizzler");
            chunk.concat(TelemetryManager.entries);
            TelemetryManager.entries.Clear();

            using (UnityWebRequest webRequest = new UnityWebRequest(TelemetryManager.url + "telemetry/payload")) {
                webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(chunk)));
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.method = UnityWebRequest.kHttpVerbPOST;
                yield return webRequest.SendWebRequest();
            }
        } else {
            var currentData = "";
            if (PlayerPrefs.HasKey("telemetry")) {
                currentData = PlayerPrefs.GetString("telemetry");
            }
                // Add to local storage
                PlayerPrefs.SetString(
                "telemetry",
                currentData + "|=|" + JsonUtility.ToJson(new TelemetryChunk(TelemetryManager.session, "Twizzler"))
            );
        }
    }
}