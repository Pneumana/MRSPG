using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    private Metronome metronome;
    private BoxCollider boxCollider;
    private PlayerAttack playerAttack;
    private Energy energy;
    private int MeleeCombo;

    void Start()
    {
        metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        playerAttack = GameObject.Find("Player").GetComponent<PlayerAttack>();
        energy = GameObject.Find("Player").GetComponent<Energy>();
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
        MeleeCombo = 1;
    }
    public IEnumerator MeleeAttack(int meleeCombo) //enables collider and reads MeleeCombo
    {
        MeleeCombo = meleeCombo;
        boxCollider.enabled = true;
        yield return new WaitForSeconds(0.05f);
        boxCollider.enabled = false;
    }

    void OnTriggerEnter(UnityEngine.Collider collision) //runs once per enemy in collider
    {
        if (!collision.gameObject.TryGetComponent<EnemyBody>(out var enemyBody)) { return; }
        if (!collision.gameObject.TryGetComponent<Enemy>(out var enemy)) { Debug.LogError("No Enemy script found on hit enemy!"); return; }
        playerAttack.DealtDamage = true;
        switch (MeleeCombo) {
            default:
                Debug.LogError("Invalid value for MeleeCombo: " + MeleeCombo);
                break;
            case 1:
                enemyBody.ModifyHealth(1);
                if (Metronome.inst.IsOnBeat()) { energy.GainEnergy(1); }
                break;
            case 2:
                enemyBody.ModifyHealth(1);
                if (Metronome.inst.IsOnBeat()) { energy.GainEnergy(2); }
                break;
            case 3:
                enemyBody.ModifyHealth(2);
                if (Metronome.inst.IsOnBeat()) { energy.GainEnergy(5); }
                break;
        }
        if (metronome.IsOnBeat() && enemy._enemy.type != EnemyType.Heavy) { enemy.Stagger(); }
    }
}
