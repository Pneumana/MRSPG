using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    private Metronome metronome;
    private PlayerAttack playerAttack;
    private Energy energy;
    private int MeleeCombo;
    public Vector3 HitboxSize;

    void Start()
    {
        metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        playerAttack = GameObject.Find("Player").GetComponent<PlayerAttack>();
        energy = GameObject.Find("Player").GetComponent<Energy>();
        MeleeCombo = 1;
    }
    public void MeleeAttack(int meleeCombo)
    {
        Collider[] Hit = Physics.OverlapBox(transform.position, HitboxSize, transform.rotation);
        foreach (Collider collider in Hit)
        {
            Debug.Log(Hit.Length);
            if (collider.gameObject.TryGetComponent<EnemyBody>(out var enemyBody) && collider.gameObject.TryGetComponent<Enemy>(out var enemy))
            {
                playerAttack.DealtDamage = true;
                switch (MeleeCombo)
                {
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
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3[] vertices = GetRotatedCubeVertices(transform.position, transform.rotation, HitboxSize);

        for (int i = 0; i < 4; i++)
        {
            int nextIndex = (i + 1) % 4;
            Gizmos.DrawLine(vertices[i], vertices[nextIndex]);
            Gizmos.DrawLine(vertices[i + 4], vertices[nextIndex + 4]);
            Gizmos.DrawLine(vertices[i], vertices[i + 4]);
        }
    }
    private Vector3[] GetRotatedCubeVertices(Vector3 position, Quaternion rotation, Vector3 size)
    {
        Vector3[] vertices = new Vector3[8];

        Vector3 halfSize = size * 0.5f;

        vertices[0] = rotation * new Vector3(-halfSize.x, -halfSize.y, -halfSize.z) + position;
        vertices[1] = rotation * new Vector3(halfSize.x, -halfSize.y, -halfSize.z) + position;
        vertices[2] = rotation * new Vector3(halfSize.x, -halfSize.y, halfSize.z) + position;
        vertices[3] = rotation * new Vector3(-halfSize.x, -halfSize.y, halfSize.z) + position;

        vertices[4] = rotation * new Vector3(-halfSize.x, halfSize.y, -halfSize.z) + position;
        vertices[5] = rotation * new Vector3(halfSize.x, halfSize.y, -halfSize.z) + position;
        vertices[6] = rotation * new Vector3(halfSize.x, halfSize.y, halfSize.z) + position;
        vertices[7] = rotation * new Vector3(-halfSize.x, halfSize.y, halfSize.z) + position;

        return vertices;
    }
}
