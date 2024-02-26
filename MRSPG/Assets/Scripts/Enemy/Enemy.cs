using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Utilize the data from the EnemySetting script to get all enemy data.
/// Will keep track and play through attack cycles.
/// </summary>

public class Enemy : MonoBehaviour
{
    #region Variables

    private Transform enemyObj;
    public EnemySetting _enemy;
    List<GameObject> enemiesInRange = new List<GameObject>();

    [Header("Enemy Parameters")]
    private Rigidbody Rigidbody;
    Vector3 targetPos;
    Vector3 lookatvector;
    bool CanAttack = true;
    bool isGrounded;
    private Transform Gun;
    [Header("Target Parameters")]
    bool playerInRange;
    float maxdistance = 6f;

    #endregion

    #region Define Enemy

    private void SetEnemyData(EnemySetting _enemy)
    {
        EnemyType _enemytype = _enemy.type;
        switch (_enemytype)
        {
            case EnemyType.Standard:
                gameObject.name = "StandardEnemy";
                gameObject.tag = "Enemy";
                Rigidbody = gameObject.GetComponent<Rigidbody>();
                break;
            case EnemyType.Heavy:
                gameObject.name = "HeavyEnemy";
                gameObject.tag = "Enemy";
                Rigidbody = gameObject.GetComponent<Rigidbody>();
                break;
            case EnemyType.Ranged:
                gameObject.name = "RangedEnemy";
                gameObject.tag = "Enemy";
                Gun = gameObject.transform.Find("Gun");
                break;

        }
        _enemy.PlayerSettings = GameObject.Find("Player");
        _enemy.PlayerObject = GameObject.Find("Player/PlayerObj");
        _enemy.Metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        enemyObj = gameObject.transform.GetChild(0);
    }

    #endregion

    public void Start()
    {
        //Use the SetEnemyData(EnemySetting _enemy) function to use the correct variables.
        SetEnemyData(_enemy);
        if(_enemy.PlayerSettings != null)
        {
        }else if (_enemy.PlayerSettings == null) { Debug.Log("<color=red>Error: </color> Player was not found.");  }
        if(enemyObj.gameObject.GetComponent<ParticleSystem>() != null)
        {
            _enemy.ChargeParticle = enemyObj.gameObject.GetComponent<ParticleSystem>();
            _enemy.ChargeParticle.Stop();
        }
    }

    private void FixedUpdate()
    {
        //Move and change the rotation of the enemy towards the player.
        if(CheckForPlayer(transform.position, maxdistance, _enemy.PlayerObject.GetComponent<Collider>()))
        {
            enemyObj.LookAt(lookatvector);
            float distanceToPlayer = targetPos.magnitude;
            if (distanceToPlayer > 2f)
            {
                float adjustedSpeed = Mathf.Lerp(0, _enemy.speed, Mathf.Clamp01(distanceToPlayer / maxdistance));

                Vector3 move = targetPos.normalized * adjustedSpeed * Time.fixedDeltaTime;
                if (Rigidbody != null)
                {
                    Rigidbody.MovePosition(transform.position + move);
                }
            }
        }
    }

    public void Update()
    {
        //Find the player position to move and look at
        targetPos = _enemy.PlayerObject.transform.position - transform.position;
        lookatvector = _enemy.PlayerObject.transform.position;
        lookatvector.y = transform.position.y;

        //Begin the attack cycle:
        playerInRange = CheckForPlayer(transform.position, _enemy.AttackRange, _enemy.PlayerObject.GetComponent<Collider>());
        if(playerInRange && _enemy.Metronome.IsOnBeat() && CanAttack)
        {
            StartCoroutine(StartAttack(_enemy.pattern));
        }
        #region Ground Check + Falling rate
        if (Physics.Raycast(transform.position, Vector3.down, 2))
        {
            isGrounded = true;
        }
        if(!isGrounded && Rigidbody != null)
        {
            Rigidbody.drag = 0;
        }
        else if(isGrounded && Rigidbody != null)
        {
            Rigidbody.drag = 3;
        }
        #endregion
        #region Slow Other Enemies:
        /* if (CheckForEnemies(transform.position, 5))
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
         }*/
        #endregion
        Death();
    }

    #region Enemy Radius
    //A sphere collider to detect when the player is in a specific range and an enemy collider to slow other enemies.
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
        Gizmos.DrawWireSphere(transform.position, maxdistance);
    }
    #endregion

    #region Attack Types
    private IEnumerator Waiter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _enemy.Animations.SetBool("InRange", false);
    }
    private IEnumerator Charge(float seconds)
    {

        float begginingspeed = _enemy.speed;
        _enemy.speed = 0f;
        _enemy.ChargeParticle.Play();
        yield return new WaitForSeconds(seconds);
        _enemy.ChargeParticle.Stop();
        Debug.Log("Charged!");
        _enemy.speed = begginingspeed;
    }
    private void Lunge()
    {
        Debug.Log("Used the lunge function");
    }
    private void LightAttack(int Damage)
    {
        Debug.Log("Used the light attack function");
        if(_enemy.Animations != null)
        {

            _enemy.Animations.SetBool("InRange", true);
        }
        _enemy.PlayerSettings.GetComponent<Health>().LoseHealth(Damage);
        StartCoroutine(Waiter(1f));
    }
    private void HeavyAttack(int Damage)
    {
        Debug.Log("Used the heavy attack function");
        _enemy.PlayerSettings.GetComponent<Health>().LoseHealth(Damage);
    }
    private void Load()
    {
        Debug.Log("Used the load function");
    }
    private IEnumerator Shoot()
    {
        Debug.Log("Used the shoot function");
        GameObject bullet = Instantiate(_enemy.Bullet, Gun.position, Quaternion.identity);
        Vector3 distance = _enemy.PlayerObject.transform.position - bullet.transform.position;
            bullet.transform.forward = distance;
        while (distance.magnitude > 3f)//bullet.transform.forward != distance || 
        {
            bullet.transform.position = Vector3.MoveTowards(bullet.transform.position, _enemy.PlayerObject.transform.position, 1 * Time.fixedDeltaTime);
            yield return null;
        }
    }

    public IEnumerator StartAttack(Attack[] pattern)
    {
        CanAttack = false;
        foreach (Attack attack in pattern)
        {
            if(playerInRange)
            {
                switch (attack)
                {
                    default:
                        break;
                    case Attack.Charge:
                        StartCoroutine(Charge(_enemy.ChargeTime));
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
                        StartCoroutine(Shoot());
                        break;
                }
                yield return new WaitForSeconds(_enemy.TimeBetweenAttacks);
            }
        }
        CanAttack = true;
    }
    #endregion

    #region Damage Conditions:
    void Death()
    {
        if (_enemy.EnemyHealth <= 0)
        {
            Debug.Log("The enemy has died");
            if (_enemy.PlayerSettings.GetComponent<Energy>().currentEnergy < 50)
            {
                _enemy.PlayerSettings.GetComponent<Energy>().GainEnergy(_enemy.EnergyGainedOnBeat, _enemy.EnergyGainedOffBeat);
            }
            Destroy(gameObject);
        }
    }
    #endregion
}