using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseTextures : MonoBehaviour
{
    [SerializeField] Material[] materials;
    [SerializeField] int materialFrameSkips;
    [SerializeField] float delta = 0.02f;
    // Start is called before the first frame update
    void Start()
    {
        Debug.LogWarning(1 / Time.deltaTime + " FPS");
        foreach(Material mat in materials)
        {
            mat.SetFloat("_BPMInterval", ((float)Metronome.inst.BPM / 60) - (materialFrameSkips * delta));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
