/// <summary>
/// This file contains the implementations for different manatee actions:
/// Swim
/// Wait
/// Breathe
/// Play
/// ChangeDirection
/// 
/// @author Alex Wills
/// @date 6/19/2023
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This action defines the manatee randomly swimming forward.
/// </summary>
public class ManateeSwim : AbstractAction
{
    private float movementSpeed, rotationSpeed;
    protected bool isSwimming = false;
    protected IEnumerator swimmingCoroutine;

    public ManateeSwim(ManateeBehavior manatee, float movementSpeed, float rotationSpeed) : base(manatee) {
        this.movementSpeed = movementSpeed;
        this.rotationSpeed = rotationSpeed;
    }

    /// <summary>
    /// Force the manatee to stop swimming.
    /// 
    /// The manatee will still drift forward for a bit.
    /// </summary>
    protected override void ForceEnd()
    {
        // Add drag to slow the manatee down
        manateeRb.drag = 1;
        if (isSwimming) {
            manatee.StopCoroutine(swimmingCoroutine);
        }
        manateeAnimator.SetBool("isSwimming", false);
    }


    /// <summary>
    /// Swim for a random amount of time.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator ActionCoroutine() {

        // Choose time to swim
        float swimTime = Random.Range(1f, 5f);
        // Choose rotation to take
        float maxRotation = 45;
        float rotationDifference = Random.Range(-maxRotation, maxRotation);


        // Choose height difference
        float heightDifference = Random.Range(-3f, 3f);
        // If manatee is currently at the surface, try to swim deeper
        if (manatee.atSurface) {
            heightDifference = -3;
        }


        // Move the manatee forward with rigidbody velocity
        // Ease into swim speed
        AnimationCurve velocityStartup = AnimationCurve.EaseInOut(0, 0, 1, 1);  // Smoothe curve
        manateeAnimator.SetBool("isSwimming", true);
        float elapsedTime = 0;
        // Go from time = 0 to time = the end of the startup curve
        while(elapsedTime < velocityStartup.keys[velocityStartup.length - 1].time && !manatee.inPersonalSpace) {    // Stop early if the manatee enters the player's personal space
            
            manateeRb.velocity = manatee.transform.forward * velocityStartup.Evaluate(elapsedTime) * movementSpeed;
            yield return null;
            elapsedTime += Time.deltaTime;
        }
        // At full speed now, turn off drag for continuous movement
        manateeRb.drag = 0;
        // manateeRb.velocity = manatee.transform.forward * movementSpeed;

        // For the swim time, swim forward, slowly swim up/down, and rotate
        elapsedTime = 0;
        float elapsedRotation = 0;
        float deltaRotation;

        // Swim until: manatee enters personal space OR manatee swims for the swimTime and completes its rotation
        while ((elapsedTime < swimTime || elapsedRotation < Mathf.Abs(rotationDifference)) && !manatee.inPersonalSpace) {    // Stop early if the manatee enters the player's personal space
            elapsedTime += Time.deltaTime;

            // Only change height if we are going down, or if we are going up and are not at the surface
            if (heightDifference <= 0 || !manatee.atSurface) {
                manatee.transform.Translate(0, heightDifference * Time.deltaTime / swimTime, 0, Space.World);

                // If we reach the surface, and the height difference is positive (this else statement)...
            } else {
                // ...swap the vertical direction to swim down
                heightDifference *= -1;
            }

            // Only change rotation if we have not finished rotating
            if (elapsedRotation < Mathf.Abs(rotationDifference)) {
                // Rotate the manatee kinematically (with the transform, not the rigidbody)
                deltaRotation = Mathf.Sign(rotationDifference) * Time.deltaTime * rotationSpeed;
                manatee.transform.Rotate(0, deltaRotation, 0, Space.World);
                elapsedRotation += Mathf.Abs(deltaRotation);

                // Update its velocity to match rotation
                manateeRb.velocity = manatee.transform.forward * movementSpeed;
            }

            yield return null;
        }


        // Finished swimming
        manateeRb.drag = 2; // Adding drag will cause the manatee to slow down to a stop
        manateeAnimator.SetBool("isSwimming", false);
        yield return new WaitForSeconds(1);
        
        this.OnComplete();
    }

}

/// <summary>
/// The manatee will wait for a random amount of time, doing nothing.
/// </summary>
public class ManateeWait : AbstractAction
{
    public ManateeWait(ManateeBehavior manatee) : base(manatee) {}
    protected override void ForceEnd()
    {
    }

