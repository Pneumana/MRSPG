using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.PlayerSettings;

public class ComboManager : MonoBehaviour
{
    public GameObject prefab;
    [SerializeField]TextMeshProUGUI comboLabel;
    [SerializeField] Image comboBar;
    [SerializeField] Image prevFillComboBar;
    public Transform eventParent;

    public List<GameObject> eventObjs = new List<GameObject>();


    public static ComboManager inst;

    float timeLeftInCombo;
    public float removeEventTick = 2;
    bool removing;

    public List<float> eventAddedTimeStamp = new List<float> ();

    public List<string> comboTier;
    public List<int> pointsPerTier;
    public List<Color> tierColors = new List<Color>();

    public float currentPoints;
    public int currentTier;

    public float startedTimestamp;
    // Start is called before the first frame update
    void Start()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            if(inst!=this)
                Destroy(gameObject);
        }
/*        AddEvent();
            AddEvent();
            AddEvent();*/
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            //if(Metronome.inst.IsOnBeat())
                AddEvent("Test", 75);
            //else
                //AddEvent("Offbeat", 10);
        }

        if(currentPoints > 0)
        {
            currentPoints -= Time.deltaTime * 15;
            if(currentTier< pointsPerTier.Count - 1)
            {
            while(currentPoints > pointsPerTier[currentTier])
            {
                //Debug.Log("tier up");
                currentTier++;
            }

            }
            if(currentTier > 0)
            {
                prevFillComboBar.color = tierColors[currentTier - 1];
                if (currentPoints < pointsPerTier[currentTier - 1])
                {
                    currentTier--;
                }
                
            }
            else
            {
                prevFillComboBar.color = Color.clear;
            }
            comboBar.color = tierColors[currentTier];
            comboLabel.text = comboTier[currentTier];
            comboLabel.color = tierColors[currentTier];
            if (currentTier != 0)
            {
                comboBar.fillAmount = (currentPoints - pointsPerTier[currentTier-1]) / ((float)pointsPerTier[currentTier] - (float)pointsPerTier[currentTier - 1]);
                prevFillComboBar.fillAmount = 1f - comboBar.fillAmount;
                //Debug.Log("difference = " + currentPoints + " - " + pointsPerTier[currentTier-1] + " = " + (currentPoints - pointsPerTier[currentTier - 1]));
                //Debug.Log("difference = " + pointsPerTier[currentTier] + " - " + pointsPerTier[currentTier - 1] + " = " + (pointsPerTier[currentTier] - (float)pointsPerTier[currentTier - 1]));
                //Debug.Log("divide = " + (currentPoints - pointsPerTier[currentTier - 1]) + " / " + (pointsPerTier[currentTier] - (float)pointsPerTier[currentTier - 1]) + " = " + ((currentPoints - pointsPerTier[currentTier - 1]) / ((float)pointsPerTier[currentTier] - (float)pointsPerTier[currentTier - 1])));
            }
            else
            {

                comboBar.fillAmount = currentPoints / (float)pointsPerTier[currentTier];
                prevFillComboBar.fillAmount = 1f - comboBar.fillAmount;
            }
        }

        if (eventObjs.Count == 0)
            return;
        if(removeEventTick>0)
            removeEventTick -= Time.deltaTime;
        if(removeEventTick <= 0)
        {
            removeEventTick = 2;
            if (startedTimestamp == 0)
            {
                startedTimestamp = Time.time;
                //Debug.Log("started @ " + startedTimestamp);
            }

            if(startedTimestamp - eventAddedTimeStamp[0] > 0.25f)
            {
                StartCoroutine(SquishEvent());
            }
            else
            {
                startedTimestamp = 0;
            }
        }


    }
    IEnumerator SquishEvent()
    {
        if (removing)
            yield return null;
        removing = true;
        //Debug.Log("squish");
        float time = 0;
        do
        {
            time+=Time.deltaTime * 3;
            //Debug.Log("squish anim % = " + time);
            eventObjs[0].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f - time);
            float pos = 0;
            for(int i = 0; i < eventObjs.Count; i++)
            {
                var go = eventObjs[i];
                var beforeMePos = pos;
                pos -= eventObjs[i].GetComponent<RectTransform>().sizeDelta.y * eventObjs[i].GetComponent<RectTransform>().localScale.y;
                if (go == eventObjs[0])
                    continue;
                go.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, beforeMePos);
                
            }
            //eventObjs[0].transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(1f, 1f);
/*            foreach(RectTransform tf in eventObjs[0].transform.GetComponentsInChildren<RectTransform>())
            {
                eventObjs[0].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f + time);
            }*/
            yield return new WaitForSeconds(0);
        } while (time < 1);
        if(eventObjs.Count > 1)
        {
            eventObjs[1].transform.SetParent(eventObjs[0].transform.parent);
        }
        Destroy(eventObjs[0]);
        eventObjs.RemoveAt(0);
        eventAddedTimeStamp.RemoveAt(0);
        removeEventTick = 0.1f;
        removing = false;
        yield return null;
    }
    [ContextMenu("Add Event")]
    public void AddEvent(string name, int points)
    {
        //needs string for label and int for score value

        currentPoints += points;
        var n = Instantiate(prefab);
        n.GetComponent<TextMeshProUGUI>().text = name;
        n.transform.SetParent(eventParent);
        n.transform.localScale = Vector3.one;
        /*        if (eventObjs.Count == 0)
                {
                    //go.transform.SetParent(eventParent);
                }
                else
                {
                    //go.transform.SetParent(eventObjs[eventObjs.Count-1].transform);
                }*/
        //n.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);
        eventObjs.Add(n);
        eventAddedTimeStamp.Add(Time.time);
        float pos = 0;
        if (eventObjs.Count > 1)
        {
            for (int i = 0; i < eventObjs.Count; i++)
            {
                var go = eventObjs[i];
                var beforeMePos = pos;
                pos -= eventObjs[i].GetComponent<RectTransform>().sizeDelta.y * eventObjs[i].GetComponent<RectTransform>().localScale.y;
                if (go == eventObjs[0])
                    continue;
                go.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, beforeMePos);

            }
        }
        else
        {
            n.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0);
        }
        currentPoints = Mathf.Clamp(currentPoints, 0, pointsPerTier[pointsPerTier.Count - 1]);
        //FindFirstObjectByType<Energy>().GainEnergy(1 + currentTier);
    }

}
