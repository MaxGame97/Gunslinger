using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{ 
    public Slider slider;

    public SliderButtonDisplayerScript button;

    private float value = 0;

    private void Start()
    {
        slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        ValueChangeCheck();
    }

    private void ValueChangeCheck()
    {
        value = slider.value;
        button.ChangeValue(value);
    }

    public float GetValue()
    {
        return value;
    }
}