    protected override IEnumerator ActionCoroutine()
    {
        // Wait between 1 and 5 seconds 
        yield return new WaitForSecondsRealtime(Random.Range(1f, 5f));
        this.OnComplete();
    }
}

/// <summary>
/// Swim to the surface and breathe for a little bit, before returning back down.
/// </summary>
public class ManateeBreathe : AbstractAction
{
    private float movementSpeed;
    public ManateeBreathe(ManateeBehavior manatee, float speed) : base(manatee)
    {
        movementSpeed = speed;
        this.interruptable = false;
    }

    protected override void ForceEnd()
    {
        // Make sure the animator is set correctly
        manateeAnimator.SetBool("isBreathing", false);
    }

    protected override IEnumerator ActionCoroutine()
    {
        // Stop moving and record the y position so we can return to this point
        manateeRb.velocity = Vector3.zero;
        float originalY = manatee.transform.position.y;

        // Swim to the surface
        while (!manatee.atSurface) {
            manatee.transform.Translate(Vector3.up * movementSpeed * Time.deltaTime, Space.World);
            yield return null;
        }

        // At the surface, take a breath
        // Start the breath animation
        manateeAnimator.SetBool("isBreathing", true);
        
        // Wait for the breathing animation to start
        while (!manateeAnimator.GetCurrentAnimatorStateInfo(0).IsTag("breathing")) {
            yield return null;
        }

        // The breathing animation has started        
        manateeAnimator.SetBool("isBreathing", false);

        // Wait until the breathing animation is complete
        while (manateeAnimator.GetCurrentAnimatorStateInfo(0).IsTag("breathing")) {
            yield return null;
        }


        // Swim back down to the original point
        float currentY = manatee.transform.position.y;
        while (manatee.transform.position.y > originalY)
        {
            manatee.transform.Translate(Vector3.down * movementSpeed * Time.deltaTime, Space.World);
            yield return null;

            // Stop swimming early if we are not moving (if there is an obstacle in our way)
            if (manatee.transform.position.y == currentY) {
                break;
            } else {
                currentY = manatee.transform.position.y;
            }
        }

        // Reset the manatee's breath component
        manatee.RefillBreath();

        this.OnComplete();
    }
}


/// <summary>
/// When the player interacts with the manatee, play a happy animation.
/// </summary>
public class ManateePlay : AbstractAction
{
    private ParticleSystem.EmissionModule happyParticles;
    public ManateePlay(ManateeBehavior manatee, ParticleSystem.EmissionModule particles) : base(manatee)
    {
        this.happyParticles = particles;
        this.interruptable = false;
    }

    protected override void ForceEnd()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator ActionCoroutine()
    {
        // Begin the rolling animation
        manateeAnimator.SetBool("isInteracting", true);
        // Wait until the animation begins
        while (!manateeAnimator.GetCurrentAnimatorStateInfo(0).IsTag("interaction")) {
            yield return null;
        }

        // Emit particles until the animation ends
        happyParticles.rateOverTime = 3;
        manateeAnimator.SetBool("isInteracting", false);
        while (manateeAnimator.GetCurrentAnimatorStateInfo(0).IsTag("interaction")) {
            yield return null;
        }

        happyParticles.rateOverTime = 0;
        
        this.OnComplete();
    }
}

/// <summary>
/// Rotate to face another direction
/// </summary>
public class ManateeChangeDirection : AbstractAction
{
    private float rotationSpeed;
    public ManateeChangeDirection(ManateeBehavior manatee, float rotationSpeed) : base(manatee)
    {
        this.rotationSpeed = rotationSpeed;

    }

    protected override IEnumerator ActionCoroutine()
    {
        // Choose a rotation between 90 and 180 degrees, in any direction
        float rotationChange = Random.Range(80f, 140f);
        if (Random.Range(0f, 1f) < 0.5) {
            rotationChange *= -1;
        }

        // Complete the rotation
        float elapsedRotation = 0;
        float deltaRotation;
        while (elapsedRotation < Mathf.Abs(rotationChange)) {
            deltaRotation = Time.deltaTime * (rotationSpeed + 5) * Mathf.Sign(rotationChange);    // Rotate based on rotation speed and the direction of the rotation
            manatee.transform.Rotate(0, deltaRotation, 0, Space.World);
            elapsedRotation += Mathf.Abs(deltaRotation);
            yield return null;
        }

        // End the coroutine
        this.OnComplete();
    }

    protected override void ForceEnd()
    {
        // Nothing extra needs to occur; the rotation will just stop
    }
}