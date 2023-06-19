/// <summary>
/// This file contains the implementations for different manatee actions.
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
    private float movementSpeed;
    public ManateeSwim(ManateeBehavior manatee, float movementSpeed) : base(manatee) {
        this.movementSpeed = movementSpeed;
    }

    /// <summary>
    /// Force the manatee to stop swimming.
    /// 
    /// The manatee will still drift forward for a bit.
    /// </summary>
    public override void ForceEnd()
    {
        manatee.StopCoroutine(coroutine);
        // Add drag to slow the manatee down
        manateeRb.drag = 1;
        manateeAnimator.SetBool("isSwimming", false);
        this.OnComplete();
    }


    /// <summary>
    /// Swim for a random amount of time.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator ActionCoroutine() {

        bool swimBackwards = (Random.Range(0, 2) < 1);

        manateeAnimator.SetBool("isSwimming", true);
        // Set velocity forward for a bit of time
        manateeRb.velocity = manatee.transform.forward * movementSpeed;
        manateeRb.drag = 0;

        if (swimBackwards) {
            manateeRb.velocity = manatee.transform.forward * movementSpeed * -1;
        }

        // Swim for a random amount of time
        yield return new WaitForSeconds(Random.Range(1, 5));

        // Come to a slow stop by adding drag for a bit of time
        manateeAnimator.SetBool("isSwimming", false);
        manateeRb.drag = 1;
        yield return new WaitForSeconds(4);
        this.OnComplete();
    }
}

/// <summary>
/// The manatee will wait for a random amount of time, doing nothing.
/// </summary>
public class ManateeWait : AbstractAction
{
    public ManateeWait(ManateeBehavior manatee) : base(manatee) {}
    public override void ForceEnd()
    {
        manatee.StopCoroutine(coroutine);
        this.OnComplete();
    }

    protected override IEnumerator ActionCoroutine()
    {
        yield return new WaitForSecondsRealtime(Random.Range(1,5));
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
    }

    public override void ForceEnd()
    {
        manatee.StopCoroutine(coroutine);

        // Make sure the animator is set correctly
        manateeAnimator.SetBool("isBreathing", false);

        this.OnComplete();
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
    }

    public override void ForceEnd()
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