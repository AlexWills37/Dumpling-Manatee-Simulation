using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script should be added to the physical player. It checks if the player is inside of the "Air Volume" and
/// calls the player's Breathe() method every frame if it is.
/// 
/// @author Alex Wills
/// @date 6/26/2023
/// </summary>
public class BreatheAir : MonoBehaviour
{
    private PlayerManager player;

    // Start is called before the first frame update
    void Start()
    {
        player = this.GetComponentInParent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Air")) {
            player.Breathe();
        }
    }
}
