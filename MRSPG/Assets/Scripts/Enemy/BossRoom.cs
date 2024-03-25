using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : MonoBehaviour
{
    public GameObject BossHealthDisplay;

    private void Start()
    {
        if(BossHealthDisplay.activeSelf) { BossHealthDisplay.SetActive(false); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            BossHealthDisplay.SetActive(true);
            //change music to boss music
        }
    }
}
