using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    public List<GameObject> ActiveEnemiesInScene = new List<GameObject>();
    public GameObject portal;
    public static EnemyTracker inst;

    [SerializeField] Animator anim;

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
        {
            if (inst != this)
                Destroy(gameObject);
        }
        portal.SetActive(false);
        if (anim != null)
        {
            anim.enabled = false;
        }
    }


    private void Update()
    {
        CheckEnemyActiveState();
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("killing all enemies");
            StartCoroutine(KillAll());
        }
    }

    IEnumerator KillAll()
    {
        while(ActiveEnemiesInScene.Count > 0)
        {
            var go = ActiveEnemiesInScene[0];
            if (go.GetComponent<EnemyBody>() != null)
                go.GetComponent<EnemyBody>().Die(EnemyBody.DamageTypes.DeathPlane);
            yield return new WaitForSeconds(0);
        }
    }

    void CheckEnemyActiveState()
    {
        if(ActiveEnemiesInScene.Count == 0)
        {
            if(anim != null)
            {
                anim.enabled = true;
            }
            portal.SetActive(true);
        }
    }
}
