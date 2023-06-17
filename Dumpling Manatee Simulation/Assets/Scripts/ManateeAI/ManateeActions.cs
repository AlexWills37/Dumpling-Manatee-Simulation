/// <summary>
/// This file contains the implementations for different manatee actions.
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
    /// Swim forward for a random amount of time.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator ActionCoroutine() {
        
        manateeAnimator.SetBool("isSwimming", true);
        // Set velocity forward for a bit of time
        manateeRb.velocity = manatee.transform.forward * movementSpeed;
        manateeRb.drag = 0;

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