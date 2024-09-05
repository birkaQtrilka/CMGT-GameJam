using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    Slider slider;
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate { ChangeMasterVolume(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMasterVolume()
    {
        SoundManager.main.ChangeMasterVolume(slider.normalizedValue);
    }
}
