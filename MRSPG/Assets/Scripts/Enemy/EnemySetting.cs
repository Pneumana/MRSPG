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
    Shoot,
    Spin,
    Lag
};
#endregion
[CreateAssetMenu(fileName = "Enemy Type", menuName = "Enemy/Standard", order = 1)]
public class EnemySetting : ScriptableObject
{
    [Header("Main Variables")]
    public Attack[] pattern;
    public EnemyType type;

    [Header("Beat Variables")]
    public int EnergyGainedOnBeat;
    public int EnergyGainedOffBeat;

    [Header("Enemy Variables")]
    public int EnemyHealth;
    public float groundRadius;

    [Header("Attack Variables")]
    public int Damage;
    public float AttackRange;
    public float TimeBetweenAttacks;
    public float ChargeTime;
    public Vector3 Hitbox;
    public float HitboxOffset;


    [Header("Target Variables")]
    public float FollowRange;
    public Metronome Metronome;
    public GameObject PlayerSettings;
    public GameObject PlayerObject;
}

