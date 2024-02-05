using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static UnityEngine.InputSystem.InputAction;

public class Controller : MonoBehaviour
{
    public static Controller inst;
    public InputControls movement;
    public LockOnSystem lockOnSystem;
    ControllerSupport controls;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        controls = new ControllerSupport();
        controls.Gameplay.Move.performed += ctx => movement.playerInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => movement.playerInput = Vector2.zero;
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Jump.performed += movement.OnJump;
        controls.Gameplay.Dash.performed += movement.OnDash;
        controls.Gameplay.Slowdown.performed += lockOnSystem.ConfigureLockOn;
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}
