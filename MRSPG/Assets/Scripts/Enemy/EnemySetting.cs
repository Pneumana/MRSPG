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
    [Header("Main Variables")]
    public EnemyType type;
    public Attack[] pattern;

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

    [Header("Ranged Variables")]
    public GameObject Bullet;
    public int BulletDamage;

    [Header("Target Variables")]
    public Metronome Metronome;
    public GameObject PlayerSettings;
    public GameObject PlayerObject;
}
