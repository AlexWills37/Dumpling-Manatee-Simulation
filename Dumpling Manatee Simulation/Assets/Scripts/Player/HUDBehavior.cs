using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Heads Up Display behavior.
/// The HUD should follow the player around at a fixed distance, following the player's horizontal rotation roughly.
/// Meant to mimic how windows in the Quest environment (like when you are setting up the guardian boundary) work.
/// The HUD only moves when the player rotates past a certain point.
/// This follows the player's gaze, instead of sticking to the player's gaze.
/// 
/// HOW TO USE:
/// Attach this script to a game object, and make sure that this game object's pivot point is on the player (the script will match their positions, so it doesn't
/// have to be exact).
/// To set the HUD up, make sure that this game object has the same rotation (in world space) as the player (in world space).
/// Then attach the HUD objects to the same game object (as children), and set them up (distance, rotation, position) how you want them to be relative to the player.
/// 
/// TECHNICAL EXPLANATION:
/// This script will cause the attached game object's Y rotation to match the player's Y rotation.
/// By having this game object as a pivot point, and attaching HUD game objects at a distance as children, the HUD will seem to move around the player.
/// 
/// @author Alex Wills
/// Updated 6/11/2022
/// </summary>
public class HUDBehavior : MonoBehaviour
{
    private Transform player;

    [Tooltip("How many degrees away from the center of the HUD before it rotates to match the player's rotation")]
    [SerializeField] private float angleWindow = 10f;

    [Tooltip("Time (in seconds) for the HUD to rotate to match the player's rotation")]
    [SerializeField] private float timeToRecenter = 0.3f;

    [Tooltip("Curve to specify how the HUD will rotate to follow the player. Values should range from 0 to 1.")]
    [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    // Remember the starting angles to keep the X and Z angles consistent
    private Vector3 startingAngles;

    // We will only move the HUD if it is currently still
    private bool stationary = true;


    // Start is called before the first frame update
    void Start()
    {
        // Get the left eye anchor to detect player rotation
        player = GameObject.Find("LeftEyeAnchor").transform;

        startingAngles = this.transform.rotation.eulerAngles;

    }


    private void LateUpdate()
    {
        // Fix the position to the player
        this.transform.position = player.position;

        // Only consider moving if the coroutine is not active
        if (stationary)
        {
            // Determine if the player is looking away from the HUD and if we should rotate the HUD
            float deltaY = CalculateDeltaY(this.transform.eulerAngles.y, player.transform.eulerAngles.y);
            
            // If the player is looking too far away, rotate to catch up
            if(Mathf.Abs(deltaY) > angleWindow)
            {
                StartCoroutine(RotateHUD(deltaY));
            }
        }

        //// Fix the y rotation to the player
        //this.transform.rotation = Quaternion.Euler(startingAngles.x, player.rotation.eulerAngles.y, startingAngles.z);
    }

    /// <summary>
    /// Calculate the shortest way to rotate from currentY to targetY.
    /// This method takes into consideration the possibility of wrapping around and rotating past 0 / 360 degrees.
    /// 
    /// Note: There might be a more efficient math-y way to do this method, but this approach makes sense to me (alex).
    /// </summary>
    /// <param name="currentY"> the starting Y rotation </param>
    /// <param name="targetY"> the ending Y rotation </param>
    /// <returns> shortest change in degrees of rotation to get from currentY to targetY </returns>
    private float CalculateDeltaY(float currentY, float targetY)
    {
        float deltaY;

        // If you take a circle representing the possible angles, and lay the circle into a line from 0 to 360,
        // there are two points: currentY and targetY. We need to determine if it is shorter to wrap around and go
        // through 0/360 degrees, or if it is shorter to not wrap around.
        float shortestDistance = Mathf.Abs(targetY - currentY);

        bool wrapAroundZero = false;
        if(360 - shortestDistance < shortestDistance)
        {
            wrapAroundZero = true;
            shortestDistance = 360 - shortestDistance;
        }

        // If we are not wrapping around 0, change in rotation is just (end - start)
        if (!wrapAroundZero)
        {
            deltaY = targetY - currentY;
        } else
        {
            // We are wrapping around 0

            // We need to rotate counterclockwise / anticlockwise / in the negative direction if the target is greater than where we currently are
            if(targetY > currentY)
            {
                deltaY = shortestDistance * -1;

            // If the target is less than the current rotation, we will rotate clockwise / in the positive direction to wrap around and get to the target
            } else
            {
                deltaY = shortestDistance;
            }

        }

        return deltaY;
    }

    /// <summary>
    /// Rotate the HUD by a number of degrees.
    /// Rotation will follow the path specified by the rotation curve, over the course of the
    /// specified rotation time. At the end of this coroutine, the HUD will be rotated deltaY degrees.
    /// </summary>
    /// <param name="deltaY"> Number of degrees to rotate the HUD </param>
    /// <returns> IEnumerator for a coroutine </returns>
    private IEnumerator RotateHUD(float deltaY)
    {
        // Tells the script that the Coroutine is happening
        stationary = false;

        float totalTime = 0f;
        float startingRotation = this.transform.eulerAngles.y;
        float currentY;

        // Animate until the time has elapsed
        while(totalTime < timeToRecenter)
        {
            totalTime += Time.deltaTime;

            // Set the y rotation based on the time elapsed, location on the curve, deltaY, and starting position
            currentY = startingRotation;
            currentY += deltaY * rotationCurve.Evaluate(totalTime / timeToRecenter);

            // Change the rotation
            this.transform.eulerAngles = new Vector3(startingAngles.x,
                                                    currentY,
                                                    startingAngles.z);
            yield return null;
        }

        // After the time has passed, snap rotation to the completed position and end the coroutine
        this.transform.eulerAngles = new Vector3(startingAngles.x,
                                                    startingRotation + deltaY,
                                                    startingAngles.z);
        stationary = true;
    }


}
