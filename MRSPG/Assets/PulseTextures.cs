using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseTextures : MonoBehaviour
{
    [SerializeField] Material[] materials;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Material mat in materials)
        {
            mat.SetFloat("_BPMInterval", ((float)Metronome.inst.BPM / 60) - (Metronome.inst.framesToSkip * Time.fixedDeltaTime));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
