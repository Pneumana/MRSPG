using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Define the enemies type, attack pattern, and health here.
/// </summary>
[CreateAssetMenu(fileName = "Enemy Type", menuName = "Enemy/Type", order = 1)]
public class EnemySetting : ScriptableObject
{
    [Header("Main Settings")]
    public EnemyType type;
    public Attack[] pattern;

    [Header("Enemy Parameters")]
    public int EnemyHealth;
    public float speed;
    public float TimeBetweenAttacks;
    public float ChargeTime;
    [Header("Beat Values")]
    public int EnergyGainedOnBeat;
    public int EnergyGainedOffBeat;
}

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

/*#region Attacks:
[System.Serializable]
public class AttackStyle
{
    [SerializeField] private Attack _attack;
}

#endregion*/

#region Enemy Types
public class Standard
{
    
}

public class Heavy
{

}

public class Ranged
{

}

#endregion
