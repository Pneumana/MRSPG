using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseTextures : MonoBehaviour
{
    [SerializeField] Material[] materials;
    [SerializeField] int materialFrameSkips;
    [SerializeField] float delta = 0.02f;

    [Range(0,0.7f)]
    [SerializeField] float eyebleed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [ContextMenu("Refresh")]
    public void Refresh()
    {
        //Debug.LogWarning(1 / Time.deltaTime + " FPS");
        //Debug.LogWarning("eyebleed setting is at " + eyebleed);
        foreach (Material mat in materials)
        {
            mat.SetFloat("_BPMInterval", ((float)Metronome.inst.BPM / 60) - (materialFrameSkips * delta));
            mat.SetFloat("_BPMIntensity", eyebleed);
        }
    }
}
