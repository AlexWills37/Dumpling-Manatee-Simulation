using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple script which holds the initialization variables and logic for every sea grass.
/// </summary>
public class GrassInitScript : MonoBehaviour
{
    [SerializeField] [Tooltip("x, -x, y, or -y")]
    private string startDirection; //the direction that the grass hsould start in

    /// <summary>
    /// gets the shape key that the grass is meant to start bending along
    /// </summary>
    /// <returns>returns an integer representing the index of the correct shape key</returns>
    public int GetShapeKeyRotation()
    {
        switch (startDirection) //simply convert the name of the axis to the correct shape key index
        {
            case "x":
                return 0;
            case "-x":
                return 1;
            case "y":
                return 2;
            case "-y":
                return 3;
            default:
                throw new System.Exception("Bad Grass Rotation"); //if an incorrect value has been input, throw an error
        }
       
    }

 
}
