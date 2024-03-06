using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    #region variables

    int _maxHealth = 5;
    public int currentHealth;

    [SerializeField]
    Image _healthImg;

    [SerializeField]
    Sprite [ ] _healthSprite;

    [HideInInspector] public CheckpointObelisk currentCheckpoint;
    #endregion

    //connor vars :D
    [SerializeField] float hurtDisplayTime = 0.27f;
    [SerializeField] float hurtFadeTime = 1;

    [SerializeField] ScriptableRendererFeature damageHUD;
    [SerializeField] Material mat;

    private void Awake()
    {
        currentHealth = _maxHealth;
        damageHUD.SetActive(false);
    }

    IEnumerator HurtHUD()
    {
        damageHUD.SetActive(true);
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

    private void Update ( )
    {
        
    }

    public void LoseHealth(int amount)
    {
        currentHealth -= amount;
        StartCoroutine(HurtHUD());
        UIUpdateHealth();
    }

    void UIUpdateHealth ( )
    {
        if ( currentHealth == 5 )
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
        }
        else if ( currentHealth == 0 )
        {
            _healthImg.sprite = _healthSprite [ 5 ];
            Die();
        }
    }

    public void Die()
    {
        if(currentCheckpoint!=null)
            GameObject.Find("PlayerObj").transform.position = currentCheckpoint.spawnPosition + Vector3.up;
        currentHealth = 5;
        UIUpdateHealth();
    }

}