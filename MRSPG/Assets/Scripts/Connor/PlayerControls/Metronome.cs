using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Metronome : MonoBehaviour
{
    public float BPM;
    public float beatDuration;

    float interval;
    float intervalHalfPoint;

    public Image visualizer;
    bool onBeat;

    public static Metronome inst;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {

        interval = Mathf.MoveTowards(interval, 0, Time.deltaTime);
        visualizer.GetComponent<RectTransform>().sizeDelta = (50 + (interval * 50)) * Vector2.one;
        if (interval == 0)
        {
            interval = 1/((float)BPM / 60);
            intervalHalfPoint = (float)BPM / 60;
            if (onBeat)
            {
                onBeat = false;
                visualizer.color = Color.red;
            }
            else
            {
                onBeat = true;
                visualizer.color = Color.green;
            }
        }
    }
    //this makes it so in any scripts we need to check for the player being on beat, we just need to do Metronome.inst.IsOnBeat()
    //which returns a bool of if the metronome is currently on an offbeat or an on beat
    public bool IsOnBeat()
    {
        return onBeat;
    }
    public float GetInterval()
    {
        return 1 / ((float)BPM / 60);
    }
}
