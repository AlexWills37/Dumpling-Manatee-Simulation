using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ManateeSchoolManager : MonoBehaviour
{
    [SerializeField] private SlideDeck presentation;
    
    // Start is called before the first frame update
    void Start()
    {
        presentation.AddEventOnFinalSlide(FinalSlide);    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FinalSlide() {
        presentation.SetButtonActive(false);
        presentation.SetButtonText("Lesson complete!");
    }
}
