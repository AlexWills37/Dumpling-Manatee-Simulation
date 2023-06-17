using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract action to implement for manatee behaviors
/// </summary>
public abstract class AbstractAction {

    protected ManateeBehavior manatee;

    protected Rigidbody manateeRb;

    protected Animator manateeAnimator;

    protected IEnumerator coroutine = null;

    public AbstractAction(ManateeBehavior manatee) {
        this.manatee = manatee;
        this.manateeRb = manatee.gameObject.GetComponent<Rigidbody>();
        this.manateeAnimator = manatee.gameObject.GetComponentInChildren<Animator>();
    }



    /// <summary>
    /// Begin this action's coroutine.
    /// </summary>
    public void StartAction() {
        coroutine = ActionCoroutine();
        manatee.StartCoroutine(coroutine);
    }

    /// <summary>
    /// Called when this action's coroutine is complete to notify the manatee script
    /// that the action is complete.
    /// </summary>
    protected void OnComplete() {
        manatee.EndCurrentAction();
    }

    /// <summary>
    /// Interrupts the current action to allow for a new action to occur.
    /// 
    /// Start by calling manatee.StopCoroutine(coroutine);
    /// then get the manatee ready for its next action,
    /// and finish by calling this.OnComplete();
    /// </summary>
    public abstract void ForceEnd();

    /// <summary>
    /// Performs the manatee's action. 
    /// 
    /// At the end of this coroutine, call this.OnComplete().
    /// </summary>
    /// <returns> IEnumerator representing the coroutine </returns>
    protected abstract IEnumerator ActionCoroutine();

}

