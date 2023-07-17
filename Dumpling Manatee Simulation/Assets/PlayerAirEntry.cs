using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAirEntry : MonoBehaviour
{
    public UnityEvent airEntry;
    public UnityEvent airExit;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Air"))
        {
            airEntry.Invoke();
            RenderSettings.fog = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Air"))
        {
            airExit.Invoke();
            RenderSettings.fog = true;

        }
    }
}
