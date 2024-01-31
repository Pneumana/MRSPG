using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Utilize the data from the EnemySetting script to get all enemy data.
/// Will keep track and play through attack cycles.
/// </summary>
public class Enemy : MonoBehaviour
{
    #region Variables
    [SerializeField] private EnemySetting _enemy;
    [SerializeField] private Metronome metronome;
    [SerializeField] private List<GameObject> enemiesInRange = new List<GameObject>();
    private int health;
    private float speed;
    private float TimeBetweenAttacks;
    private float ChargeTime;
    private bool CanAttack = true;

    private GameObject _player;
    private Health _playerhealth;
    private Vector3 _target;


    #endregion

    public void Start()
    {
        SetTag(_enemy.type);
        metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        _player = GameObject.Find("Player/PlayerObj");
        if(_player != null)
        {
            _playerhealth = GameObject.Find("Player").GetComponent<Health>();
        }else if (_player == null) { Debug.Log("<color=red>Error: </color> Player was not found.");  }
        health = _enemy.EnemyHealth;
        speed = _enemy.speed;
        TimeBetweenAttacks = _enemy.TimeBetweenAttacks;
        ChargeTime = _enemy.ChargeTime;
    }
    public void Update()
    {

        _target = _player.transform.position;
        float _distFromPlr = Vector3.Distance(transform.position, _target);
        float enemy_speed = speed * Time.deltaTime;

        if(CheckForPlayer(transform.position, 5, _player.GetComponent<Collider>()))
        {
            //transform.position = Vector3.MoveTowards(transform.position, _target, enemy_speed);
            transform.LookAt(_target);
        }
        if(CheckForPlayer(transform.position, 2, _player.GetComponent<Collider>()) && CanAttack)
        {
            CanAttack = false;
            StartCoroutine(StartAttack(_enemy.pattern));
        }

        if (CheckForEnemies(transform.position, 5))
        {
            enemiesInRange.Remove(gameObject);
            foreach(GameObject enemy in enemiesInRange)
            {
                enemy.GetComponent<Enemy>().speed = 1f;
            }
        }
        else
        {
            foreach(GameObject enemy in enemiesInRange)
            {
                enemy.GetComponent<Enemy>().speed = 6f;
            }
            enemiesInRange.Clear();
        }
    }

    #region Define Enemy
    private string SetTag(EnemyType _enemytype)
    {
        switch(_enemytype)
        {
            case EnemyType.Standard:
                gameObject.name = "StandardEnemy";
                gameObject.tag = "Enemy";
                break;
            case EnemyType.Heavy:
                gameObject.name = "HeavyEnemy";
                gameObject.tag = "Enemy";
                break;
            case EnemyType.Ranged: 
                gameObject.name = "RangedEnemy";
                gameObject.tag = "Enemy";
                break;

        }
        return null;
    }

    #endregion

    #region Enemy Radius
    private bool CheckForPlayer(Vector3 center, float radius, Collider plr)
    {
        Collider[] collider = Physics.OverlapSphere(center, radius);
        foreach (var obj in collider)
        {
            if(obj == plr)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckForEnemies(Vector3 center, float radius)
    {
        Collider[] collider = Physics.OverlapSphere(center, radius);
        foreach(var obj in collider)
        {
            if(obj.tag == "Enemy")
            {
                if(!enemiesInRange.Contains(obj.gameObject))
                {
                    enemiesInRange.Add(obj.gameObject);
                }
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 2);
        Gizmos.DrawWireSphere(transform.position, 5);
    }
    #endregion

    #region Attack Types
    private IEnumerator Charge(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Debug.Log("Charged!");
    }
    private void LightAttack(int Damage)
    {
        Debug.Log("Used the light attack function");
        _playerhealth.LoseHealth(Damage);
    }
    private void HeavyAttack(int Damage)
    {
        Debug.Log("Used the heavy attack function");
        _playerhealth.LoseHealth(Damage);
    }
    private void Load()
    {
        Debug.Log("Used the load function");
    }
    private void Shoot()
    {
        Debug.Log("Used the shoot function");
    }

    private IEnumerator StartAttack(Attack[] pattern)
    {
        foreach(Attack attack in pattern)
        {
            if(CheckForPlayer(transform.position, 2, _player.GetComponent<Collider>()) && metronome.IsOnBeat())
            {
                switch (attack)
                {
                    default:
                        break;
                    case Attack.Charge:
                        StartCoroutine(Charge(1f));
                        break;
                    case Attack.Light:
                        LightAttack(1);
                        break;
                    case Attack.Heavy:
                        HeavyAttack(3);
                        break;
                    case Attack.Load:
                        Load();
                        break;
                    case Attack.Shoot:
                        Shoot();
                        break;
                }
                yield return new WaitForSeconds(TimeBetweenAttacks);
            }
        }
        CanAttack = true;
    }
    #endregion
}
