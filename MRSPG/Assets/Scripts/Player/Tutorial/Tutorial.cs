using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;
using Unity.VisualScripting;
using Cinemachine;
using System;

public class Tutorial : MonoBehaviour
{
    [Header("Text Object")]
    public GameObject TutorialScreen;
    public TextMeshProUGUI tutorialText;
    public List<string> strings = new List<string>();
    public LockOnSystem lockon;


    [Header("Animations")]
    public Image Hold;


    [Header("Player")]
    public InputControls player;
    public CinemachineFreeLook cam;
    float firstSpeed;
    float stopped = 0f;

    public bool brokeHold;
    private SetTutorialPoint point;

    private void Start()
    {
        firstSpeed = player.speed;
        TutorialScreen.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("TutorialTrigger"))
        {
            point = collision.GetComponent<SetTutorialPoint>();
            if (point.enableAction)
            {
                point.EnableActionInput();
            }
            if(point.pauseForTutorial)
            {
                Paused();
                tutorialText.text = point.text;
                TutorialScreen.SetActive(true);
            }
            Destroy(collision.gameObject, 0.1f);
        }
    }


    public void Paused()
    {
        Debug.Log("paused");
        cam.enabled = false;
        Controller.inst.controls.Gameplay.Disable();
        Controller.inst.controls.FreezeActions.Enable();
        Cursor.visible = false;
        player.speed = stopped;
    }
    public void Resume(InputAction.CallbackContext context)
    {
        Hold.fillAmount = 0f;

        cam.enabled = true;
        Controller.inst.controls.Gameplay.Enable();
        Controller.inst.controls.FreezeActions.Disable();

        TutorialScreen.SetActive(false);
        Cursor.visible = true;
        player.speed = firstSpeed;

        if (point.disableAction)
        {
            point.DisableActionInput();
        }
    }

    IEnumerator HoldAnimation()
    {
        StopCoroutine(CancelledHoldAnimation());
        float amount = Hold.fillAmount;
        float time = 0f;
        while (time < 1f && !brokeHold)
        {
            time += Time.deltaTime;
            Hold.fillAmount = Mathf.Lerp(amount, 1f, (time / 1f));
            yield return null;
        }
    }

    IEnumerator CancelledHoldAnimation()
    {
        brokeHold = true;
        StopCoroutine(HoldAnimation());
        float amount = Hold.fillAmount;
        float time = 0f;
        while (time < 0.3f)
        {
            time += Time.deltaTime;
            
            Hold.fillAmount = Mathf.Lerp(amount, 0f, (time / 0.3f));

            yield return null;
        }
        Hold.fillAmount = 0f;
        brokeHold = false;
    }

    public void BeginHoldAnim(InputAction.CallbackContext context)
    {
        StartCoroutine(HoldAnimation());
    }

    public void BeginCancelledAnim(InputAction.CallbackContext context)
    {
        StartCoroutine(CancelledHoldAnimation());
    }
}
