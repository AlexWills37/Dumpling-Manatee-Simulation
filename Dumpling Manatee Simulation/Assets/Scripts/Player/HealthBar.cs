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
/// Updated: 08/20/21
/// 
/// </summary>

public class HealthBar : MonoBehaviour
{

    public Slider slider;
    public Gradient gradient;
    public Image fill;

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
