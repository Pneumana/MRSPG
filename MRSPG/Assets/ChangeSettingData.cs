using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSettingData : MonoBehaviour
{
    public void ChangeSetting()
    {
        SettingsDataHolder.inst.pulseIntensity = FindFirstObjectByType<Slider>().value;
    }
}
