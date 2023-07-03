using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Force this game object to copy the position of another game object.
/// 
/// @author Alex Wills
/// Updated 6/15/2023
/// </summary>
public class FollowObject : MonoBehaviour
{



    [Tooltip("The game object to follow")]
    [SerializeField] private Transform toFollow;

    [SerializeField] private bool matchRotation = true;
    [SerializeField] private bool followXPosition = true;
    [SerializeField] private bool followYPosition = true;
    [SerializeField] private bool followZPosition = true;

    private bool matchPosition;

    // Start is called before the first frame update
    void Start()
    {

        // Verify there is an object to follow
        if (toFollow == null){
            Debug.LogError(this.gameObject.name + " has the FollowObject script, but does not have a target to follow.");
            matchRotation = false;
            followXPosition = false;
            followYPosition = false;
            followZPosition = false;
        }

        matchPosition = matchRotation && followXPosition && followYPosition && followZPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Copy the target's position if we are following everything
        if (matchPosition) {
            this.transform.position = toFollow.position;
        } else {
            // Otherwise, the new position should copy the selected values from the target and keep the unselected values the same
            Vector3 newPosition;
            newPosition.x = followXPosition ? toFollow.position.x : this.transform.position.x;
            newPosition.y = followYPosition ? toFollow.position.y : this.transform.position.y;
            newPosition.z = followZPosition ? toFollow.position.z : this.transform.position.z;
            this.transform.position = newPosition;
        }

        // If matching rotation, copy the rotation
        if (matchRotation) {
            this.transform.eulerAngles = toFollow.eulerAngles;    
        }
    }
}
