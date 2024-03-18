using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using static UnityEngine.InputSystem.InputAction;

public class Controller : MonoBehaviour
{
    public static Controller inst;
    public InputControls movement;
    public LockOnSystem lockOnSystem;
    public Gun gun;
    public PlayerAttack playerAttack;
    public UIManager pauseUI;
    [HideInInspector] public ControllerSupport controls;

    public Vector2 lookInput;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if(inst!=this)
                Destroy(gameObject);
        }

        controls = new ControllerSupport();
        controls.Gameplay.Move.performed += ctx => movement.playerInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => movement.playerInput = Vector2.zero;

        controls.Gameplay.Camera.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Camera.canceled += ctx => lookInput = Vector2.zero;
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Jump.performed += movement.OnJump;
        controls.Gameplay.Dash.performed += movement.OnDash;
        controls.Gameplay.Fire.performed += gun.Shoot;
        if (pauseUI != null) { controls.Gameplay.NavMenuDown.performed += pauseUI.MakeSelection; }
        controls.Gameplay.NavMenuup.performed += pauseUI.MakeSelection;
        controls.Gameplay.Attack.performed += playerAttack.Attack;
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}
