using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.Universal;
using Color = UnityEngine.Color;

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
    bool ShootingRange;
    public bool PlayerIsInSight;

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
        _enemy.Metronome = Metronome.inst;
        enemyObj = gameObject.transform.GetChild(0);
        _enemy.Animations = enemyObj.transform.GetChild(0).GetComponent<Animator>();
    }

    #endregion

    #region Enemy Radius
    //A sphere collider to detect when the player is in a specific range and an enemy collider to slow other enemies.
    private bool CheckForPlayer(Vector3 center, float radius, Collider plr)
    {
        Collider[] collider = Physics.OverlapSphere(center, radius);
        foreach (var obj in collider)
        {
            if (obj == plr)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckForEnemies(Vector3 center, float radius)
    {
        Collider[] collider = Physics.OverlapSphere(center, radius);
        foreach (var obj in collider)
        {
            if (obj.tag == "Enemy")
            {
                if (!enemiesInRange.Contains(obj.gameObject))
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
        Gizmos.DrawWireSphere(transform.position, _enemy.AttackRange);
        Gizmos.DrawWireSphere(transform.position, _enemy.ShootRange);
        Gizmos.DrawWireSphere(transform.position, _enemy.FollowRange);
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
        if(CheckForPlayer(transform.position, _enemy.FollowRange, _enemy.PlayerObject.GetComponent<Collider>()))
        {
            enemyObj.LookAt(lookatvector);
            float distanceToPlayer = targetPos.magnitude;
            if (distanceToPlayer > 2f)
            {
                float adjustedSpeed = Mathf.Lerp(0, _enemy.speed, Mathf.Clamp01(distanceToPlayer / _enemy.FollowRange));

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

        LayerMask Enemy = LayerMask.GetMask("Enemy");
        RaycastHit ray;
        if (Physics.Raycast(enemyObj.position, lookatvector - transform.position, out ray))
        {
            if (ray.collider.CompareTag("Player"))
            {
                PlayerIsInSight = true;
            }
            else PlayerIsInSight = false;
        }
        //Begin the attack cycle:

        playerInRange = CheckForPlayer(transform.position, _enemy.AttackRange, _enemy.PlayerObject.GetComponent<Collider>());
        if(playerInRange && _enemy.Metronome.IsOnBeat() && CanAttack)
        {
            StartCoroutine(StartAttack(_enemy.pattern));
        }

        if(_enemy.type == EnemyType.Ranged)
        {
            ShootingRange = CheckForPlayer(transform.position, _enemy.ShootRange, _enemy.PlayerObject.GetComponent<Collider>());
            if (ShootingRange && _enemy.Metronome.IsOnBeat() && CanAttack)
            {
                StartCoroutine(StartAttack(_enemy.pattern));
            }
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
    private IEnumerator Load(float seconds)
    {
        _enemy.ChargeParticle.Play();
        yield return new WaitForSeconds(seconds);
        _enemy.ChargeParticle.Stop();
    }
    private IEnumerator Shoot(int Damage)
    {
        LayerMask Player = LayerMask.GetMask("Player");
        LayerMask Enemy = LayerMask.GetMask("Enemy");
        bool bulletCollided = false;
        GameObject bullet = Instantiate(_enemy.Bullet, Gun.position, Quaternion.identity);
        bool HitPlayer = Physics.CheckSphere(bullet.transform.position, 0.1f, Player);
        Vector3 distance = _enemy.PlayerObject.transform.position - bullet.transform.position;
        bullet.transform.forward = distance;
        Vector3 PositionOnBeat = _enemy.PlayerObject.transform.position;
        while (bullet.transform.position != PositionOnBeat)
        {
            bullet.transform.position = Vector3.MoveTowards(bullet.transform.position, PositionOnBeat, 8f * Time.fixedDeltaTime);
            if (Physics.CheckSphere(bullet.transform.position, 0.1f, Player))
            {
                if(_enemy.PlayerSettings.GetComponent<InputControls>().canDash)
                {
                    //_enemy.PlayerSettings.GetComponent<Health>().LoseHealth(Damage);
                    Debug.Log("The bullet hit the player");
                }else if(!_enemy.PlayerSettings.GetComponent<InputControls>().canDash && Metronome.inst.IsOnBeat())
                {
                    PositionOnBeat = transform.position;
                    Debug.Log("The bullet changed direction");
                }
                bulletCollided = true;
                Destroy(bullet);
                break;
            }
            else if (bullet != null && Physics.CheckSphere(bullet.transform.position, 0.1f, Enemy))
            {
                gameObject.GetComponent<EnemyBody>().ModifyHealth(2);
                Debug.Log("The bullet hit itself");
                bulletCollided = true;
                Destroy(bullet);
                break;
            }
            yield return null;
        }
        if (bullet != null && !bulletCollided) { Destroy(bullet); Debug.Log("The bullet did not collide with anything"); }
    }

    public IEnumerator StartAttack(Attack[] pattern)
    {
        CanAttack = false;
        foreach (Attack attack in pattern)
        {
            if(playerInRange || ShootingRange)
            {
                switch (attack)
                {
                    default:
                        break;
                    case Attack.Charge:
                        StartCoroutine(Charge(_enemy.ChargeTime));
                        break;
                    case Attack.Light:
                        LightAttack(_enemy.Damage);
                        break;
                    case Attack.Heavy:
                        HeavyAttack(_enemy.Damage);
                        break;
                    case Attack.Load:
                        StartCoroutine(Load(_enemy.ChargeTime));
                        break;
                    case Attack.Shoot:
                        if (PlayerIsInSight == true) StartCoroutine(Shoot(_enemy.BulletDamage));
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
        Energy energy = _enemy.PlayerSettings.GetComponent<Energy>();
        if (_enemy.EnemyHealth <= 0)
        {
            Debug.Log("The enemy has died");
            if (Metronome.inst.IsOnBeat()) { energy.GainEnergy(10); }
            else { energy.GainEnergy(5); }
            /*if (_enemy.PlayerSettings.GetComponent<Energy>().currentEnergy < 50)
            {
                _enemy.PlayerSettings.GetComponent<Energy>().GainEnergy(_enemy.EnergyGainedOnBeat, _enemy.EnergyGainedOffBeat);
            }*/
            Destroy(gameObject);
        }
    }
    #endregion
}