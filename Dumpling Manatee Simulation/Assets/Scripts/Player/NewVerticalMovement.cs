using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Alternative to Sami's VerticalMovement.cs.
/// Uses the X and Y buttons on the Quest controller to move the player vertically by adjusting the player's velocity.
/// To be used with NewPlayerController.cs for full underwater movement.
/// 
/// With this script enabled, the player will not be affected by vertical movement. The player will either move up, down, or be stationary.
/// 
/// @author Alex Wills
/// Updated 6/14/2022
/// </summary>
public class NewVerticalMovement : MonoBehaviour
{
    private bool movingDown = false;
    private bool movingUp = false;

    [Tooltip("How fast the player should move vertically")]
    [SerializeField] private float verticalSpeed = 5f;

    private new Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Every physics step, move the player vertically if necessary.
    /// </summary>
    private void FixedUpdate()
    {
        OVRInput.FixedUpdate();
        movingUp = OVRInput.Get(OVRInput.RawButton.Y);
        movingDown = OVRInput.Get(OVRInput.RawButton.X);

        float verticalVelocity = 0;

        // Determine how to change the physics
        // If both buttons or neither button is pressed, do not move vertically
        if(movingDown == movingUp)
        {
            verticalVelocity = 0;
        } else if (movingDown)
        {
            verticalVelocity = -1;
        } else
        {
            verticalVelocity = 1;
        }

        rigidbody.velocity = new Vector3(rigidbody.velocity.x,
                                        verticalVelocity * verticalSpeed,
                                      rigidbody.velocity.z);
    }
}
