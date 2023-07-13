using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Popup : MonoBehaviour
{
    [SerializeField]
    GameObject popupIcon;
    [SerializeField]
    GameObject popupCanvas;
    [SerializeField]
    float popupDuration;
    private bool showing = false;
    GameObject physicalPlayer;
    private float timer;
    public UnityEvent onPopupInteraction = new UnityEvent();

    private void Start()
    {
        physicalPlayer = GameObject.Find("PhysicalPlayer");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "FlipperCollider")
        {
            onPopupInteraction.Invoke();
            popupIcon.SetActive(false);
            popupCanvas.SetActive(true);
            timer = 0;
            showing = true;
        }
    }
    private void Update()
    {
        if (timer > popupDuration)
        {
            timer = 0;
            showing = false;
            popupIcon.SetActive(true);
            popupCanvas.SetActive(false);
        }
        else if (showing)
        {
            timer += Time.deltaTime;
            transform.LookAt(physicalPlayer.transform);
        }
    }

    private IEnumerator unPopup()
    {
        yield return new WaitForSeconds(popupDuration);
        popupIcon.SetActive(true);
        popupCanvas.SetActive(false);
    }
}
