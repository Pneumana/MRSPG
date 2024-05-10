using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Application;

public class ChangeSettingData : MonoBehaviour
{
    bool awakeCheck;

/*    private void Awake()
    {
        SceneManager.sceneLoaded += LoadCallback;
    }
    void LoadCallback(Scene scene, LoadSceneMode sceneType)
    {
        awakeCheck = false;
    }*/
    public void ChangeSetting()
    {


            Debug.Log("allowed because its not awake");
            SettingsDataHolder.inst.pulseIntensity = transform.GetComponentInChildren<Slider>().value;
    }
}
