using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering.Universal;
using static EnemySetting;
using Color = UnityEngine.Color;
using UnityEngine.UI;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;
using UnityEngine.Rendering;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.AI;

/// <summary>
/// Utilize the data from the EnemySetting script to get all enemy data.
/// Will keep track and play through attack cycles.
/// </summary>

public class Enemy : MonoBehaviour
{
    #region Variables
    EnemyBody body;
    Transform enemyObj;
    private GameObject playerObj;
    public EnemySetting _enemy;
    HeavyEnemySettings heavy_enemy;
    RangedEnemySettings ranged_enemy;
    BossEnemySettings boss_enemy;
    List<GameObject> enemiesInRange = new List<GameObject>();

    [Header("Enemy Parameters")]
    Rigidbody Rigidbody;
    Vector3 targetPos;
    Vector3 lookatvector;
    public bool DisableAttack;
    bool CanAttack = true;
    bool IsStaggered = false;
    int PauseBeat;
    bool isGrounded;
    Transform Gun;
    ParticleSystem ChargeParticle;

    Animator Animations;


    [Header("Warning Pop Up")]
    GameObject Warning;
    bool WarningPlayed;

    [Header("Target Parameters")]
    bool playerInRange;
    bool ShootingRange;
    public bool PlayerIsInSight;
    public bool LookAt = true;
    [HideInInspector] public bool aggro = false;
    LayerMask PlayerMask;

    LayerMask GroundMask;
    private Metronome Metronome;
    GameObject Flare;

    #endregion

    #region Define Enemy

    private void SetEnemyData(EnemySetting _enemy)
    {
        EnemyType _enemytype = _enemy.type;
        gameObject.name = _enemy.EnemyName;
        gameObject.tag = "Enemy";
        enemyObj = gameObject.transform.GetChild(0);
        Animations = this.GetComponent<Animator>();
        switch (_enemytype)
        {
            case EnemyType.Standard:

                Animations = enemyObj.GetComponent<Animator>();
                Rigidbody = gameObject.GetComponent<Rigidbody>();
                break;
            case EnemyType.Heavy:
                heavy_enemy = _enemy as HeavyEnemySettings;
                Rigidbody = gameObject.GetComponent<Rigidbody>();
                break;
            case EnemyType.Ranged:
                ranged_enemy = _enemy as RangedEnemySettings;
                Gun = gameObject.transform.GetChild(0).Find("Gun");
                Flare = transform.Find("EnemyCanvas").transform.Find("LensFlare").gameObject;
                Flare.SetActive(false);
                break;
            case EnemyType.Boss:
                boss_enemy = _enemy as BossEnemySettings;
                Rigidbody = gameObject.GetComponent<Rigidbody>();
                Animations = enemyObj.GetComponent<Animator>();
                if (boss_enemy.EnemyName == "Homunculus")
                {
                    Gun = gameObject.transform.GetChild(0).Find("Gun");
                }
                break;

        }
        _enemy.PlayerSettings = GameObject.Find("Player");
        _enemy.PlayerObject = GameObject.Find("Player/PlayerObj");
        Metronome = Metronome.inst;
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
        Gizmos.DrawWireSphere(transform.position, _enemy.FollowRange);
        if (ranged_enemy != null)
        {
            Gizmos.DrawWireSphere(transform.position, ranged_enemy.ShootRange);
        }
        Gizmos.color = Color.red;
        Vector3[] vertices = GetRotatedCubeVertices(transform.position + transform.forward * _enemy.HitboxOffset, transform.rotation, _enemy.Hitbox);

        for (int i = 0; i < 4; i++)
        {
            int nextIndex = (i + 1) % 4;
            Gizmos.DrawLine(vertices[i], vertices[nextIndex]);
            Gizmos.DrawLine(vertices[i + 4], vertices[nextIndex + 4]);
            Gizmos.DrawLine(vertices[i], vertices[i + 4]);
        }
    }
    private Vector3[] GetRotatedCubeVertices(Vector3 position, Quaternion rotation, Vector3 size)
    {
        Vector3[] vertices = new Vector3[8];

        // Calculate vertices relative to the center of the cube
        Vector3 halfSize = size * 0.5f;

        vertices[0] = rotation * new Vector3(-halfSize.x, -halfSize.y, -halfSize.z) + position;
        vertices[1] = rotation * new Vector3(halfSize.x, -halfSize.y, -halfSize.z) + position;
        vertices[2] = rotation * new Vector3(halfSize.x, -halfSize.y, halfSize.z) + position;
        vertices[3] = rotation * new Vector3(-halfSize.x, -halfSize.y, halfSize.z) + position;

        vertices[4] = rotation * new Vector3(-halfSize.x, halfSize.y, -halfSize.z) + position;
        vertices[5] = rotation * new Vector3(halfSize.x, halfSize.y, -halfSize.z) + position;
        vertices[6] = rotation * new Vector3(halfSize.x, halfSize.y, halfSize.z) + position;
        vertices[7] = rotation * new Vector3(-halfSize.x, halfSize.y, halfSize.z) + position;

        return vertices;
    }
    #endregion

