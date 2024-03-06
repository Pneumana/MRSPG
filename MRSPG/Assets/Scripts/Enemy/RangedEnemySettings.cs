using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ranged Enemy Settings", menuName = "Enemy/Ranged Enemy", order = 2)]
public class RangedEnemySettings : EnemySetting
{
    [Header("Ranged Variables")]
    public GameObject Bullet;
    public float ShootRange;
    public int BulletDamage;
}
