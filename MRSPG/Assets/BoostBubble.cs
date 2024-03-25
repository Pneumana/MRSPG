using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoostBubble : MonoBehaviour
{
    [SerializeField] float boost;
    [SerializeField] float respawnTime;

    Renderer rend;
    Collider coll;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        coll = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "PlayerObj")
        {
            //add the difference in position from the player and the object on collision as extra boost height to compensate if the player doesnt land exactly on top of the object.
            InputControls.instance.velocity.y = boost;
            StartCoroutine(Popped());
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "PlayerObj")
        {
            //add the difference in position from the player and the object on collision as extra boost height to compensate if the player doesnt land exactly on top of the object.
            InputControls.instance.velocity.y = boost;
            StartCoroutine(Popped());
        }
    }

    IEnumerator Popped()
    {
        rend.enabled = false;
        coll.enabled = false;


        float t = 0;
        do
        {
            t += Time.deltaTime;
            yield return new WaitForSeconds(0);
        } while (t < respawnTime);

        rend.enabled = true;
        coll.enabled = true;

        yield return null;
    }
}
