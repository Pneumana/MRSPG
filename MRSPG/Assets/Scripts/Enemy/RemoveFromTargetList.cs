using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveFromTargetList : MonoBehaviour
{
    private void OnDestroy()
    {
        GameObject.Find("TimeScaler").GetComponent<LockOnSystem>().UpdateEnemyList();
    }
}
