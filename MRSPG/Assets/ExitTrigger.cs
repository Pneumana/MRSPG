using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public string mapLoad;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "PlayerObj")
        {
            LoadLevel.instance.Activate(mapLoad);
        }
    }
}
