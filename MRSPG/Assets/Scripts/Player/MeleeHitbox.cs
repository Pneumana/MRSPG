using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    private Metronome Metronome;
    private BoxCollider BoxCollider;
    private PlayerAttack PlayerAttack;
    private Energy Energy;
    private int MeleeCombo;

    void Start()
    {
        Metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        PlayerAttack = GameObject.Find("Player").GetComponent<PlayerAttack>();
        Energy = GameObject.Find("Player").GetComponent<Energy>();
        BoxCollider = GetComponent<BoxCollider>();
        BoxCollider.enabled = false;
        MeleeCombo = 1;
    }
    public IEnumerator MeleeAttack(int meleeCombo) //enables collider and reads MeleeCombo
    {
        MeleeCombo = meleeCombo;
        BoxCollider.enabled = true;
        yield return new WaitForSeconds(0.05f);
        BoxCollider.enabled = false;
    }

    void OnTriggerEnter(UnityEngine.Collider collision) //runs once per enemy in collider
    {
        if (!collision.gameObject.TryGetComponent<EnemyBody>(out var enemyBody)) { Debug.Log("Hit non enemy"); return; }
        else { Debug.Log("Hit enemy"); }
        if (!collision.gameObject.TryGetComponent<Enemy>(out var enemy)) { Debug.LogError("No Enemy script found on hit enemy!"); return; }
        PlayerAttack.DealtDamage = true;
        switch (MeleeCombo) {
            default:
                Debug.LogError("Invalid value for MeleeCombo: " + MeleeCombo);
                break;
            case 1:
                enemyBody.ModifyHealth(1);
                if (Metronome.inst.IsOnBeat()) { Energy.GainEnergy(1); }
                break;
            case 2:
                enemyBody.ModifyHealth(1);
                if (Metronome.inst.IsOnBeat()) { Energy.GainEnergy(2); }
                break;
            case 3:
                enemyBody.ModifyHealth(2);
                if (Metronome.inst.IsOnBeat()) { Energy.GainEnergy(5); }
                break;
        }
        if (Metronome.IsOnBeat() !& enemy._enemy.type == EnemyType.Heavy) { StopCoroutine(enemy.StartAttack(enemy._enemy.pattern)); } //inturrupts enemy attack
    }
}
