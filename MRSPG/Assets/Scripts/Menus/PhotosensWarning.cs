using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class PhotosensWarning : MonoBehaviour
{
    [SerializeField] ControllerSupport controls;
    [SerializeField] PlayableDirector dissolve;
    public Canvas canvas;
    bool played;
    public static PhotosensWarning inst;

    private void Awake()
    {
        if (inst == null)
            inst = this;
        else
        {
            if (inst != this)
                Destroy(gameObject);
        }
        canvas.gameObject.SetActive(false);
        controls = new ControllerSupport();
        DontDestroyOnLoad(transform.root.gameObject);
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

    IEnumerator SensWarning()
    {
        played = true;
        dissolve.Play();
        canvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}
