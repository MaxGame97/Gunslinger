using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderScript : MonoBehaviour {

    public Slider VolumeSlider;

    public VolumeButtonDisplayerScript button;

    public float volume = 70;

	public void SlidingTheVolumeSlider(int newVolume)
    {
        volume = newVolume;
    }

    private void Start()
    {
        VolumeSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    private void ValueChangeCheck()
    {
        volume = VolumeSlider.value;
        button.ChangeValue(volume);
    }
}
