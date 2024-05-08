using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System;


public class Settings : MonoBehaviour
{
    #region Variables

    [SerializeField] TMP_Dropdown _resDropdown;
    [SerializeField] TMP_Dropdown _antiAliasing; 
    [SerializeField] AudioMixer _gameMixer;
    [SerializeField] Slider _sfxSlider;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Toggle _fullscreenToggle;

    public float sfxVolumeValue, musicVolumeValue;
    public int antiAliasingValue, resolutionValue = 0;
    public bool fullscreen;

    /*
    [SerializeField] Toggle _noAA;
    [SerializeField] Toggle _2xAA;
    [SerializeField] Toggle _4xAA;
    [SerializeField] Toggle _8xAA;
    */

    Resolution [ ] _resolutions;

    #endregion

    private void Start ( )
    {
        _sfxSlider.value = PlayerPrefs.GetFloat ( "SFX Volume" );
        _musicSlider.value = PlayerPrefs.GetFloat ( "Music Volume" );
        _antiAliasing.value = PlayerPrefs.GetInt ( "Anti Aliasing" );
        _fullscreenToggle.isOn = PlayerPrefs.GetInt ( "Fullscreen" ) > 0;
        _resDropdown.value = PlayerPrefs.GetInt ( "Resolution" );
        GetResolutions ( );
    }

    void GetResolutions ( )
    {
        _resolutions = Screen.resolutions;

        _resDropdown.ClearOptions ( );

        List<string> options = new List<string> ( );

        for ( int i = 0 ; i < _resolutions.Length ; i++ )
        {
            string option = _resolutions [ i ].width + "x" + _resolutions [ i ].height;
            options.Add ( option );

            if ( _resolutions [ i ].width == Screen.currentResolution.width && _resolutions [ i ].height == Screen.currentResolution.height )
            {
                resolutionValue = i;
            }
        }
        _resDropdown.AddOptions ( options );
        _resDropdown.value = resolutionValue;
        _resDropdown.RefreshShownValue ( );
    }

    public void SetRes (int resolutionIndex )
    {
        resolutionValue = resolutionIndex;
    }

    public void SaveSettings ( )
    {
        _gameMixer.SetFloat ( "SFX Volume" , sfxVolumeValue );
        PlayerPrefs.SetFloat ( "SFX Volume" , sfxVolumeValue );

        _gameMixer.SetFloat ( "Music Volume, " , musicVolumeValue );
        PlayerPrefs.SetFloat ( "Music Volume" , musicVolumeValue );

        QualitySettings.antiAliasing = antiAliasingValue;
        PlayerPrefs.SetInt ( "Anti Aliasing" , antiAliasingValue );

        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt ( "Fullscreen" , fullscreen ? 1 : 0 );

        Resolution resolution = _resolutions [ resolutionValue ];
        Screen.SetResolution ( resolution.width , resolution.height , Screen.fullScreen );
        PlayerPrefs.SetInt ( "Resolution" , resolutionValue );
    }

    public void SetFullScreen ( bool isFullscreen )
    {
        fullscreen = isFullscreen;
    }

    public void SetAntiAliasing ( int qualityIndex )
    {
        antiAliasingValue = qualityIndex;
    }

    /*
    public void SetAntiAliasingTwo (int qualityIndex )
    {
        QualitySettings.antiAliasing = qualityIndex;

        if ( _2xAA.isOn == true )
        {
            _noAA.isOn = false;
            _4xAA.isOn = false;
            _8xAA.isOn = false;
        }
    }

    public void SetAntiAliasingFour (int qualityIndex )
    {
        QualitySettings.antiAliasing = qualityIndex;

        if ( _4xAA.isOn == true )
        {
            _noAA.isOn = false;
            _2xAA.isOn = false;
            _8xAA.isOn = false;
        }
    }

    public void SetAntiAliasingEight (int qualityIndex )
    {
        QualitySettings.antiAliasing = qualityIndex;

        if ( _8xAA.isOn == true )
        {
            _noAA.isOn = false;
            _2xAA.isOn = false;
            _4xAA.isOn = false;
        }
    }
    */

    public void SFXControl ( float sfxvolume )
    {
        sfxVolumeValue = sfxvolume;
    }

    public void MusicControl ( float musicVolume )
    {
        musicVolumeValue = musicVolume;
    }

   
}
