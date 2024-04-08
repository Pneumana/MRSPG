using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;
using static UnityEngine.InputSystem.InputAction;

public class Controller : MonoBehaviour
{
    public static Controller inst;
    public InputControls movement;
    public LockOnSystem lockOnSystem;
    public Gun gun;
    public PlayerAttack playerAttack;
    public PauseMenu pauseUI;
    public Tutorial Tutorial;
    [HideInInspector] public ControllerSupport controls;

    public Vector2 lookInput;

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
            Destroy(gameObject);

        controls = new ControllerSupport();
        controls.Gameplay.Move.performed += ctx => movement.playerInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => movement.playerInput = Vector2.zero;

        controls.Gameplay.Camera.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Camera.canceled += ctx => lookInput = Vector2.zero;

        controls.Gameplay.Jump.performed += movement.OnJump;
        controls.Gameplay.Dash.performed += movement.OnDash;
        controls.Gameplay.Fire.performed += gun.Shoot;
        controls.Gameplay.Attack.performed += playerAttack.Attack;
        if (pauseUI != null) { lockOnSystem.paused = true; }
        if (Tutorial != null)
        {
            controls.FreezeActions.TutorialConfirm.started += Tutorial.BeginHoldAnim;
            controls.FreezeActions.TutorialConfirm.performed += Tutorial.Resume;
            controls.FreezeActions.TutorialConfirm.canceled += Tutorial.BeginCancelledAnim;
        }
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();

    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
        Debug.LogWarning("The controls have been disabled");
    }
}
