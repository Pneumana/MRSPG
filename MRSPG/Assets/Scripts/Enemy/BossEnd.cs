using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossEnd : MonoBehaviour
{

    EnemyBody body;

    private void Start()
    {
        body = GetComponent<EnemyBody>();
    }

    private void Update()
    {
        if(body.health <= 0)
        {
            SceneManager.LoadScene("Level 1.5");
        }
    }
}
