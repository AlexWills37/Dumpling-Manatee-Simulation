using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 
///
/// </summary>
/// 
/// https://learn.microsoft.com/en-us/dotnet/api/system.io.directory.createdirectory?view=net-7.0 

public class LocalDataManager
{
    private static string sessionPath;
    private static FileInfo sessionFile;
    public static void CreateLocalSession() {
        Debug.Log("Creating local session");
        
        // Create directory for storing data, if it doesn't yet exist
        sessionPath = Application.persistentDataPath + "/SessionDataLogs/";
        if (!Directory.Exists(sessionPath)) {
            DirectoryInfo di = Directory.CreateDirectory(sessionPath);
        }

        // Now create a new file for telemetry
        sessionPath += GenerateSessionID() + ".csv";
        

        sessionFile = new FileInfo(sessionPath);
        if (!sessionFile.Exists) {
            sessionFile.Create();
        }

        // Add the header for the csv file

        using (FileStream stream = sessionFile.OpenWrite()) {
            stream.Write(System.Text.Encoding.UTF8.GetBytes("name,time,vec,textContent,intContent\n"));
        }
       
    }

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

    

    private static string GenerateSessionID() {
        return "test";
    }
}
