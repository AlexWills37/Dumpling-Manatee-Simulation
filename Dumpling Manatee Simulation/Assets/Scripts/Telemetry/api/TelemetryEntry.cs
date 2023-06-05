using System.Collections.Generic;
using System;

[System.Serializable]
public class TelemetryEntry {
    public string name;
    public Vec3 vec = null;
    public string textContent = null;
    public int intContent = 0;

    public long time = DateTimeOffset.Now.ToUnixTimeMilliseconds();

    public TelemetryEntry(string name, Vec3 vec) {
        this.name = name;
        this.vec = vec;
    }
    
    public TelemetryEntry(string name, string content) {
        this.name = name;
        this.textContent = content;
    }

    public TelemetryEntry(string name, string content, int intContent) {
        this.name = name;
        this.textContent = content;
        this.intContent = intContent;
    }

    public TelemetryEntry(string name, int intContent) {
        this.name = name;
        this.intContent = intContent;
    }

    public TelemetryEntry(string name) {
        this.name = name;
    }
}