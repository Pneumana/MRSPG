using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Settings", menuName = "Enemy/Boss", order = 3)]
public class BossEnemySettings : EnemySetting
{
    [System.Serializable]
    public class EnemyPattern
    {
        public Attack[] pattern;
    }

    [Header("Boss Variables")]
    public EnemyPattern[] BossPattern;
}
