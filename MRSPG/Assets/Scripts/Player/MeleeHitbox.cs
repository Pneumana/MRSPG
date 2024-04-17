using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeHitbox : MonoBehaviour
{
    #region Variables
    private Metronome metronome;
    private PlayerAttack playerAttack;
    private InputControls inputControls;
    private Energy energy;
    public Vector3 HitboxSize;

    public List<EnemyBody> hurtList = new List<EnemyBody>();
    #endregion

    void Start()
    {
        metronome = GameObject.Find("Metronome").GetComponent<Metronome>();
        playerAttack = GameObject.Find("Player").GetComponent<PlayerAttack>();
        inputControls = GameObject.Find("Player").GetComponent<InputControls>();
        energy = GameObject.Find("Player").GetComponent<Energy>();
    }
    public void MeleeAttack(int meleeCombo)
    {
        
        Collider[] Hit = Physics.OverlapBox(transform.position, HitboxSize, transform.rotation);
        foreach (Collider collider in Hit)
        {
            if (collider.gameObject.TryGetComponent<EnemyBody>(out var enemyBody) && collider.gameObject.TryGetComponent<Enemy>(out var enemy)) //checks if target collider is from an enemy, also gets target's scripts
            {
                playerAttack.DealtDamage = true;
                switch (meleeCombo)
                {
                    default:
                        Debug.LogError("Invalid value for MeleeCombo: " + meleeCombo);
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

        //Added by connor
        hurtList.Clear();

    }
    private void OnDrawGizmos()
    {
        if (EnemyInRange()) { Gizmos.color = Color.blue; }
        else { Gizmos.color = Color.gray; }
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
    public bool EnemyInRange()
    {
        if (inputControls == null) return false;
        return Physics.CheckBox(transform.position, HitboxSize, transform.rotation, inputControls.enemyLayer);
    }
}
