using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Define the enemies type, attack pattern, and health here.
/// </summary>
#region Enums
public enum EnemyType
{
    Standard, //0
    AnkleBiters, //1
    Heavy, //2
    Ranged, //3
    Boss // 4
};
public enum Attack
{
    Charge,
    Lunge,
    Light,
    Heavy,
    Load,
    Shoot
};
#endregion
[CreateAssetMenu(fileName = "Enemy Type", menuName = "Enemy/Standard", order = 1)]
public class EnemySetting : ScriptableObject
{
    [System.Serializable]
    public class EnemyPattern
    {
        public Attack[] pattern;
    }
    public EnemyPattern[] EnemyPatterns;
    [Header("Main Variables")]
    public EnemyType type;

    [Header("Beat Variables")]
    public int EnergyGainedOnBeat;
    public int EnergyGainedOffBeat;

    [Header("Enemy Variables")]
    public int EnemyHealth;
    public float speed;
    public Rigidbody Rigidbody;

    [Header("Attack Variables")]
    public int Damage;
    public float AttackRange;
    public float TimeBetweenAttacks;
    public float ChargeTime;
    public float BaseDamage;
    public GameObject Hitbox;
    public ParticleSystem ChargeParticle;
    public Animator Animations;


    [Header("Target Variables")]
    public float FollowRange;
    public Metronome Metronome;
    public GameObject PlayerSettings;
    public GameObject PlayerObject;
}

[CreateAssetMenu(fileName = "Ranged Enemy Settings", menuName = "Enemy/Ranged Enemy", order = 2)]
public class RangedEnemySettings : EnemySetting
{
    [Header("Ranged Variables")]
    public GameObject Bullet;
    public float ShootRange;
    public int BulletDamage;
}
/*
[CreateAssetMenu(fileName = "Boss Settings", menuName = "Enemy/Boss", order = 3)]
public class BossEnemySettings : EnemySetting
{
    public float i;
}*/