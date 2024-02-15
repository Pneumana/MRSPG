using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvTrigger : MonoBehaviour
{
    public List<IEnvTriggered> triggers = new List<IEnvTriggered>();

    public float delay;
    public bool onlyOnBeat;

    private void Start()
    {
        if(delay == -1)
        {
            delay = Metronome.inst.GetInterval();
        }
    }

    public void TriggerAll()
    {
        if(delay == 0)
        {
            foreach (IEnvTriggered t in triggers)
            {
                t.Activated();
            }
        }
        else
        {
            StartCoroutine(TriggerLoop());
        }
    }
    IEnumerator TriggerLoop()
    {
        int loops = 0;
        do
        {
            if (Metronome.inst.IsOnBeat() && onlyOnBeat || !onlyOnBeat)
            {
                triggers[loops].Activated();
                loops++;
            }
            yield return new WaitForSeconds(delay);
        }while(loops < triggers.Count);
        yield return null;
    }
}
