using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * This script is changing the scene when the player collides with an apple.
 * It is attached to the apple object in the "Be a Manatee" Scene.
 * 
 * @author Sami Cemek
 * @author Alex Wills
 * Updated: 6/20/22
 */

public class ChangeScene : MonoBehaviour
{

    /// <summary>
    /// Load a scene using this script. Can be called from Unity Events, for example.
    /// </summary>
    /// <param name="sceneIndex"> the scene to transition to. </param>
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    /// <summary>
    /// Load the next scene in order of the build settings.
    /// If the game is at the last scene, it should wrap back around to the first scene.
    /// </summary>
    public void LoadNextScene()
    {
        // Go to the next scene in the build settings
        int sceneNumber = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;        
        SceneManager.LoadScene(sceneNumber);        
    }
}