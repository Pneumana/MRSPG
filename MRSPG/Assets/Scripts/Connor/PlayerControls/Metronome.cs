using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Metronome : MonoBehaviour
{
    public float BPM;
    public float beatDuration;
    public int BeatsPassed;

    [SerializeField]float interval;
    [SerializeField]float intervalHalfPoint;
    [SerializeField] float forgivness;

    public Image visualizer;
    bool onBeat;

    public static Metronome inst;

    private void Awake()
    {
        BeatsPassed = 0;
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
        intervalHalfPoint = Mathf.MoveTowards(intervalHalfPoint, 0, Time.deltaTime);
        visualizer.GetComponent<RectTransform>().sizeDelta = (50 + (interval * 50)) * Vector2.one;

        if (intervalHalfPoint == 0)
        {
            intervalHalfPoint = (1 / ((float)BPM / 60)) * forgivness;
            //Debug.Log(intervalHalfPoint);

            if (onBeat)
            {
                BeatsPassed++;
                onBeat = false;
                visualizer.color = Color.red;
            }
            else
            {
                BeatsPassed++;
                onBeat = true;
                visualizer.color = Color.green;
            }
        }
        if (interval == 0)
        {
            interval = 1 / ((float)BPM / 60);
        }
            

        
    }
    //this makes it so in any scripts we need to check for the player being on beat, we just need to do Metronome.inst.IsOnBeat()
    //which returns a bool of if the metronome is currently on an offbeat or an on beat


    public bool IsOnBeat()
    {
        //calculate exactly how on beat the player is
        return onBeat;
    }
    public float GetInterval()
    {
        return 1 / ((float)BPM / 60);
    }
}
