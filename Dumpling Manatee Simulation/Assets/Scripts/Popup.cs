using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField]
    GameObject popupIcon;
    [SerializeField]
    GameObject popupCanvas;
    [SerializeField]
    float popupDuration;
    GameObject physicalPlayer;

    private void Start()
    {
        physicalPlayer = GameObject.Find("PhysicalPlayer");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "FlipperCollider")
        {
            popupIcon.SetActive(false);
            popupCanvas.SetActive(true);
            transform.LookAt(physicalPlayer.transform);
            StartCoroutine("unPopup");
        }
    }

    private IEnumerator unPopup()
    {
        yield return new WaitForSeconds(popupDuration);
        popupIcon.SetActive(true);
        popupCanvas.SetActive(false);
    }
}
