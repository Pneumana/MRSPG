using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatHandler : MonoBehaviour
{
    [SerializeField] private AudioSource track;
    [SerializeField] private float bpm;
    [SerializeField] private Intervals[] intervals;
}

[System.Serializable]
public class Intervals
{
    [SerializeField] private float beats;

    public float FindIntervals(float bpm)
    {
        return 60f / (bpm * beats);
    }
}
