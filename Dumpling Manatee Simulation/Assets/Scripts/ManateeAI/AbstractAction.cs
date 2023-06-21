using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract action to implement for manatee behaviors.
/// To create a new action, extend this class and implement the following methods:
///     - constructor:
///         the constructor should call the base constructor with the ManateeBehavior parameter
///         example: public NewAction(ManateeBehavior manatee) : base(manatee) {}
///     - protected IEnumerator ActionCoroutine():
///         this is the coroutine that contains your action.
///         when the coroutine is finished, end the method with this.OnComplete()
///     - protected void ForceEnd():
///         this method is called externally to force the action coroutine to end.
///         the coroutine will be stopped before this method is called,
///         and this.OnComplete() will be called after this method.
///         Since the action coroutine will be stopped, this method should reset
///         any of the manatee's values to be prepared for the next action.
/// 
///         If you do not want the action to be interrupted, set
///         interruptable = false;
///         in the constructor, and ForceEnd() will never be called.
/// 
/// @author Alex Wills
/// @date 6/20/2023
/// </summary>
public abstract class AbstractAction {

    protected ManateeBehavior manatee;  // Manatee script

    protected Rigidbody manateeRb;  // Rigidbody for controlling manatee movement

    protected Animator manateeAnimator; // Animator for controlling manatee animation

    protected IEnumerator coroutine = null; // Stores the active coroutine

    // True if this action can be stopped with ForceEnd()
    protected bool interruptable = true;

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
    /// Attempts to stop the action. If the action is not interruptable (breathing, playing), 
    /// this function will return false.
    /// </summary>
    /// <returns> True if and only if the action is interruptable and stopped </returns>
    public bool StopAction() {
        if (interruptable) {
            manatee.StopCoroutine(coroutine);
            this.ForceEnd();
            this.OnComplete();
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Prepares the manatee to take its next action.
    /// This function can be called on interruptable actions at 
    /// any point during the action.
    /// </summary>
    protected abstract void ForceEnd();

    /// <summary>
    /// Performs the manatee's action. 
    /// 
    /// At the end of this coroutine, call this.OnComplete().
    /// </summary>
    /// <returns> IEnumerator representing the coroutine </returns>
    protected abstract IEnumerator ActionCoroutine();

}