    public void Start()
    {
        //Use the SetEnemyData(EnemySetting _enemy) function to use the correct variables.
        playerObj = GameObject.Find("PlayerObj");
        Warning = transform.Find("EnemyCanvas").transform.Find("Warning").gameObject;
        Warning.SetActive(false);
        body = GetComponent<EnemyBody>();
        SetEnemyData(_enemy);
        PlayerMask = LayerMask.GetMask("Player");
        GroundMask = LayerMask.GetMask("Ground");
        if (_enemy.PlayerSettings != null)
        {
        }else if (_enemy.PlayerSettings == null) { Debug.Log("<color=red>Error: </color> Player was not found.");  }
        if(enemyObj.gameObject.GetComponent<ParticleSystem>() != null)
        {
            ChargeParticle = enemyObj.gameObject.GetComponent<ParticleSystem>();
            ChargeParticle.Stop();
        }
    }

    private void FixedUpdate()
    {
        //Move and change the rotation of the enemy towards the player.
        if(aggro && !WarningPlayed)
        {
            StartCoroutine(WarningPopUp());
        }
        if(LookAt && CheckForPlayer(transform.position, _enemy.FollowRange, _enemy.PlayerObject.GetComponent<Collider>()) || aggro)
        {
            transform.LookAt(lookatvector);
        }
        if (CheckForPlayer(transform.position, _enemy.FollowRange, _enemy.PlayerObject.GetComponent<Collider>()) || aggro)
        {
            if (body.me != null && body.me.enabled)
                body.me.destination = _enemy.PlayerObject.transform.position;
            aggro = true;
        }
    }

