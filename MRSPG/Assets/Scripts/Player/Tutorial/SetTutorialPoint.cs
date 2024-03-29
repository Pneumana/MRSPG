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
    public bool pauseForTutorial;

    [Space(10)]
    [Header("Enable or Disable an action")]
    public bool enableAction;
    public bool disableAction;

    [Space(10)]
    [Header("Actions")]
    public InputActionReference[] actionsToDisable;
    public InputActionReference actionToEnable;

    public void EnableActionInput()
    {
        Controller.inst.controls.FindAction(actionToEnable.name).Enable();
    }

    public void DisableActionInput()
    {
        foreach(InputActionReference iar in actionsToDisable)
        {
            Controller.inst.controls.FindAction(iar.name).Disable();
        }
    }
}
