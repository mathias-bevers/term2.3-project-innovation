using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSlider : MonoBehaviour
{
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
