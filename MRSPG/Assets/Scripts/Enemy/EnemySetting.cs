using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Type", menuName = "Enemy/Type", order = 1)]
public class EnemySetting : ScriptableObject
{
    public EnemyType type;
    [SerializeField] private AttackStyle[] _pattern;
}

#region Enums
public enum EnemyType
{
    Standard, //0
    Heavy, //1
    Ranged //2
};
enum Attack
{
    Charge,
    Light,
    Heavy,
    Load,
    Shoot
};
#endregion

#region Attacks:
[System.Serializable]
public class AttackStyle
{
    [SerializeField] private Attack _attack;
}
public class ChargeUp
{

}

public class LightAttack
{

}

public class HeavyAttack
{

}
#endregion

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
