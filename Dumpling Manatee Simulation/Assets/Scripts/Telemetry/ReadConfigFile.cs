using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Xml.Serialization;

/// <summary>
/// Helper class for reading a local config file.
/// Currently this config file is only used for finding the backend server URL,
/// so the class is specialized for this function. Future work includes making this
/// system more general to work for any config file.
/// 
/// If the config file/its fields do not exist, this system creates them.
/// 
/// @author Alex Wills
/// @date 23 March, 2024
/// </summary>
public class ReadConfigFile
{
    /// <summary>
    /// Finds/creates the backendURL: field in Config/config.txt
    /// </summary>
    /// <returns></returns>
    public static string GetConnectionURL()
    {
        string url = "";

        // Create the directory for the config if it doesn't exist
        string directory = Application.persistentDataPath + "/Config/";
        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }

        string configPath = directory + "config.txt";
        Debug.Log("Config file at: " + configPath);


        // Open/create and read the file
        FileInfo configFile = new FileInfo(configPath);
        bool foundKey = false;
        using (FileStream stream = configFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)) {
            using StreamReader reader = new(stream);
            string line;

            // Look for the URL
            while (!foundKey && ((line = reader.ReadLine()) != null))
            {
                if (line.Contains("backendURL:"))
                {
                    url = GetValueFromLine(line, "backendURL:");
                    foundKey = true;
                }
            }
        }
        // If the key doesn't exist, write a placeholder in the file for future use
        if (!foundKey) {
            using StreamWriter writer = new(configPath, true, System.Text.Encoding.UTF8);
            writer.WriteLine("backendURL:");
        }

        return url;
    }

    /// <summary>
    /// Returns the substring of text following the "key" in the "line".
    /// </summary>
    /// <param name="line">The string to search</param>
    /// <param name="key">The text to search for</param>
    /// <returns>The remainder of the string after the key</returns>
    private static string GetValueFromLine(string line, string key) 
    {
        string value = "";
        int keyIndex = line.IndexOf(key);
        if (keyIndex != -1) {
            int startIndex = keyIndex + key.Length;
            value = line.Substring(startIndex);
        }
        return value;
    }

}
