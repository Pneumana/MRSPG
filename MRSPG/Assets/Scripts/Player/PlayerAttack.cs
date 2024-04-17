using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    #region variables
    public int MeleeCombo; //Will be out of date if read by other scripts
    public byte MeleeSide; //For animation
    public int RecentAttack;
    public bool DealtDamage;
    public bool HealCombo;
    private Vector3 EnemyDir;
    private Metronome metronome;
    private MeleeHitbox meleeHitbox;
    public CharacterController controller;
    private InputControls inputControls;
    private LockOnSystem lockOnSystem;
    private GameObject player;
    private GameObject playerObj;
    float timeout;
    #endregion
    void Start()
    {
        metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        meleeHitbox = GameObject.Find("MeleeHitbox").GetComponent<MeleeHitbox>();
        inputControls = GameObject.Find("Player").GetComponent<InputControls>();
        lockOnSystem = GameObject.Find("TimeScaler").GetComponent<LockOnSystem>();
        player = GameObject.Find("Player");
        playerObj = GameObject.Find("PlayerObj");
        MeleeCombo = 2;
        HealCombo = true;
    }
    public void Attack(InputAction.CallbackContext context) //Starts melee attack and updates melee combo
    {
        StopAllCoroutines();
        playerObj.GetComponent<Animator>().SetInteger("AttackChain", playerObj.GetComponent<Animator>().GetInteger("AttackChain") + 1);
        StartCoroutine(TimeOutAnimation());


        if (RecentAttack == metronome.BeatsPassed) { MeleeCombo = 1; HealCombo = true; return; } //anti button spam
        if (RecentAttack + 2 <= metronome.BeatsPassed || RecentAttack == metronome.BeatsPassed || !DealtDamage || MeleeCombo == 3) //reset melee combo conditions
        {
            MeleeCombo = 1;
            HealCombo = true;
        }
        else
        {
            MeleeCombo++;
            if (!Metronome.inst.IsOnBeat()) { HealCombo = false; }
        }
        if (lockOnSystem.trackedEnemy != null && !lockOnSystem.freeAim)
        {
            //points towards locked on enemy for melee
            Vector3 enemyDirection = (lockOnSystem.trackedEnemy.transform.position - playerObj.transform.position).normalized;
            EnemyDir = new Vector3(enemyDirection.x, 0, enemyDirection.z);
            player.transform.forward = EnemyDir;
        }
        else
        {
            EnemyDir = player.transform.forward;
        }
        switch (MeleeCombo)
        {
            case 1:
                if (/*!EnemyInRange() && */inputControls.canDash) { StartCoroutine(inputControls.ApplyDash(EnemyDir, 20, 0.15f, false, "MeleeSlide")); }
                //playerObj.GetComponent<Animator>().SetInteger("AttackChain", 1 +((MeleeSide + 1)% 2));
                break;
            case 2:
                if (/*!EnemyInRange() && */inputControls.canDash) { StartCoroutine(inputControls.ApplyDash(EnemyDir, 20, 0.15f, false, "MeleeSlide")); }
                //playerObj.GetComponent<Animator>().SetInteger("AttackChain", 1 + ((MeleeSide + 1) % 2));
                break;
            case 3:
                if (/*!EnemyInRange() && */inputControls.canDash) { StartCoroutine(inputControls.ApplyDash(EnemyDir, 40, 0.05f, false, "MeleeSlide")); }
                //playerObj.GetComponent<Animator>().SetInteger("AttackChain", 3);
                if (HealCombo)
                {
                    Health health = player.GetComponent<Health>();
                    if (health.currentHealth < 4) { health.LoseHealth(-1); StartCoroutine(health.HealHUD());  }
                }
                break;
        }
        //StartCoroutine(TimeOutAnimation());
        DealtDamage = false;
        RecentAttack = metronome.BeatsPassed;
        meleeHitbox.MeleeAttack(MeleeCombo);
    }

    IEnumerator TimeOutAnimation()
    {
        timeout = 0.75f;
        playerObj.GetComponent<Animator>().SetLayerWeight(1, 1);
        while (timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return new WaitForSeconds(0);
        }
        //Debug.Log("player attack animation timed out");
        playerObj.GetComponent<Animator>().SetTrigger("AttackTimeout");
        playerObj.GetComponent<Animator>().SetInteger("AttackChain", 0);
        //MeleeSide = 2;

        float a = 1;
        while (a > 0)
        {
            a -= Time.deltaTime;
            playerObj.GetComponent<Animator>().SetLayerWeight(1, a);
            yield return new WaitForSeconds(0);
        }



        yield return null;
    }

}
