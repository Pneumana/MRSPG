using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class CutsceneLogic : MonoBehaviour
{
    private PlayableDirector playableDirector;
    private ControllerSupport controls;
    public bool ActiveCutscene = false;
    void Start()
    {
        playableDirector = GameObject.Find("Main Camera").GetComponent<PlayableDirector>();
    }
    private void Update()
    {
        if (playableDirector.state == PlayState.Playing) 
        {
            if (ActiveCutscene == true) { return; }
            Debug.LogWarning("Frozen");
            ActiveCutscene = true; 
        }
        else 
        {
            if (ActiveCutscene == false) { return; }
            Debug.LogWarning("Unfrozen");
            ActiveCutscene = false; 
        }
        if (controls.Gameplay.Dash.WasPressedThisFrame())
        {
            Debug.Log("Cutscene Skipped");
            playableDirector.Stop();
        }
    }
}