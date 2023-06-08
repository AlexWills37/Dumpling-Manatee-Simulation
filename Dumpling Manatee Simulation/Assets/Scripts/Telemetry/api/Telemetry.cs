using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Let it be deserialized into json
[System.Serializable]
public class TelemetryProperty {
    public string name;
    public object value;

    // constructor
    public TelemetryProperty(string name, object value) {
        this.name = name;
        this.value = value;
    }
}

[System.Serializable]
public class Telemetry {
    
}