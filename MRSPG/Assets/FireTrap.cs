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

    float targetWarnFill;
    int loops;
    [SerializeField]Vector3 launchPos;

    // Start is called before the first frame update
    void Start()
    {
        bpm = (1 / ((float)Metronome.inst.BPM / 60));
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
        var n = Instantiate(fireball);
        n.GetComponent<EnemyBullet>().enabled = true;
        n.transform.forward = transform.up;
        n.transform.position = transform.position + launchPos;
    }
    private void OnDrawGizmosSelected()
    {
        Debug.DrawLine(transform.position + launchPos, transform.position + launchPos + transform.up, Color.red);
        Debug.DrawLine(transform.position + launchPos - transform.right * 0.5f, transform.position+ launchPos + transform.right * 0.5f, Color.red);
        Debug.DrawLine(transform.position + launchPos + transform.forward * 0.5f, transform.position + launchPos - transform.forward * 0.5f, Color.red);
    }
}
