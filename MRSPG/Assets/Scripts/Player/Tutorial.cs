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

public class Tutorial : MonoBehaviour
{
    [Header("Text Object")]
    public GameObject TutorialScreen;
    public TextMeshProUGUI tutorialText;
    public List<string> strings = new List<string>();


    [Header("Animations")]
    public Image Hold;


    [Header("Player")]
    public InputControls player;
    public CinemachineFreeLook cam;
    float firstSpeed;
    float stopped = 0f;

    private int index;
    public bool brokeHold;

    private void Start()
    {
        firstSpeed = player.speed;
        Controller.inst.controls.Gameplay.TutorialConfirm.Disable();
        TutorialScreen.SetActive(false);
        strings.Add("Shorter walls allow you to jump up onto them. Walk near the wall and jump to grab the ledge.");
        strings.Add("Checkpoints are marked as the glowing obelisks.");
        strings.Add("Try doing a delayed jump, walk off the ledge and jump before you hit the ground to gain distance.");
        strings.Add("You can dash using " + "O" + ", dash after jumping to maintain height.");
        strings.Add("Objects with an orange reticle will allow you to swap places with them and slow down time, swapping uses 20 energy. You can swap using " + "Left Trigger");
        strings.Add("more tutorial this way yeaaaaaaaaaaaaaaaah");
        strings.Add("Swapping with some objects can damage or kill an enemy.");
        strings.Add("Dashing into an enemy will shove them, shoving them into a wall damages them.");
        strings.Add("Enemies take fall damage. You don't.");
    }

    private void OnTriggerEnter(Collider collision)
    {
        Paused();
        if(collision.CompareTag("TutorialTrigger"))
        {
            tutorialText.text = strings[index];
            TutorialScreen.SetActive(true);
            index++;
        }
        Destroy(collision.gameObject, 0.1f);
    }


    public void Paused()
    {
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
