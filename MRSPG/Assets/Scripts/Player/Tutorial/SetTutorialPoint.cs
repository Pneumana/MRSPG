using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class SetTutorialPoint : MonoBehaviour
{
    public string text;
    public bool disableAction;
    public bool enableAction;
    public InputActionReference[] thisAction;
    public InputActionReference DisplayThisAction;

    private void Start()
    {
    }
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("SetTutorialPoint was entered");
        if (collision.CompareTag("Player"))
        {
            if(disableAction)
            {
                DisableActionInput();
            }else if(enableAction)
            {
                EnableActionInput();
            }
        }
    }

    public void EnableActionInput()
    {
        Controller.inst.controls.FindAction(DisplayThisAction.name).Enable();
    }

    public void DisableActionInput()
    {
        foreach(InputActionReference iar in thisAction)
        {
            Controller.inst.controls.FindAction(iar.name).Disable();
        }
    }
}
