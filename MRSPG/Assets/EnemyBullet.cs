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
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.name == "PlayerObj")
        {
            //hurt player
            FindFirstObjectByType<Health>().LoseHealth(1);
        }
        else if(collision.collider.gameObject.GetComponent<EnemyBody>()!=null)
        {
            collision.collider.gameObject.GetComponent<EnemyBody>().ModifyHealth(damage);
        }
        destroyed = true;
    }
}
