using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VolumeSlider : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Toggle toggle;

    private void Start()
    {
        slider.SetValueWithoutNotify(ClientSettings.volume);
        toggle.SetIsOnWithoutNotify(ClientSettings.useGyro);
    }

    public void SlideVolume(Single volume)
    {
        ClientSettings.volume = (int)volume;
    }

    public void GyroMode(bool mode)
    {
        ClientSettings.useGyro = mode;
    }

    public void Graphics(int mode)
    {

    }
}
