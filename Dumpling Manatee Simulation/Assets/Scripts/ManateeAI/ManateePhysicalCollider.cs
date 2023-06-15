using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to detect collisions and triggers exclusively with the manatee itself (as
/// opposed to the manatee's object detection collider).
/// 
/// The player's personal space collider should NOT have the Player tag.
/// 
/// @author Alex Wills
/// Updated 6/22/22
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ManateePhysicalCollider : MonoBehaviour
{

    [Tooltip("The manatee object this script is attached to.")]
    [SerializeField] private ManateeBehavior manatee;

    [Tooltip("Collider trigger indicating the player's personal space to not invade.")]
    private Collider playersPersonalSpace;

    void Awake()
    {
        if(playersPersonalSpace == null)
        {
            playersPersonalSpace = GameObject.Find("Personal Space").GetComponent<Collider>();
        }
    }

    /// <summary>
    /// When the manatee enters the player's personal space, let the Manatee script know.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("COLLIDER ENTERED: " + other.gameObject.tag);
        
        
        // Know when the manatee is in the player's personal space
        if (other == playersPersonalSpace)
        {
            manatee.SetInPlayerSpace(true);

        // If the player collides, count that as a boop
        } else if (other.gameObject.CompareTag("Player"))
        {
            manatee.InteractWithManatee();  

        // If the manatee is breathing air, set the surface status
        } else if (other.gameObject.CompareTag("Air"))
        {
            manatee.SetAtSurface(true);
        }
    }

    /// <summary>
    /// When the manatee exits the player's personal space, let the Manatee script know.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        // Know when the manatee is out of the player's personal space
        if (other == playersPersonalSpace)
        {
            manatee.SetInPlayerSpace(false);

        // Detect when the manatee is no longer at the surface
        } else if (other.gameObject.CompareTag("Air"))
        {
            manatee.SetAtSurface(false);
        }
    }
}
