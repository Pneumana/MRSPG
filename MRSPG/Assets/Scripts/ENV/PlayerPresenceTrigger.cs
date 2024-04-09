using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPresenceTrigger : MonoBehaviour
{
    EnvTrigger parent;
    private void Start()
    {
        parent  =GetComponentInParent<EnvTrigger>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "PlayerObj")
        {
            parent.TriggerAll();
            Debug.Log("Move");
        }
    }
}
