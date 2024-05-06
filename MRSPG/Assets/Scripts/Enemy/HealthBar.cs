using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public EnemyBody enemy;
    [HideInInspector] public Image EnemyHealthBar;
    [HideInInspector] public Image Trailer;
    [HideInInspector] public Image Fill;
    public Gradient gradient;
    private float max;
    private float current;
    private float target;
    private bool running;

    private void Start()
    {
        max = enemy._enemy.EnemyHealth;
        EnemyHealthBar = transform.Find("EnemyCanvas").transform.Find("HealthBorder").GetComponent<Image>();
        Fill = EnemyHealthBar.transform.GetChild(1).GetComponent<Image>();
        Trailer = EnemyHealthBar.transform.GetChild(0).GetComponent<Image>();
    }

    private void OnEnable()
    {
        enemy.health = (int)max;
    }

    private void Update()
    {
        current = enemy.health;
        EnemyHealthBar.transform.forward = Camera.main.transform.forward;
        target = current / max;

        Fill.fillAmount = target;
        if (Trailer.fillAmount > Fill.fillAmount)
            Trailer.fillAmount = Fill.fillAmount;
        if (!running) StartCoroutine(LoseHealthAnim());
        //Fill.color = gradient.Evaluate(1f- Fill.fillAmount);
    }
    IEnumerator LoseHealthAnim()
    {
        running = true;
        float amount = Fill.fillAmount;
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * 4;
            Trailer.fillAmount = Mathf.Lerp(amount, target, time);
            yield return null;
        }
        running = false;
    }
    public void Refresh()
    {
        StopAllCoroutines();
        running = false;
        Fill.fillAmount = enemy._enemy.EnemyHealth;
    }
}