using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

/// <summary>
/// Stores telemetry data locally. Useful if there is no connection to a server, but we still want to track
/// telemetry data.
/// 
/// This should not be used every time; it will create new CSV files for the data, which can use up storage on the headset.
/// For this reason, if the game cannot connect to the server, the player should have to opt-in to storing data locally.
/// (Also this way, we can confirm where the data is going, and we can ignore data entirely if playing for fun/demo).
/// 
/// Data is stored in the persistentDataPath/SessionDataLogs/[sessionID].csv
/// For locating the persistentDataPath, see https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html
///
/// @author Alex Wills
/// @date 12 November 2023
/// </summary>
/// Resources:
/// Creating a directory: https://learn.microsoft.com/en-us/dotnet/api/system.io.directory.createdirectory?view=net-7.0 
/// Appending to the end of a file by moving the cursor to the file's end: https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.seek?view=net-7.0#system-io-filestream-seek(system-int64-system-io-seekorigin)
/// FileStream (used to write): https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream?view=net-7.0
/// FileInfo (represents the file): https://learn.microsoft.com/en-us/dotnet/api/system.io.fileinfo?view=net-7.0
public class LocalDataManager
{
    private static string sessionPath;  // Filepath for the data storage
    private static FileInfo sessionFile;    // Representation of the file

    /// <summary>
    /// Initializes local data storage by creating a new CSV file with a sessionID and writing the header.
    /// </summary>
    public static void CreateLocalSession() {
        
        // Create directory for storing data, if it doesn't yet exist
        sessionPath = Application.persistentDataPath + "/SessionDataLogs/";
        if (!Directory.Exists(sessionPath)) {
            Directory.CreateDirectory(sessionPath);
        }

        // Now create a new file for telemetry
        TelemetryManager.session = GenerateSessionID();
        sessionPath += TelemetryManager.session + ".csv";
        
        sessionFile = new FileInfo(sessionPath);
        

        if (!sessionFile.Exists) {
            using FileStream stream = sessionFile.Create();
        }

        // Add the header for the csv file
        using (FileStream stream = sessionFile.OpenWrite()) {
            stream.Write(System.Text.Encoding.UTF8.GetBytes("name,time,vec,textContent,intContent\n"));
        }
       
    }

    /// <summary>
    /// Saves the current list of TelemetryEntry objects to this session's CSV file.
    /// </summary>
    public static void SendPayload() {
        using (FileStream stream = sessionFile.OpenWrite()) {
            stream.Seek(0, SeekOrigin.End);

            // Write all of the telemetry entries
            foreach (TelemetryEntry entry in TelemetryManager.entries) {
                stream.Write(System.Text.Encoding.UTF8.GetBytes(String.Format("{0},{1},{2},{3},{4}\n", entry.name, entry.time, entry.vec, entry.textContent, entry.intContent)));
            }

            // Empty the entry list for future entries
            TelemetryManager.entries.Clear();
        }
    }

    
    /// <summary>
    /// Generate a session ID, used for naming the file where this data will be stored.
    /// SessionID should be unique and non-identifying.
    /// </summary>
    /// <returns> A unique and non-identifying session ID </returns>
    private static string GenerateSessionID() {
        return "test";
    }
}
