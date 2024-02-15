using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public int MeleeCombo;
    public int AttackDelay;
    public GameObject AttackHitbox;
    public Controller control;
    void Start()
    {
        MeleeCombo = 0;
        AttackHitbox.SetActive(false);
    }

    void Update()
    {
        
    }

    public void Attack(InputAction.CallbackContext context)
    {
        Debug.Log("Melee");
        AttackHitbox.SetActive(true);
    }
}
