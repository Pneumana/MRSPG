using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed;
    public bool destroyed;
    public int damage = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!destroyed)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else
        {
            var list = GetComponentsInChildren<ParticleSystem>();
            int remainingParticles = 0;
            foreach (ParticleSystem ps in list)
            {
                remainingParticles += ps.particleCount;
            }
            if (remainingParticles == 0)
                Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("enemybullet hit " + collision.gameObject.name, gameObject);
        if (collision.gameObject.name == "PlayerObj")
        {
            //hurt player
            FindFirstObjectByType<Health>().LoseHealth(1);
        }
        else if (collision.gameObject.GetComponent<EnemyBody>() != null)
        {
            collision.gameObject.GetComponent<EnemyBody>().ModifyHealth(damage);
        }
        destroyed = true;
        GetComponent<Collider>().enabled = false;
        var list = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in list)
        {
            var emm = ps.emission;
            emm.rateOverTimeMultiplier = 0;
        }
    }
}
