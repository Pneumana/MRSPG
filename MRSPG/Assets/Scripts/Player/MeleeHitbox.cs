using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    private Metronome Metronome;
    private BoxCollider BoxCollider;
    public int MeleeCombo;

    void Start()
    {
        Metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        BoxCollider = GetComponent<BoxCollider>();
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
        if (!collision.gameObject.TryGetComponent<EnemyBody>(out var enemyBody)) { return; }
        if (!collision.gameObject.TryGetComponent<Enemy>(out var enemy)) { return; }
        switch (MeleeCombo) {
            default:
                break;
            case 1:
                enemyBody.ModifyHealth(-1);
                break;
            case 2:
                enemyBody.ModifyHealth(-1);
                break;
            case 3:
                enemyBody.ModifyHealth(-2);
                break;
        }
        if (Metronome.IsOnBeat()) { StopCoroutine(enemy.StartAttack(enemy._enemy.pattern)); } //inturrupts enemy attack

    }
}
