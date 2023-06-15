using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this script to the manatee's trigger.
/// This script works with ManateeBehavior2 to detect what objects are in this manatee's
/// range (to keep the detection trigger seperate from the rigidbody collider).
/// 
/// The player's personal space collider should NOT have the Player Tag.
/// 
/// @author Alex Wills
/// Updated 6/22/22
/// </summary>
public class ManateeVisionTrigger : MonoBehaviour
{
    [Tooltip("The manatee object this script is attached to.")]
    [SerializeField] private ManateeBehavior manatee;

    /// <summary>
    /// When the player is in this collider's range, tell the manatee script to follow the player.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            manatee.SetPlayerFollow(true);
        }
    }

    /// <summary>
    /// When the player leaves the collider's range, stop following the player.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            manatee.SetPlayerFollow(false);
        }
    }
}
