using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvTrigger : MonoBehaviour
{
    public List<IEnvTriggered> triggers = new List<IEnvTriggered>();
    [Header("Set to -1 to use Metronome delay")]
    public float delay;
    public bool onlyOnBeat;

    private void Start()
    {
        //add all child objects with IEnvTriggered to list
        foreach(IEnvTriggered child in transform.GetComponentsInChildren<IEnvTriggered>())
        {
            triggers.Add(child);
        }
        if(delay == -1)
        {
            delay = Metronome.inst.GetInterval();
        }
    }

    [ContextMenu("Trigger All")]
    public void TriggerAll()
    {
        if(delay == 0)
        {
            foreach (IEnvTriggered t in triggers)
            {
                t.Activated(delay);
            }
        }
        else
        {
            StartCoroutine(TriggerLoop());
        }
    }
    IEnumerator TriggerLoop()
    {
        if (onlyOnBeat)
        {
            yield return new WaitForSeconds(Metronome.inst.interval);
        }
        int loops = 0;
        if(triggers.Count > 0)
        {
            do
            {

                triggers[loops].Activated(delay);
                loops++;
                yield return new WaitForSeconds(delay);
            } while (loops < triggers.Count);
        }
        
        yield return null;
    }
}
