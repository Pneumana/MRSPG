using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    #region variables
    public int MeleeCombo; //Will be out of date if read by other scripts
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
        MeleeCombo = 1;
        HealCombo = true;
    }
    public void Attack(InputAction.CallbackContext context) //Starts melee attack and updates melee combo
    {
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

        timeout = 0.75f;
        StartCoroutine(TimeOutAnimation());
        playerObj.GetComponent<Animator>().SetInteger("AttackChain", MeleeCombo);
        switch (MeleeCombo)
        {
            case 1:
                
                if (/*!EnemyInRange() && */inputControls.canDash) { StartCoroutine(inputControls.ApplyDash(EnemyDir, 30, 0.05f, false, "MeleeSlide")); }
                break;
            case 2:
                
                if (/*!EnemyInRange() && */inputControls.canDash) { StartCoroutine(inputControls.ApplyDash(EnemyDir, 30, 0.05f, false, "MeleeSlide")); }
                break;
            case 3:
                
                if (/*!EnemyInRange() && */inputControls.canDash) { StartCoroutine(inputControls.ApplyDash(EnemyDir, 30, 0.05f, false, "MeleeSlide")); }
                if (HealCombo)
                {
                    Health health = player.GetComponent<Health>();
                    if (health.currentHealth < 4) { health.LoseHealth(-1); }
                }
                break;
        }
        DealtDamage = false;
        RecentAttack = metronome.BeatsPassed;
        meleeHitbox.MeleeAttack(MeleeCombo);
    }

    IEnumerator TimeOutAnimation()
    {
        while(timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return new WaitForSeconds(0);
        }
        Debug.Log("player attack animation timed out");
        playerObj.GetComponent<Animator>().SetTrigger("AttackTimeout");
        playerObj.GetComponent<Animator>().SetInteger("AttackChain", 0);
        yield return null;
    }

}
