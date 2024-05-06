using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsDataHolder : MonoBehaviour
{
    public static SettingsDataHolder inst;

    public float pulseIntensity;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(inst);
        }
        else
        {
            if(inst!=this)
                Destroy(gameObject);
        }
        
        SceneManager.sceneLoaded += LoadCallback;
    }

    void LoadCallback(Scene scene, LoadSceneMode sceneType)
    {
        Debug.Log("load game callback");
        if (GameObject.FindFirstObjectByType<ChangeSettingData>() != null)
        {
            Debug.Log("found change setting data script");
            var set = FindFirstObjectByType<ChangeSettingData>();
            set.transform.GetChild(1).GetComponent<Slider>().value = pulseIntensity;
        }
    }
}
