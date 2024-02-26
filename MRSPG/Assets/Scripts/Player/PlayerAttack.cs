using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    #region variables
    public int MeleeCombo; //Only updates when an attack is attempted
    public int RecentAttack;
    public GameObject AttackHitbox;
    private Metronome Metronome;
    private MeleeHitbox MeleeHitbox;
    public Controller control;
    #endregion
    void Start()
    {
        Metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        MeleeHitbox = GameObject.Find("MeleeHitbox").GetComponent<MeleeHitbox>();
        MeleeCombo = 0;
    }
    public void Attack(InputAction.CallbackContext context) //Starts melee attack and updates melee combo
    {
        if (RecentAttack == Metronome.BeatsPassed) { return; }
        if (RecentAttack + 2 <= Metronome.BeatsPassed || MeleeCombo == 3)
        {
            MeleeCombo = 1;
        }
        else
        {
            MeleeCombo++;
        }
        RecentAttack = Metronome.BeatsPassed;
        StartCoroutine(MeleeHitbox.MeleeAttack(MeleeCombo));
    }
}
