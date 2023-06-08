using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Alternative to the OVRPlayerController prefab. This player controller takes in input from the left thumbstick
/// and moves the player accordingly.
/// You can customize the movement speed.
/// In the future, this script could be expanded to include more aspects like the OVRPlayerController (jumping, rotation with the right stick,
/// stepped-speed, etc.)
/// 
/// This script exists to allow us to use a rigidbody with the player for our own settings (in particular, turning gravity off for underwater movement).
/// 
/// For this script to work, the "Player", with a collider and rigidbody, must not be in a parent-child hierarchy with the OVRCameraRig
/// Recommended hierarchy:
/// > Player
///   > Player Controlle (attach this script here)
///   > OVRCameraRig
///   
/// You also must set up the cameraRig and forwardDirection through the inspector.
/// 
/// @author Alex Wills
/// Updated: 6/14/2022
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class NewPlayerController : MonoBehaviour
{
    [Header("OVR Objects (Must be set up)")]

    [Tooltip("Camera rig tracking the headset and controllers")]
    [SerializeField] private GameObject cameraRig;

    [Tooltip("Center Eye Anchor of the camera rig / Transform containing the direction the player is facing")]
    [SerializeField] private Transform forwardDirection;

    [Header("Movement Settings")]

    [Tooltip("Top speed for the player")]
    [SerializeField] private float movementSpeed = 5f;

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
    /// Every physics step, ensure that the camera rig is following the player collider and respond to player input. 
    /// </summary>
    private void FixedUpdate()
    {
        // Read the left thumbstick
        OVRInput.FixedUpdate();
        Vector2 inputDirection = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);

        // Move the rigidbody and have the camera rig follow along
        rigidbody.velocity = CalculateVelocity(inputDirection);
        cameraRig.transform.localPosition = this.transform.localPosition;
    }

    /// <summary>
    /// Converts 2D input into 3D movement.
    /// Specifically, it takes in a Vector2 with values from -1 to 1, and it uses
    /// this script's movementSpeed and the player's forward direction to create a Vector3 in world space
    /// representing the player's new velocity
    /// </summary>
    /// <param name="input"> Vector2 with values on [-1, 1] representing the joystick input </param>
    /// <returns> World space based Vector3 representing the player's velocity </returns>
    private Vector3 CalculateVelocity(Vector2 input)
    {

        
        Vector3 localVelocity = new Vector3(input.x * movementSpeed, rigidbody.velocity.y, input.y * movementSpeed);

        // Rotate local velocity around y axis to get the world space velocity (from https://gamedevbeginner.com/how-to-rotate-in-unity-complete-beginners-guide/#rotate_vector)
        Vector3 worldVelocity = Quaternion.Euler(0, forwardDirection.eulerAngles.y, 0) * localVelocity;

        return worldVelocity;
        
    }
}
