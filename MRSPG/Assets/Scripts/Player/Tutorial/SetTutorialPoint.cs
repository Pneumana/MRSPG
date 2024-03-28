using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class SetTutorialPoint : MonoBehaviour
{
    public string text;
    public bool disableAction;
    public bool enableAction;
    public InputActionReference thisAction;
    public InputActionAsset asset;

    private void OnTriggerEnter(Collider collision)
    {
        if (enableAction && collision.CompareTag("Player"))
        {
            Controller.inst.controls.FindAction(thisAction.name).Enable();
            Debug.LogWarning(thisAction.name + " is being enabled.");
        }
    }
}
