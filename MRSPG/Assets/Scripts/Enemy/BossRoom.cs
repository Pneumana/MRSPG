using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossRoom : MonoBehaviour
{
    public EnemyBody enemy;
    public GameObject BossHealthBar;
    public Image Fill;
    public Gradient gradient;
    public bool StartFight;
    public bool FledFight;
    private float max;
    private float current;
    private float target;
    public bool running;

    private void Start()
    {
        max = enemy._enemy.EnemyHealth;
        if (BossHealthBar.activeSelf) { BossHealthBar.SetActive(false); }
    }

    private void OnTriggerEnter(Collider other)
    {
        FledFight = false;
        if (other.CompareTag("Player"))
        {
            Fill.fillAmount = target;
            if(!StartFight)
            {
                Fill.fillAmount = 1f;
                StartFight = true;
            }
            BossHealthBar.SetActive(true);
            BossHealthBar.transform.Find("BossName").GetComponent<TextMeshProUGUI>().text = enemy._enemy.EnemyName;
            //change music to boss music
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (StartFight && !FledFight)
        {
            current = enemy.health;
            target = current / max;
            if (!running) StartCoroutine(LoseHealthAnim());
            Fill.color = gradient.Evaluate(1f - Fill.fillAmount);
            if (enemy == null) { BossHealthBar.SetActive(false); }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            FledFight = true;
            StopCoroutine(LoseHealthAnim());
            BossHealthBar.SetActive(false);
            //change music to boss music
        }
    }

    IEnumerator LoseHealthAnim()
    {
        running = true;
        float amount = Fill.fillAmount;
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            Fill.fillAmount = Mathf.Lerp(amount, target, (time / 1f));
            yield return null;
        }
        running = false;
    }
}
