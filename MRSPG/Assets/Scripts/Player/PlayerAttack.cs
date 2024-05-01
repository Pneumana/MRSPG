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
    Coroutine endAnim;
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
        if (endAnim != null)
        {
            StopCoroutine(endAnim);
            endAnim = null;
        }
        if(endAnim==null)
            endAnim = StartCoroutine(TimeOutAnimation());


        playerObj.GetComponent<Animator>().SetLayerWeight(1, 1);
        if (RecentAttack == metronome.BeatsPassed) { MeleeCombo = 1; HealCombo = true; return; } //anti button spam

        playerObj.GetComponent<Animator>().SetTrigger("Attacking");

        if (RecentAttack + 2 <= metronome.BeatsPassed || RecentAttack == metronome.BeatsPassed || !DealtDamage || MeleeCombo == 3) //reset melee combo conditions
        {
            if(MeleeCombo!=1)
                Debug.LogWarning("reset combo count " + MeleeCombo + " to 1");
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
        playerObj.GetComponent<Animator>().SetInteger("AttackChain", MeleeCombo);
        Debug.LogWarning("processing melee combo " + MeleeCombo);

        if (MeleeCombo == 1)
        {
            playerObj.GetComponent<Animator>().Play("SwingA", 1, 0);
        }

        switch (MeleeCombo)
        {
            case 1:
                if (/*!EnemyInRange() && */inputControls.canDash) { StartCoroutine(inputControls.ApplyDash(EnemyDir, 20, 0.15f, false, "MeleeSlide")); }
                break;
            case 2:
                if (/*!EnemyInRange() && */inputControls.canDash) { StartCoroutine(inputControls.ApplyDash(EnemyDir, 20, 0.15f, false, "MeleeSlide")); }
                break;
            case 3:
                if (/*!EnemyInRange() && */inputControls.canDash) { StartCoroutine(inputControls.ApplyDash(EnemyDir, 40, 0.05f, false, "MeleeSlide")); }
                if (HealCombo)
                {
                    Health health = player.GetComponent<Health>();
                    if (health.currentHealth < 4) { health.LoseHealth(-1); StartCoroutine(health.HealHUD());  }
                }
                break;
        }
        
        DealtDamage = false;
        RecentAttack = metronome.BeatsPassed;
        meleeHitbox.MeleeAttack(MeleeCombo);
    }

    IEnumerator TimeOutAnimation()
    {
        timeout = Metronome.inst.GetInterval();
        if (MeleeCombo == 3) timeout *= 0.25f;
        playerObj.GetComponent<Animator>().SetLayerWeight(1, 1);
        while (timeout > 0)
        {
            timeout -= Time.deltaTime;
            /*if (timeout > Metronome.inst.GetInterval()/2)
                player.GetComponent<InputControls>().velocity = new Vector3(0, player.GetComponent<InputControls>().velocity.y, 0);*/
            yield return new WaitForSeconds(0);
        }
        
        
        float a = 0;
        while (a < Metronome.inst.GetInterval())
        {
            a += Time.deltaTime;
            float zeroToOne = a / Metronome.inst.GetInterval();
            zeroToOne = Mathf.Clamp01(zeroToOne);
            playerObj.GetComponent<Animator>().SetLayerWeight(1, 1 - zeroToOne);
            Debug.LogWarning("attack animation layer fade " + zeroToOne);
            yield return new WaitForSeconds(0);
        }



        yield return null;
    }

}
