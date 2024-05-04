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
    Boss, // 4
    You // 5
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
    public string EnemyName;
    public Attack[] pattern;
    public EnemyType type;

    [Header("Beat Variables")]
    public int EnergyGainedOnBeat;
    public int EnergyGainedOffBeat;

    [Header("Enemy Variables")]
    public int EnemyHealth;

    public float groundRadius;
    public float NavMeshSpeed;
    public float NavMeshSlowedSpeed;

    [Header("Rigidbody mass when the enemy is being knocked back")]
    public float knockbackMass;

    [Header("Attack Variables")]
    public int Damage;
    public float AttackRange;
    public Vector3 Hitbox;
    public float HitboxOffset;


    [Header("Target Variables")]
    public float FollowRange;
    public GameObject PlayerSettings;
    public GameObject PlayerObject;
}

