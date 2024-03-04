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
    public bool ToggleVisualBug; //To prevent possible side effects, will be removed when bug fixed
    public GameObject AttackHitbox;
    private Metronome Metronome;
    private MeleeHitbox MeleeHitbox;
    public Controller control;
    private InputControls inputControls;
    private LockOnSystem lockOnSystem;
    private GameObject Player;
    #endregion
    void Start()
    {
        Metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        MeleeHitbox = GameObject.Find("MeleeHitbox").GetComponent<MeleeHitbox>();
        inputControls = GameObject.Find("Player").GetComponent<InputControls>();
        lockOnSystem = GameObject.Find("TimeScaler").GetComponent<LockOnSystem>();
        Player = GameObject.Find("PlayerObj");
        MeleeCombo = 0;
    }
    public void Attack(InputAction.CallbackContext context) //Starts melee attack and updates melee combo
    {
        if (RecentAttack == Metronome.BeatsPassed) { return; }
        if (RecentAttack + 2 <= Metronome.BeatsPassed || !DealtDamage || MeleeCombo == 3)
        {
            MeleeCombo = 1;
        }
        else
        {
            MeleeCombo++;
        }
        if (lockOnSystem.trackedEnemy != null && ToggleVisualBug) 
        { 
            Vector3 EnemyDirection = (lockOnSystem.trackedEnemy.transform.position - Player.transform.position).normalized;
            Player.transform.forward = new Vector3(EnemyDirection.x, 0, EnemyDirection.z);
        }
        switch (MeleeCombo)
        {
            case 1:
                if (!EnemyInRange()) { inputControls.AddPush(Player.transform.forward, 30, 0.98f); }
                break;
            case 2:
                if (!EnemyInRange()) { inputControls.AddPush(Player.transform.forward, 30, 0.98f); }
                break;
            case 3:
                if (!EnemyInRange()) { inputControls.AddPush(Player.transform.forward, 30, 0.98f); }
                break;
        }
        DealtDamage = false;
        RecentAttack = Metronome.BeatsPassed;
        StartCoroutine(MeleeHitbox.MeleeAttack(MeleeCombo));
    }

    public bool EnemyInRange()
    {
        return Physics.CheckBox(MeleeHitbox.transform.position, new Vector3(1.7f, 1.4f, 1), MeleeHitbox.transform.rotation, inputControls.enemyLayer);//hitbox size value temporarily hard-coded for testing, if hitbox size changes this will be inaccurate.
    }
}
