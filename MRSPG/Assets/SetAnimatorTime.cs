using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAnimatorTime : MonoBehaviour
{
    // Start is called before the first frame update
    public int scaler = 1;
    /*void OnEnable()
    {
        SetSpeed();
    }*/
    void Start()
    {
        SetSpeed();
    }
    void SetSpeed()
    {
        GetComponent<Animator>().SetFloat("IntervalSpeed", (Metronome.inst.BPM / 60) * scaler);
    }
}
