using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Metronome : MonoBehaviour
{
    public float BPM;
    public float beatDuration;
    public int BeatsPassed;

    public float interval;
    public float intervalMax;

    [SerializeField]float intervalHalfPoint;
    [SerializeField] float forgivness;

    [SerializeField] Sprite beatIcon;

    public Image visualizer;
    public bool onBeat;

    public static Metronome inst;

    TextMeshProUGUI onbeatDebug;

    public Vector3 startPos;

    [SerializeField] public int framesToSkip;
    [Header("Not a direct 1:1 for number of beats displayed")]
    [SerializeField] float beatPopulation;


    public float beatPercent;
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
        startPos = transform.localPosition;
        intervalMax = GetInterval();
        try
        {

            onbeatDebug = transform.Find("DebugText").GetComponent<TextMeshProUGUI>();
        }
        catch { }
    }

    private void FixedUpdate()
    {
        GetComponent<Image>().color = new Color(1,1,1, Mathf.MoveTowards(GetComponent<Image>().color.a, 0, Time.deltaTime * 5));


        interval = Mathf.MoveTowards(interval, 0, Time.deltaTime);
        intervalHalfPoint = Mathf.MoveTowards(intervalHalfPoint, 0, Time.deltaTime);
        //visualizer.GetComponent<RectTransform>().sizeDelta = (50 + (interval * 50)) * Vector2.one;

/*        if (interval / (GetInterval() / 2) >= 1 - forgivness || interval / (GetInterval() / 2) <= forgivness)
        {
            onBeat = true;
            GetComponent<Image>().color = new Color(0, 1, 0);
        }
        else
        {
            onBeat = false;
            GetComponent<Image>().color = new Color(1, 1, 1);
        }*/

        if ((intervalHalfPoint - (framesToSkip * Time.deltaTime)) <= 0)
        {
            intervalHalfPoint = (1 / ((float)BPM / 60)) * 0.5f;
        }
        if ((interval - (framesToSkip * Time.deltaTime)) <= 0)
        {
            interval = 1 / ((float)BPM / 60);
            BeatsPassed++;
            //create animation components
            StartCoroutine(AnimateBeats(interval));
        }


        GetComponent<AudioSource>().pitch = Time.timeScale;
        if (onbeatDebug != null)
        {
            if (onBeat)
            {
                onbeatDebug.text = "On Beat";
            }
            else
                onbeatDebug.text = "OFF Beat";
        }
    }
    private void Update()
    {
        beatPercent = Mathf.Abs((2 * interval / intervalMax)- 1);
        onBeat = beatPercent >= 1 - forgivness;
    }
    IEnumerator AnimateBeats(float _interval)
    {
        _interval *= beatPopulation;
        float t = 0;

        var left = new GameObject();
        left.name = "LeftBeat";
        left.AddComponent<Image>();

        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        var x = 50;
        var y = 100;

        var ls = left.GetComponent<Image>();
        ls.sprite = beatIcon;

        bool flip = true;

        //left.AddComponent<RectTransform>();
        left.GetComponent<RectTransform>().sizeDelta = new Vector2(x * canvas.scaleFactor, y * canvas.scaleFactor);
        left.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);
        left.transform.SetParent(transform);

        //onBeat = false;

        var right = new GameObject();
        right.name = "RightBeat";
        right.AddComponent<Image>();

        var rs = right.GetComponent<Image>();
        rs.sprite = beatIcon;

        //right.AddComponent<RectTransform>();
        right.GetComponent<RectTransform>().sizeDelta = new Vector2(x * canvas.scaleFactor, y * canvas.scaleFactor);
        right.GetComponent<RectTransform>().localScale = new Vector2(-1, 1);
        right.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);
        right.transform.SetParent(transform);

        do
        {
            t += Time.deltaTime;

/*            if (t > (_interval * forgivness) && flip == false)
            {
                
            }*/

                var percent = t / _interval;
            ls.color = new Color(1, 1, 1, Mathf.Clamp01(percent + 0.25f));
            rs.color = new Color(1, 1, 1, Mathf.Clamp01(percent + 0.25f));

            left.GetComponent<RectTransform>().anchoredPosition = Vector3.left * (200 - (Mathf.Clamp01(percent) * 200));
            right.GetComponent<RectTransform>().anchoredPosition = Vector3.right * (200 - (Mathf.Clamp01(percent) * 200));

                //right.GetComponent<RectTransform>().sizeDelta = new Vector2(x * canvas.scaleFactor, y * canvas.scaleFactor);
                //left.GetComponent<RectTransform>().sizeDelta = new Vector2(x * canvas.scaleFactor, y * canvas.scaleFactor);
            if(t > _interval - (_interval * forgivness) && flip == true)
            {
                flip = false;
                //onBeat = true;
                //ls.color = new Color(1, 0, 1, 1);
                //rs.color = new Color(1, 0, 1, 1);
            }
            yield return new WaitForSeconds(0);
        } while (t < _interval);
        t = 0;
        do
        {
            t += Time.deltaTime;
            var percent = t / (_interval * forgivness);
            //onBeat = true;
            ls.color = new Color(1, 1, 1, 1 - Mathf.Clamp01(percent));
            rs.color = new Color(1, 1, 1, 1 - Mathf.Clamp01(percent));
            //right.GetComponent<RectTransform>().sizeDelta = new Vector2(x * canvas.scaleFactor, y * canvas.scaleFactor);
            //left.GetComponent<RectTransform>().sizeDelta = new Vector2(x * canvas.scaleFactor, y * canvas.scaleFactor);

            yield return new WaitForSeconds(0);
        } while (t < _interval * forgivness);
        //this forgiveness used to be 0.25f

        //onBeat = false;
        Destroy(left);
        Destroy(right);

        yield return null;
    }
    //this makes it so in any scripts we need to check for the player being on beat, we just need to do Metronome.inst.IsOnBeat()
    //which returns a bool of if the metronome is currently on an offbeat or an on beat


    public bool IsOnBeat(bool isplayer = false)
    {
        /*
        //max inter
        var intHalf = (GetInterval() / 2);

        Debug.Log("top interval :" + (intHalf - (intHalf * forgivness)) + " bottom interval: " + ((intHalf * (forgivness))));

        onBeat = false;
        if (intervalHalfPoint < (intHalf * forgivness))
        {
            Debug.Log(intervalHalfPoint + ": bottom interval was used");
            onBeat = true;
        }
        else if(intervalHalfPoint > (intHalf * (1 - forgivness)))
        {
            Debug.Log(intervalHalfPoint + ": top interval was used");
            onBeat = true;
        }*/

        if (onBeat && isplayer)
            GetComponent<Image>().color = new Color(1, 1, 1, 1);
        return onBeat;
    }
    public float GetInterval()
    {
        return 1 / ((float)BPM / 60);
    }
}
