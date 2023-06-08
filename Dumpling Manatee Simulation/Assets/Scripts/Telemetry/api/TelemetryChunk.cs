using System.Collections.Generic;

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