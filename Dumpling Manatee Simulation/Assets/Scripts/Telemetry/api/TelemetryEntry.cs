using System.Collections.Generic;
using System;

/// <summary>
/// Representation of a data entry for the backend system.
/// 
/// Each Telemetry contains the following:
///     name - required string to describe the data entry
///     vec - optional vector information in the entry (default: null)
///     textContent - optional string information (default: null)
///     intContent - optional integer information (default: 0)
///     time - The system time (in milliseconds) at this entry's creation
/// 
/// Commented by Alex Wills
/// @author Ender Fluegge
/// @date 11 November 2023
/// </summary>
[System.Serializable]
public class TelemetryEntry {
    
    // Required information
    public string name;
    public long time = DateTimeOffset.Now.ToUnixTimeMilliseconds();

    // Optional information
    public Vec3 vec = null;
    public string textContent = null;
    public int intContent = 0;

    /* Constructors for different types of TelemetryEntries (depends on data types being recorded) */

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