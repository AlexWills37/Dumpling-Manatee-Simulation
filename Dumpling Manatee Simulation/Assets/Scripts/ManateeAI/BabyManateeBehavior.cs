using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Baby manatees will follow their parent
/// 
/// @author Alex Wills
/// Updated 7/11/2022
/// </summary>
public class BabyManateeBehavior : ManateeBehavior
{
    [Tooltip("This manatee's parent to follow")]
    [SerializeField] private GameObject parentManatee;

    [Tooltip("How close the manatee should be to its parent")]
    [SerializeField] private float followDistance = 5f;

    private Vector3 distanceFromParent;

    // // Start is called before the first frame update
    // new void Start()
    // {
    //     // Ensure that the base manatee behavior is setup correctly
    //     base.Start();
    //     this.manateeRb.drag = 0.5f;
    // }

    // /// <summary>
    // /// This method is where the baby manatee makes different decisions than the parent manatee
    // /// </summary>
    // override protected void Update()
    // {
    //     distanceFromParent = parentManatee.transform.position - this.transform.position;

    //     // If the manatee is far from the parent, move towards the parent
    //     if(distanceFromParent.magnitude > followDistance)
    //     {
    //         this.manateeRb.velocity = distanceFromParent.normalized * this.movementSpeed;
    //     }

    //     // Match the parent's rotation
    //     this.transform.rotation = parentManatee.transform.rotation;
        
    // }
}
