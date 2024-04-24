using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;
using TMPro;

public class PhotosensWarning : MonoBehaviour
{
    [SerializeField] ControllerSupport controls;
    [SerializeField] PlayableDirector dissolve;
    public Canvas canvas;
    bool played;
    public static PhotosensWarning inst;
    [SerializeField] private InputActionReference input;
    [SerializeField] private TextMeshProUGUI textmesh;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            if (inst != this)
                Destroy(canvas.gameObject);
        }
        controls = new ControllerSupport();
        textmesh.text = "Press " + GetBinding() + " to continue.";
        
        foreach (InputBinding i in input.action.bindings)
        {
            Debug.Log(i);
        }
    }
    private void OnEnable()
    {
        controls.Gameplay.Enable();

    }
    private void Update()
    {
        if(controls.Gameplay.Dash.WasPressedThisFrame() && !played)
        {
            StartCoroutine(SensWarning());
        }
    }

    private String GetBinding()
    {

        //InputBinding binding = input.action.bindings.FirstOrDefault();
        //return binding.ToDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
        return input.action.GetBindingDisplayString();
    }

    IEnumerator SensWarning()
    {
        played = true;
        dissolve.Play();
        canvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}