    public void Update()
    {
        if (Animations != null && body.me != null)
            Animations.SetFloat("Speed", this.GetComponent<NavMeshAgent>().velocity.magnitude);

        if(DisableAttack)
        {
            body.DisablePathfinding();
        }
        //Find the player position to move and look at
        targetPos = _enemy.PlayerObject.transform.position - transform.position;
        lookatvector = _enemy.PlayerObject.transform.position;
        lookatvector.y = transform.position.y;

        LayerMask Enemy = LayerMask.GetMask("Enemy");
        RaycastHit ray;
        Vector3 RayPosition = enemyObj.position;
        RayPosition.y = _enemy.PlayerObject.transform.position.y;
        if (Physics.Raycast(RayPosition, lookatvector - transform.position, out ray))
        {
            if (ray.collider.CompareTag("Player"))
            {
                PlayerIsInSight = true;
            }
            else PlayerIsInSight = false;
        }

        //Player sight visual:
        Debug.DrawRay(RayPosition, lookatvector - transform.position, Color.green);
        #region Attacking
        //Begin the attack cycle:
        playerInRange = CheckForPlayer(transform.position, _enemy.AttackRange, _enemy.PlayerObject.GetComponent<Collider>());
        if(playerInRange && Metronome.IsOnBeat() && CanAttack)
        {
            StartCoroutine(StartAttack(_enemy.pattern));
        }

        if(_enemy.type == EnemyType.Ranged)
        {
            ShootingRange = CheckForPlayer(transform.position, ranged_enemy.ShootRange, _enemy.PlayerObject.GetComponent<Collider>());
            if (ShootingRange)
            {
                aggro = true;
            }
            if (aggro && Metronome.IsOnBeat() && CanAttack)
            {
                StartCoroutine(StartAttack(_enemy.pattern));
            }
        }

        /*Boss 3 (You)
        It has the same movement as the player and attacks using the same dash and light attacks (slightly altered for gameplay reasons).
        
        ‘You’ has 3 attack patterns, charge up > light attack > light attack > light attack (on beat). 
        If facing the pit it will charge up > dash into the player (on beat). If the player is far from ‘You’, it will charge up > charge up > fire gun (on beat). 
        ‘You’s gun attack will display the same glare effect on charge up as the ranged enemy. 
        A perfectly timed dash in the bullets direction will send it back in ‘You’s direction. */

        if (_enemy.type == EnemyType.Boss)
        {
            bool AttackingRange = CheckForPlayer(transform.position, _enemy.FollowRange, _enemy.PlayerObject.GetComponent<Collider>());
            if (playerInRange && Metronome.IsOnBeat() && CanAttack)
            {
                StartCoroutine(StartAttack(boss_enemy.BossPattern[1].pattern));
            }
            else if(AttackingRange && Metronome.IsOnBeat() && CanAttack) { StartCoroutine(StartAttack(boss_enemy.BossPattern[0].pattern)); }
        }

        if(_enemy.type == EnemyType.You)
        {

        }
        #endregion
        #region Ground Check + Falling rate
        Vector3 GroundCheck = transform.GetChild(1).position;
        isGrounded = Physics.CheckSphere(GroundCheck, _enemy.groundRadius, GroundMask);
        if (!isGrounded && Rigidbody != null)
        {
            Rigidbody.drag = 0;
            if (transform.position.y < -50f)
            {
                body.ModifyHealth(body.health);
            }
        }
        else if(isGrounded && Rigidbody != null)
        {
            Rigidbody.drag = 3;
        }
        #endregion


        if (aggro)
        {
            if (body.me != null && body.me.enabled)
            {
                //if(body.me.isPathStale)

                var distToTarget = Vector3.Distance(transform.position, body.me.pathEndPosition);
                var playerDistFromEnd = Vector3.Distance(transform.position, body.me.pathEndPosition);
                if (distToTarget <= 1.0f || playerDistFromEnd >= 1 || body.me.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
                {
                    body.me.destination = _enemy.PlayerObject.transform.position;
                }
            }
        }
    }


    #region Attack Types

    private IEnumerator WarningPopUp()
    {
        WarningPlayed = true;
        Warning.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Destroy(Warning);
    }
    private IEnumerator Waiter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        //Animations.SetBool("InRange", false);
    }
    public void Stagger()
    {
        IsStaggered = true;
        if (gameObject.GetComponent<NavMeshAgent>() != null) { gameObject.GetComponent<NavMeshAgent>().enabled = false; }
        body.Shoved(playerObj.transform.forward * 5, "Stagger");
    }
    private IEnumerator Charge(int beats)
    {
        this.GetComponent<NavMeshAgent>().speed = _enemy.NavMeshSlowedSpeed;
        if (!playerInRange)
        {
            this.GetComponent<NavMeshAgent>().speed = _enemy.NavMeshSpeed;
            if (Animations != null)
                Animations.SetBool("Charge", false);
        }
        //if (ChargeParticle != null) ChargeParticle.Play();
        PauseBeat = Metronome.BeatsPassed;
        //yield return new WaitUntil(() => PauseBeat >= Metronome.BeatsPassed + beats);
        yield return new WaitForSeconds(Metronome.GetInterval());
        //if (ChargeParticle != null) ChargeParticle.Stop();
        this.GetComponent<NavMeshAgent>().speed = _enemy.NavMeshSpeed;
    }
    private void Lunge()
    {
        Debug.Log("Used the lunge function");
    }
    private void LightAttack(int Damage)
    {
        if(Physics.CheckBox(transform.position + transform.forward, _enemy.Hitbox, Quaternion.identity, PlayerMask))
        {
            _enemy.PlayerSettings.GetComponent<Health>().LoseHealth(Damage);
        }
    }
    private void HeavyAttack(int Damage)
    {

        if (Animations != null)
            Animations.SetBool("Charge", false);
        if (Physics.CheckBox(transform.position + transform.forward, _enemy.Hitbox, Quaternion.identity, PlayerMask))
        {
            _enemy.PlayerSettings.GetComponent<Health>().LoseHealth(Damage);
        }
    }
    private IEnumerator Load(int beats)
    {
        if (!ShootingRange)
        {
            if (Animations != null)
                Animations.SetBool("Charge", false);
        }
        Flare.SetActive(true);
        PauseBeat = Metronome.BeatsPassed;
        //yield return new WaitUntil(() => PauseBeat >= Metronome.BeatsPassed + beats);
        yield return new WaitForSeconds(Metronome.GetInterval());
    }
    private IEnumerator Shoot(int Damage)
    {

        LayerMask Player = LayerMask.GetMask("Player");
        LayerMask Enemy = LayerMask.GetMask("Enemy");
        GameObject bullet = Instantiate(ranged_enemy.Bullet, Gun.position, Quaternion.identity);
        bool HitPlayer = Physics.CheckSphere(bullet.transform.position, 0.1f, Player);
        Vector3 distance = _enemy.PlayerObject.transform.position - bullet.transform.position;
        bullet.transform.forward = distance;
        Vector3 PositionOnBeat = _enemy.PlayerObject.transform.position;
        while (bullet.transform.position != PositionOnBeat)
        {
            bullet.transform.position = Vector3.MoveTowards(bullet.transform.position, PositionOnBeat, 35f * Time.fixedDeltaTime);
            if (Physics.CheckSphere(bullet.transform.position, 0.1f, Player))
            {
                if(_enemy.PlayerSettings.GetComponent<InputControls>().canDash)
                {
                    _enemy.PlayerSettings.GetComponent<Health>().LoseHealth(Damage);
                    Debug.Log("The bullet hit the player");
                    Destroy(bullet);
                    break;
                }
                else if(!_enemy.PlayerSettings.GetComponent<InputControls>().canDash && Metronome.inst.IsOnBeat())
                {
                    PositionOnBeat = transform.position;
                    Debug.Log("The bullet changed direction");
                }
            }
            else if (bullet != null && Physics.CheckSphere(bullet.transform.position, 0.1f, Enemy))
            {
                gameObject.GetComponent<EnemyBody>().ModifyHealth(2);
                Debug.Log("The bullet hit itself");
                Destroy(bullet);
                break;
            }
            yield return null;
        }
        if (Physics.CheckSphere(bullet.transform.position, 0.1f)) Destroy(bullet);
        Animations.SetBool("Attack", false);
        Destroy(bullet);
    }
    private void SpinAttack(int Damage)
    {
        Debug.Log("boss go spinny");
        if (Physics.CheckBox(transform.position + transform.forward, _enemy.Hitbox, Quaternion.identity))
        {
            _enemy.PlayerSettings.GetComponent<Health>().LoseHealth(Damage);
        }
    }
    private IEnumerator EndLag(int beats)
    {
        PauseBeat = Metronome.BeatsPassed;
        //yield return new WaitUntil(() => PauseBeat >= Metronome.BeatsPassed + beats);
        yield return new WaitForSeconds(Metronome.GetInterval());
        Debug.Log("boss lazy, boss need time to recoop");
    }

