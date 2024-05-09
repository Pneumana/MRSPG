using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    #region variables

    [HideInInspector] public int _maxHealth = 5;
    public int currentHealth;

    [SerializeField]
    Image _healthImg;

    [SerializeField]
    Sprite [ ] _healthSprite;

    [HideInInspector] public CheckpointObelisk currentCheckpoint;

    //connor vars :D
    [SerializeField] float hurtDisplayTime = 0.27f;
    [SerializeField] float hurtFadeTime = 1;

    [SerializeField] ScriptableRendererFeature damageHUD;
    [SerializeField] Material mat;

    [SerializeField] ScriptableRendererFeature healHUD;
    [SerializeField] Material mat2;

    #endregion

    private void Awake()
    {
        currentHealth = _maxHealth;
        damageHUD.SetActive(false);
    }

    IEnumerator HurtHUD()
    {
        damageHUD.SetActive(true);
        healHUD.SetActive(false);
        mat.SetFloat("_Fade", 0);
        yield return new WaitForSecondsRealtime(hurtDisplayTime);

        float t = 0;
        do
        {
            t += Time.unscaledDeltaTime;
            mat.SetFloat("_Fade", t);
            yield return new WaitForSecondsRealtime(0);
        } while (t < hurtFadeTime);

        damageHUD.SetActive(false);

        yield return null;
    }

    public IEnumerator HealHUD()
    {
        healHUD.SetActive(true);
        damageHUD.SetActive(false);
        mat2.SetFloat("_Fade", 0);
        yield return new WaitForSecondsRealtime(hurtDisplayTime);

        float t = 0;
        do
        {
            t += Time.unscaledDeltaTime;
            mat2.SetFloat("_Fade", t);
            yield return new WaitForSecondsRealtime(0);
        } while (t < hurtFadeTime);

        healHUD.SetActive(false);

        yield return null;
    }

    private void Update ( )
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            LoseHealth(-1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            LoseHealth(1);
        }
        UIUpdateHealth ( );
    }

    public void LoseHealth(int amount)
    {
        StopAllCoroutines();
        currentHealth -= amount;
        if(amount > 0)
        {
            StartCoroutine(HurtHUD());
        }
        else
        {
            StartCoroutine(HealHUD());
        }
        UIUpdateHealth();
    }

    void UIUpdateHealth ( )
    {
        _healthImg.sprite = _healthSprite[_healthSprite.Length - (currentHealth + 1)];

        /*if ( currentHealth == 5 )
        {
            _healthImg.sprite = _healthSprite [ 0 ];
        }
        else if ( currentHealth == 4 )
        {
            _healthImg.sprite = _healthSprite [ 1 ];
        }
        else if ( currentHealth == 3 )
        {
            _healthImg.sprite = _healthSprite [ 2 ];
        }
        else if ( currentHealth == 2 )
        {
            _healthImg.sprite = _healthSprite [ 3 ];
        }
        else if ( currentHealth == 1 )
        {
            _healthImg.sprite = _healthSprite [ 4 ];
        }*/
        if ( currentHealth == 0 )
        {
            _healthImg.sprite = _healthSprite [ 5 ];
            StartCoroutine(Die());
        }
    }

    public IEnumerator Die()
    {

        Debug.Log("player died");
        if (currentCheckpoint != null)
        {
            var obj = GameObject.Find("PlayerObj");
            var input = GameObject.Find("Player").GetComponent<InputControls>();
            currentCheckpoint.GetComponent<CheckpointObelisk>().RespawnReset();
            input.velocity = Vector3.zero;
            input.controller.enabled = false;


            var temp = -currentCheckpoint.transform.parent.GetChild(0).transform.forward;
            temp.y = 0;
            var look = Quaternion.LookRotation(temp, Vector3.up);
            obj.transform.forward = temp;
            Debug.Log("Dead: changing camera angle to " + look.eulerAngles.y);
            GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>().m_XAxis.Value = look.eulerAngles.y - 180;
            GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>().m_YAxis.Value = 0.5f;

            obj.transform.position = currentCheckpoint.spawnPosition + Vector3.up * 1.5f;
            input.controller.enabled = true;
            foreach(BattleBounds bb in FindObjectsByType<BattleBounds>(FindObjectsSortMode.None))
            {
                bb.RespawnAll();
            }
            GameObject.Find("PlayerCam").GetComponent<CinemachineInputProvider>().enabled = true;
            GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>().LookAt = obj.transform;
            

            GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>().Follow = obj.transform;
            input.doMovement = true;
        }
        else
        {
            Debug.Log("no checkpoint to respawn the player at!");
        }
        LockOnSystem.LOS.StopLockOn();
        currentHealth = 5;
        UIUpdateHealth();
        yield return null;
    }

    private void OnDestroy()
    {
        mat.SetFloat("_Fade", 1);
    }
}