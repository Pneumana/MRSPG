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
    public EnemyType type;
    public Attack[] pattern;
    public int EnemyHealth;
    public float speed;
    public float TimeBetweenAttacks;
    public float ChargeTime;
}

#region Enums
public enum EnemyType
{
    Standard, //0
    Heavy, //1
    Ranged, //2
    Boss // 3
};
public enum Attack
{
    Charge,
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
