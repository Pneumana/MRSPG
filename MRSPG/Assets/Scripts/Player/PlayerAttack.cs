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
    public bool ToggleVisualBug; //To prevent possible side effects. will be removed when bug fixed
    public int HealCombo;
    private Vector3 EnemyDir;
    private Metronome metronome;
    private MeleeHitbox meleeHitbox;
    public CharacterController controller;
    private InputControls inputControls;
    private LockOnSystem lockOnSystem;
    private GameObject player;
    private GameObject playerObj;
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
        HealCombo = 0;
    }
    public void Attack(InputAction.CallbackContext context) //Starts melee attack and updates melee combo
    {

        if (RecentAttack == metronome.BeatsPassed) { MeleeCombo = 1; HealCombo = 0; return; }
        if (RecentAttack + 2 <= metronome.BeatsPassed || RecentAttack == metronome.BeatsPassed || !DealtDamage || MeleeCombo == 3)
        {
            MeleeCombo = 1;
            HealCombo = 0;
        }
        else
        {
            MeleeCombo++;
            if (Metronome.inst.IsOnBeat()) { HealCombo++; }
            else { HealCombo = 0; }
        }
        if (lockOnSystem.trackedEnemy != null && !lockOnSystem.freeAim)
        {
            Vector3 EnemyDirection = (lockOnSystem.trackedEnemy.transform.position - player.transform.position).normalized;
            EnemyDir = new Vector3(EnemyDirection.x, 0, EnemyDirection.z);
            if (ToggleVisualBug) { playerObj.transform.forward = EnemyDir; } //If this is Player, the direction is correct for only 1 dash and a few things break but the player faces the correct direction. If this is PlayerObj, the dash is always correct but the player rotation gets messed up
        }
        else
        {
            EnemyDir = playerObj.transform.forward;
        }
        switch (MeleeCombo)
        {
            case 1:
                if (!EnemyInRange() && inputControls.canDash) { StartCoroutine(inputControls.ApplyDash(EnemyDir, 30, 0.05f, false, "MeleeSlide")); }
                break;
            case 2:
                if (!EnemyInRange() && inputControls.canDash) { StartCoroutine(inputControls.ApplyDash(EnemyDir, 30, 0.05f, false, "MeleeSlide")); }
                break;
            case 3:
                if (!EnemyInRange() && inputControls.canDash) 
                {
                    StartCoroutine(inputControls.ApplyDash(EnemyDir, 30, 0.05f, false, "MeleeSlide"));
                    if (HealCombo == 3) 
                    {
                        Health health = player.GetComponent<Health>();
                        if(health.currentHealth < 4) { health.LoseHealth(-1); }
                    }
                }
                break;
        }
        DealtDamage = false;
        RecentAttack = metronome.BeatsPassed;
        StartCoroutine(meleeHitbox.MeleeAttack(MeleeCombo));
    }

    public bool EnemyInRange()
    {
        return Physics.CheckBox(meleeHitbox.transform.position, new Vector3(1.7f, 1.4f, 1), meleeHitbox.transform.rotation, inputControls.enemyLayer);//hitbox size value temporarily hard-coded for testing, if hitbox size changes this will be inaccurate.
    }
}
