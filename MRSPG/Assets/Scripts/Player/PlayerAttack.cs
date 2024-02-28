using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    #region variables
    public int MeleeCombo; //Will be out of date if read by other scripts
    public int RecentAttack;
    public bool DealtDamage;
    public GameObject AttackHitbox;
    private Metronome Metronome;
    private MeleeHitbox MeleeHitbox;
    public Controller control;
    private InputControls inputControls;
    #endregion
    void Start()
    {
        Metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        MeleeHitbox = GameObject.Find("MeleeHitbox").GetComponent<MeleeHitbox>();
        inputControls = GameObject.Find("Player").GetComponent<InputControls>();
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
        switch (MeleeCombo)
        {
            default:
                break;
            case 1:
                inputControls.AddPush(inputControls.moveDirection, 20, 0.98f);
                break;
            case 2:
                inputControls.AddPush(inputControls.moveDirection, 20, 0.98f);
                break;
            case 3:
                inputControls.AddPush(inputControls.moveDirection, 10, 0.98f);
                break;
        }
        DealtDamage = false;
        RecentAttack = Metronome.BeatsPassed;
        StartCoroutine(MeleeHitbox.MeleeAttack(MeleeCombo));
    }
}
