using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public EnemyBody enemy;
    private Image EnemyHealthBar;
    private Image Fill;
    public Gradient gradient;
    private float max;
    private float current;
    private float target;
    private bool running;

    private void Start()
    {
        max = enemy._enemy.EnemyHealth;
        EnemyHealthBar = transform.Find("EnemyCanvas").transform.Find("HealthBorder").GetComponent<Image>();
        Fill = EnemyHealthBar.transform.GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        current = enemy.health;
        EnemyHealthBar.transform.forward = Camera.main.transform.forward;
        target = current / max;
        if(!running) StartCoroutine(LoseHealthAnim());
        Fill.color = gradient.Evaluate(1f- Fill.fillAmount);
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