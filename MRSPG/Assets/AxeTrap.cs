using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AxeTrap : MonoBehaviour
{
    float bpm;
    float t;
    bool activated;

    public float minRotation;
    public float maxRotation;

    [SerializeField]int loopsToTrigger;
    [SerializeField] int loopsToStop;
    [SerializeField] GameObject pivot;
    [SerializeField] Transform axehead;
    [SerializeField] Image warn;
    [SerializeField] LineRenderer chain;

    float startZ;

    float targetWarnFill;
    int loops;
    Vector3 spikepos;

    // Start is called before the first frame update
    void Start()
    {
        bpm = (1 / ((float)Metronome.inst.BPM / 60));
        spikepos = pivot.transform.position;
        startZ = pivot.transform.rotation.eulerAngles.z;
        chain.SetPosition(0, spikepos);
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
            StartCoroutine(Activate(maxRotation));
        }
        if(loops == loopsToStop && activated)
        {
            activated = false;
            loops = 0;
            //activate
            StartCoroutine(Activate(minRotation));
        }
    }
    IEnumerator Activate(float input)
    {
        axehead.GetComponent<Collider>().enabled = true;
        Quaternion currentRot = pivot.transform.rotation;
        Quaternion targetRot = Quaternion.Euler(new Vector3(pivot.transform.rotation.eulerAngles.x, pivot.transform.rotation.eulerAngles.y, input));

        float currentZ = pivot.transform.rotation.eulerAngles.z;
        float t = 0;
        do
        {
            t += Time.deltaTime;
            Debug.Log((t / bpm) * 5);
            var lerp = Mathf.Lerp(currentZ, input, (t / bpm) * 5);
            //lerp = Mathf.Clamp(lerp, 0f, 1f);
            Debug.Log((t / bpm) * 5 + " clamped to " + lerp);
            pivot.transform.rotation = Quaternion.Slerp(currentRot, targetRot, (t / bpm) * 5);
            chain.SetPosition(1, axehead.position);
            yield return new WaitForSeconds(0);
        } while (t < bpm / 5);

        axehead.GetComponent<Collider>().enabled = false;

        yield return null;
    }
    IEnumerator Deactivate()
    {
        /*float raiseAmount = 0;
        do
        {
            raiseAmount += Time.deltaTime;
            spikes.transform.position = spikepos + (transform.up * 0.4f) - (transform.up * (raiseAmount / bpm)) * 0.4f;
            yield return new WaitForSeconds(0);
        } while (raiseAmount < bpm);*/
        yield return null;
    }
}
