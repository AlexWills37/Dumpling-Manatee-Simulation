using System.Collections.Generic;

/// <summary>
/// Groups of information to create payloads to send from the headset to the backend server.
/// 
/// TelemetryChunk:
///     - ChunkHeader - contains identifying info for the simulation instance sending the data
///         - platform - string representing the device platform (based on the API's .getPlatform() method, useful for comparing the simulation on different devices).
///         - simulation - string holding the simulation name. This is set in TelemetryManager (useful for keeping data separate between projects).
///         - version - string to indicate the version number. Set in this file.
///         - session - string to indicate the instance of this simulation. This is the most important part!! This is how the database is separated into multiple sessions.
///     - entries - List of TelemetryEntry objects
///         TelemetryEntry is defined in TelemetryEntry.cs:
///         - name - name of the entry event, used to classify what the data represents
///         - time - system time of when the entry is created
///         - other data - Vec3, string, int: extra data to include in the entry.
/// 
/// Commented by Alex Wills
/// @author Ender Fluegge
/// @datea 11 November 2023
/// </summary>

/// <summary>
/// Holds information about the simulation and the sessionID to organize data.
/// </summary>
[System.Serializable]
public class ChunkHeader {
    public string platform;
    public string simulation;
    public string version;
    public string session;

    public ChunkHeader(string platform, string simulation, string version, string session) {
        this.platform = platform;
        this.simulation = simulation;
        this.version = version;
        this.session = session;
    }
}

/// <summary>
/// Payload to send to the database. Contains a ChunkHeader with metainformation and a list of TelemetryEntry objects
/// with the recorded data.
/// </summary>
[System.Serializable]
public class TelemetryChunk {
    public ChunkHeader header;
    public List<TelemetryEntry> entries;

    public TelemetryChunk(string session, string simulation) {
        this.header = new ChunkHeader(TelemetryApi.getPlatform(), simulation, "1.0.0", session);
        this.entries = new List<TelemetryEntry>();
    }

    public void concat(List<TelemetryEntry> entries) {
        this.entries.AddRange(entries);
    }
}