using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassInitScript : MonoBehaviour
{
    [SerializeField]
    private string startDirection;

    public int GetShapeKeyRotation()
    {
        switch (startDirection)
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
                throw new System.Exception("Bad Grass Rotation");
        }
       
    }

 
}
