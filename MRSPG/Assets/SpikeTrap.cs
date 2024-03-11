using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpikeTrap : MonoBehaviour
{
    float bpm;
    float t;
    bool activated;
    [SerializeField]int loopsToTrigger;
    [SerializeField] int loopsToStop;
    [SerializeField] GameObject spikes;
    [SerializeField] Image warn;

    float targetWarnFill;
    int loops;
    Vector3 spikepos;

    // Start is called before the first frame update
    void Start()
    {
        bpm = (1 / ((float)Metronome.inst.BPM / 60));
        spikepos = spikes.transform.position; ;
    }

    // Update is called once per frame
    void Update()
    {
        t = Mathf.MoveTowards(t, 0, Time.deltaTime);
        if (t == 0)
        {
            t = bpm;
            loops++;

        }
        if (!activated)
        {
            targetWarnFill = (loops + 1f) / loopsToTrigger;
        }
        warn.fillAmount = Mathf.Lerp(warn.fillAmount, targetWarnFill, 0.25f);
        if (loops == loopsToTrigger && !activated)
        {
            activated = true;
            loops = 0;
            //activate
            StartCoroutine(Activate());
        }
        if(loops == loopsToStop && activated)
        {
            activated = false;
            loops = 0;
            //activate
            StartCoroutine(Deactivate());
        }
    }
    IEnumerator Activate()
    {
        float raiseAmount = 0;
        do
        {
            raiseAmount += Time.deltaTime;
            spikes.transform.position = spikepos + (transform.up * (raiseAmount/(bpm/5))) * 0.4f;
            yield return new WaitForSeconds(0);
        } while (raiseAmount < bpm / 5);
        yield return null;
    }
    IEnumerator Deactivate()
    {
        float raiseAmount = 0;
        do
        {
            raiseAmount += Time.deltaTime;
            spikes.transform.position = spikepos + (transform.up * 0.4f) - (transform.up * (raiseAmount / bpm)) * 0.4f;
            yield return new WaitForSeconds(0);
        } while (raiseAmount < bpm);
        yield return null;
    }
}
