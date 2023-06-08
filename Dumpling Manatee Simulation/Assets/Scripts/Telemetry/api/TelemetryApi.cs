using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;

public class TelemetryApi {
    public static bool isInternetConnected() {
        return true;
    }

    public static IEnumerable<bool> testInternetConnected() {
        // Submit a request to google.com, returning true if it was successful, and false otherwise.
        UnityWebRequest www = UnityWebRequest.Get("http://google.com");
        yield return www.SendWebRequest().isDone;
        bool isConnected = www.isNetworkError || www.isHttpError;
        yield return isConnected;
    }

    public static string getPlatform() {
        // if (Application.platform == RuntimePlatform.Android) {
        //     return "Cardboard";
        // } else {
        //     return "Quest2";
        // }
        return SystemInfo.deviceType.ToString();
    }
}   