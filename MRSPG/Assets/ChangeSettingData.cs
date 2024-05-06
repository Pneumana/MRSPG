using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSettingData : MonoBehaviour
{
    bool awakeCheck;
    public void ChangeSetting()
    {
        if(awakeCheck == true)
            SettingsDataHolder.inst.pulseIntensity = FindFirstObjectByType<Slider>().value;
        else
            awakeCheck = true;
    }
}
