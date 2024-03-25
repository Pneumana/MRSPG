using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossRoom : MonoBehaviour
{
    public GameObject BossHealthDisplay;
    public HealthBar BossHealth;
    public string Name;

    private void Start()
    {
        BossHealth.EnemyHealthBar = BossHealthDisplay.GetComponent<Image>();
        if (BossHealthDisplay.activeSelf) { BossHealthDisplay.SetActive(false); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            BossHealthDisplay.SetActive(true);
            BossHealthDisplay.transform.Find("BossName").GetComponent<TextMeshProUGUI>().text = Name;
            //change music to boss music
        }
    }
}
