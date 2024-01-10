using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaler : MonoBehaviour
{
    public float minTimeScale;
    public float maxTimeScale = 1;
    public float scaleSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            //slow down time
            Time.timeScale = Mathf.Lerp(Time.timeScale, minTimeScale, Time.unscaledDeltaTime * scaleSpeed);
        }
        else
        {
            //speed up time
            Time.timeScale = Mathf.Lerp(Time.timeScale, maxTimeScale, Time.unscaledDeltaTime * scaleSpeed);
        }
    }
}
