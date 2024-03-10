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

    [SerializeField] Sprite beatIcon;

    public Image visualizer;
    bool onBeat;

    public static Metronome inst;

    [SerializeField] public int framesToSkip;
    [Header("Not a direct 1:1 for number of beats displayed")]
    [SerializeField] float beatPopulation;

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

    private void FixedUpdate()
    {
        GetComponent<Image>().color = new Color(1,1,1, Mathf.MoveTowards(GetComponent<Image>().color.a, 0, Time.deltaTime * 5));


        interval = Mathf.MoveTowards(interval, 0, Time.deltaTime);
        intervalHalfPoint = Mathf.MoveTowards(intervalHalfPoint, 0, Time.deltaTime);
        //visualizer.GetComponent<RectTransform>().sizeDelta = (50 + (interval * 50)) * Vector2.one;

        if ((intervalHalfPoint - (framesToSkip * Time.deltaTime)) <= 0)
        {
            intervalHalfPoint = (1 / ((float)BPM / 60)) * forgivness;
            //Debug.Log(intervalHalfPoint);

            if (onBeat)
            {
                BeatsPassed++;
                onBeat = false;
                //visualizer.color = Color.red;
            }
            else
            {
                BeatsPassed++;
                onBeat = true;
                //visualizer.color = Color.green;
            }
        }
        if ((interval - (framesToSkip * Time.deltaTime)) <= 0)
        {
            interval = 1 / ((float)BPM / 60);
            //create animation components
            StartCoroutine(AnimateBeats(interval));
        }

        GetComponent<AudioSource>().pitch = Time.timeScale;
        
    }

    IEnumerator AnimateBeats(float _interval)
    {
        _interval *= beatPopulation;
        float t = 0;

        var left = new GameObject();
        left.name = "LeftBeat";
        left.AddComponent<Image>();

        var ls = left.GetComponent<Image>();
        ls.sprite = beatIcon;

        left.AddComponent<RectTransform>();
        left.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 100);
        left.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);
        left.transform.SetParent(transform);
        

        var right = new GameObject();
        right.name = "RightBeat";
        right.AddComponent<Image>();

        var rs = right.GetComponent<Image>();
        rs.sprite = beatIcon;

        right.AddComponent<RectTransform>();
        right.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 100);
        right.GetComponent<RectTransform>().localScale = new Vector2(-1, 1);
        right.GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);
        right.transform.SetParent(transform);

        do
        {
            t += Time.deltaTime;

            var percent = t / _interval;
            ls.color = new Color(1, 1, 1, Mathf.Clamp01(percent + 0.25f));
            rs.color = new Color(1, 1, 1, Mathf.Clamp01(percent + 0.25f));

            left.GetComponent<RectTransform>().anchoredPosition = Vector3.left * (200 - (Mathf.Clamp01(percent) * 200));
            right.GetComponent<RectTransform>().anchoredPosition = Vector3.right * (200 - (Mathf.Clamp01(percent) * 200));
            yield return new WaitForSeconds(0);
        } while (t < _interval);
        t = 0;
        do
        {
            t += Time.deltaTime;
            var percent = t / (_interval * 0.25f);

            ls.color = new Color(1, 1, 1, 1 - Mathf.Clamp01(percent));
            rs.color = new Color(1, 1, 1, 1 - Mathf.Clamp01(percent));

            yield return new WaitForSeconds(0);
        } while (t < _interval * 0.25f);

        Destroy(left);
        Destroy(right);

        yield return null;
    }
    //this makes it so in any scripts we need to check for the player being on beat, we just need to do Metronome.inst.IsOnBeat()
    //which returns a bool of if the metronome is currently on an offbeat or an on beat


    public bool IsOnBeat(bool isplayer = false)
    {
        //calculate exactly how on beat the player is
        if(onBeat && isplayer)
            GetComponent<Image>().color = new Color(1, 1, 1, 1);
        return onBeat;
    }
    public float GetInterval()
    {
        return 1 / ((float)BPM / 60);
    }
}
