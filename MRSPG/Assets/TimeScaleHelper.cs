using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeScaleHelper : MonoBehaviour
{
     void Update()
    {
        GetComponent<TextMeshProUGUI>().text = "Time Scale is " + Time.timeScale;
    }
}