    public IEnumerator StartAttack(Attack[] pattern)
    {
        if(!DisableAttack)
        {
            CanAttack = false;
            foreach (Attack attack in pattern)
            {
                if (IsStaggered) { IsStaggered = false; break; }
                if (playerInRange || ShootingRange || aggro)
                {
                    switch (attack)
                    {
                        default:
                            break;
                        case Attack.Charge:

                            if (Animations != null)
                                Animations.SetBool("Charge", true);
                            StartCoroutine(Charge(1));
                            break;
                        case Attack.Light:
                            if (Animations != null)
                                Animations.SetBool("Attack", true);
                            if (PlayerIsInSight == true) LightAttack(_enemy.Damage);
                            break;
                        case Attack.Heavy:
                            if (Animations != null)
                                Animations.SetBool("Attack", true);
                            if (PlayerIsInSight == true) HeavyAttack(_enemy.Damage);
                            break;
                        case Attack.Load:
                            if (Animations != null)
                                Animations.SetBool("Charge", true);
                            StartCoroutine(Load(1));
                            break;
                        case Attack.Shoot:
                            if (Animations != null)
                                Animations.SetBool("Attack", true);
                            if (PlayerIsInSight == true) StartCoroutine(Shoot(ranged_enemy.BulletDamage));
                            break;
                        case Attack.Spin:
                            if (PlayerIsInSight == true) SpinAttack(_enemy.Damage);
                            break;
                        case Attack.Lag:
                            if (PlayerIsInSight == true) StartCoroutine(EndLag(1));
                            break;
                    }
                    yield return new WaitForSeconds(Metronome.GetInterval());
                    if (Animations != null)
                    {
                        Animations.SetBool("Charge", false);
                        Animations.SetBool("Attack", false);
                    }
                }
            }
            CanAttack = true;

        }
    }
    #endregion
}