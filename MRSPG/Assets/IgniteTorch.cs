using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IgniteTorch : MonoBehaviour, IEnvTriggered
{
    [SerializeField]
    ParticleSystem flames;
    [SerializeField]
    Light glow;
    private void Start()
    {
        flames.Stop();
        glow.enabled = false;
    }
    public void Activated(float delay)
    {
        
        StartCoroutine(Ignite());
    }
    // Update is called once per frame
    IEnumerator Ignite()
    {
        float t = 0;
        glow.enabled = true;
        var start = glow.intensity;
        flames.Play();
        while (t < 1)
        {
            t += Time.deltaTime;
            glow.intensity = Mathf.Lerp(0, start, t);
            yield return new WaitForSeconds(0);
        }

        yield return null;
    }
}
