using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Force a game object to stay at a single Y position for its entire lifetime.
/// Attached to the Sun Rays particle effect.
/// The particle effect is a child of the OVR camera, so that the sunrays follow the player's position.
/// This script allows the particles to follow the player, but not change their Y position.
/// 
/// @author Alex Wills
/// Updated 7/2/22
/// </summary>
public class MaintainHeight : MonoBehaviour
{
    private float constantY;
    

    // Start is called before the first frame update
    void Start()
    {
        constantY = this.transform.position.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position = new Vector3(this.transform.position.x,
                                                constantY,
                                                this.transform.position.z);
    }
}
