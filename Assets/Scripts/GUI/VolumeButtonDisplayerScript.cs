using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeButtonDisplayerScript : MonoBehaviour {

    [SerializeField]
    private int currentValue = 70;

    public Text valueText;

    public void ChangeValue(float value)
    {
        currentValue = (int)value;
        valueText.text = currentValue.ToString();
    }
}
