using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public int MeleeCombo;
    public int RecentAttack;
    public GameObject AttackHitbox;
    private Metronome Metronome;
    private MeleeHitbox MeleeHitbox;
    public Controller control;
    void Start()
    {
        Metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        MeleeHitbox = GameObject.Find("MeleeHitbox").GetComponent<MeleeHitbox>();
        MeleeCombo = 1;
    }

    void Update()
    {
        
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if(RecentAttack == Metronome.BeatsPassed) { return; }
        if (RecentAttack + 2 <= Metronome.BeatsPassed)
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
