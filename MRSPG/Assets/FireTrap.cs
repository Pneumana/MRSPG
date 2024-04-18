using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireTrap : MonoBehaviour
{
    float bpm;
    float t;
    bool activated;
    [SerializeField] int loopsToTrigger;
    [SerializeField] int loopsToStop;
    [SerializeField] GameObject fireball;
    [SerializeField] Image warn;

    [SerializeField] Animator rig;

    float targetWarnFill;
    int loops;
    [SerializeField]Vector3 launchPos;

    float scale = 1;
    float maxscale = 1.25f;

    // Start is called before the first frame update
    void Start()
    {
        bpm = (1 / ((float)Metronome.inst.BPM / 60));
        rig.SetFloat("IntervalSpeed", (Metronome.inst.BPM / 60));
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
        //var targetscale = 
        rig.gameObject.transform.localScale = Vector3.Lerp(Vector3.one * scale, Vector3.one * maxscale, warn.fillAmount);
        if (loops == loopsToTrigger && !activated)
        {
            activated = true;
            loops = 0;
            //activate
            Shoot();
        }
        if (loops == loopsToStop && activated)
        {
            activated = false;
            loops = 0;
            //activate
            
        }
    }
    void Shoot()
    {
        rig.SetTrigger("Activate");
        var n = Instantiate(fireball);
        n.GetComponent<EnemyBullet>().enabled = true;
        n.GetComponent<EnemyBullet>().homingTarget = GameObject.Find("PlayerObj").transform;
        n.transform.forward = transform.up;
        n.transform.position = transform.position + transform.up;
        Debug.Log("spawned object at " + n.transform.position);
    }
    private void OnDrawGizmosSelected()
    {
        launchPos = transform.up;
        Debug.DrawLine(transform.position + launchPos, transform.position + launchPos + transform.up, Color.red);
        Debug.DrawLine(transform.position + launchPos - transform.right * 0.5f, transform.position+ launchPos + transform.right * 0.5f, Color.red);
        Debug.DrawLine(transform.position + launchPos + transform.forward * 0.5f, transform.position + launchPos - transform.forward * 0.5f, Color.red);
    }
}
