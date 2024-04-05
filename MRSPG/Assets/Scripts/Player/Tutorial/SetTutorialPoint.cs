using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class SetTutorialPoint : MonoBehaviour
{
    [TextArea(10, 10)]
    public string text;
    public bool pauseForTutorial;

    [Space(10)]
    [Header("Enable or Disable an action")]
    public bool enableAction;
    public bool disableAction;

    [Space(10)]
    [Header("Actions")]
    public InputActionReference[] actionsToDisable;
    public InputActionReference[] actionsToEnable;


    public void EnableActionInput()
    {
        foreach (InputActionReference iar in actionsToEnable)
        {
            Debug.Log("Enabled" + iar.name);
            Controller.inst.controls.FindAction(iar.name).Enable();
        }
    }

    public void DisableActionInput()
    {
        foreach(InputActionReference iar in actionsToDisable)
        {
            Debug.Log("Disabled" + iar.name);
            Controller.inst.controls.FindAction(iar.name).Disable();
        }
    }
}
