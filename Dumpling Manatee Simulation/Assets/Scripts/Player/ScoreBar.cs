using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary>
/// This script sets the max health and breath bar.
/// Also, it changes the bar’s slider value. These methods are 
/// accessed from another script.
///
/// @author Sami Cemek
/// @author Alex Wills
/// Updated: 08/20/21
/// Updated: 06/12/2023
/// 
/// </summary>

[RequireComponent(typeof(Slider))]
public class ScoreBar : MonoBehaviour
{

    private Slider slider;
    
    [Tooltip("The score meter's color gradient")]
    [SerializeField] private Gradient gradient;
    
    [Tooltip("The slider's fill object (for changing the meter's color)")]
    [SerializeField] private Image fill;


    void Start() 
    {
        slider = this.GetComponent<Slider>();
    }


    public void SetBarValue(float newValue) {
        slider.value = newValue;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public float GetBarValue() {
        return slider.value;
    }

    //HEALTH
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    //BREATH
    public void SetMaxBreath(float breath)
    {
        slider.maxValue = breath;
        slider.value = breath;

        fill.color = gradient.Evaluate(1f);
    }

    public void SetBreath(float breath)
    {
        slider.value = breath;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
