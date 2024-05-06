using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    #region
    public static Sounds instance;

    public AudioClip sfx_hitMarker, sfx_onBeat, sfx_teleport, sfx_shoot, sfx_dash, sfx_enemyHit, sfx_enemyCharge, sfx_bossHit, sfx_enemyShoot;
    public GameObject soundObject;
    public GameObject soundLoop;

    #endregion

    private void Awake ( )
    {
        instance = this;
    }

    public void PlaySFX(string sfxName )
    {
        switch ( sfxName )
        {
            case "Hit Marker":
                SoundObjectCreation ( sfx_hitMarker );
                break;
            case "On Beat":
                SoundObjectCreation ( sfx_onBeat );
                break;
            case "Teleport":
                SoundObjectCreation ( sfx_teleport );
                break;
            case "Shoot":
                SoundObjectCreation ( sfx_shoot );
                break;
            case "Dash":
                SoundObjectCreation ( sfx_dash );
                break;
            case "Enemy Hit Marker":
                SoundObjectCreation ( sfx_enemyHit );
                break;
            case "Enemy Charge":
                SoundObjectCreation ( sfx_enemyCharge );
                break;
            case "Boss Hit Marker":
                SoundObjectCreation ( sfx_bossHit );
                break;
            case "Enemy Shoot":
                SoundObjectCreation ( sfx_enemyShoot );
                break;
            default:
                break;
        }
    }

    void SoundObjectCreation(AudioClip clip )
    {
        GameObject newobject = Instantiate ( soundObject , transform );
        newobject.GetComponent<AudioSource> ( ).clip = clip;
        newobject.GetComponent<AudioSource> ( ).Play ( );
    }

    void SoundObjectCreationLoop (AudioClip clipLoop )
    {
        GameObject newobject = Instantiate ( soundObject , transform );
        newobject.GetComponent<AudioSource> ( ).clip = clipLoop;
        newobject.GetComponent<AudioSource> ( ).Play ( );
    }
}
