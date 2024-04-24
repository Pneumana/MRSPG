using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class CutsceneLogic : MonoBehaviour
{
    private PlayableDirector playableDirector;
    public GameObject playerCam;
    public bool ActiveCutscene = false;
    bool SceneHasCutscenes;
    void Start()
    {
        if (GameObject.Find("Main Camera").GetComponent<PlayableDirector>())
        {
            SceneHasCutscenes = true;
            playableDirector = GameObject.Find("Main Camera").GetComponent<PlayableDirector>(); 
        }
        playerCam = GameObject.Find("PlayerCam");
    }
    private void Update()
    {
        if(SceneHasCutscenes)
        {
            if (playableDirector.state == PlayState.Playing) 
            {
                if (ActiveCutscene == true) { return; }
                ActiveCutscene = true; 
            }
            else 
            {
                if (ActiveCutscene == false) { return; }
                ActiveCutscene = false; 
            }
        }
    }
    public void SkipCutscene(InputAction.CallbackContext context)
    {
        playableDirector.Stop();
        playerCam.GetComponent<Cinemachine.CinemachineFreeLook>().enabled = true;
    }
}