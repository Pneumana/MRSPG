using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsDataHolder : MonoBehaviour
{
    public static SettingsDataHolder inst;

    public float pulseIntensity;

    private void Awake()
    {
        if (inst != null)
        {
            inst = this;
            DontDestroyOnLoad(inst);
        }
        else
        {
            if(inst!=this)
                Destroy(gameObject);
        }
    }
}
