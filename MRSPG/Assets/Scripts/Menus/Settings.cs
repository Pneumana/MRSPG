using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System;
using Cinemachine;


public class Settings : MonoBehaviour
{
    #region Variables

    [Header ( "Values" )]
    public float sfxVolumeValue;
    public float musicVolumeValue;
    public float camSenseXDecelValue;
    public float camSenseYDecelValue;
    public float camSenceXAccelValue;
    public float camSenseYAccelValue;

    public int antiAliasingValue;
    public int resolutionValue;

    public bool fullscreen;

    [Space ( 10 )]
    [Header ( "Ref" )]
    [SerializeField] AudioMixer _gameMixer;
    [SerializeField] CinemachineFreeLook _playerCam;
    [SerializeField] TMP_Dropdown _resDropdown;
    [SerializeField] TMP_Dropdown _antiAliasing;
    [SerializeField] TMP_Text _sfxValue;
    [SerializeField] TMP_Text _musicValue;
    [SerializeField] TMP_Text _camXDecelValue;
    [SerializeField] TMP_Text _camYDecelValue;
    [SerializeField] TMP_Text _camXAccelValue;
    [SerializeField] TMP_Text _camYAccelValue;
    [SerializeField] Slider _sfxSlider;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _camSenseXDecel;
    [SerializeField] Slider _camSenseYDecel;
    [SerializeField] Slider _camSenseXAccel;
    [SerializeField] Slider _camSenseYAccel;
    [SerializeField] Toggle _fullscreenToggle;

    [SerializeField] List<ChangeSettingData> connorSettings = new List<ChangeSettingData>();

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
        _fullscreenToggle.isOn = PlayerPrefs.GetInt ( "Fullscreen" ) != 0;
        GetResolutions();
        _resDropdown.value = PlayerPrefs.GetInt ( "Resolution" );

        _camSenseXDecel.value = PlayerPrefs.GetFloat ( "Camera X Sense" );
        _camSenseXAccel.value = PlayerPrefs.GetFloat ( "Camera X Accel" );
        _camSenseYDecel.value = PlayerPrefs.GetFloat ( "Camera Y Sense" );
        _camSenseYAccel.value = PlayerPrefs.GetFloat ( "Camera Y Accel" );
    }

    private void Update ( )
    {
        SoundValueUpdate ( );
        CameraValueUpdate ( );
    }

    public void SaveSettings ( )
    {
        _gameMixer.SetFloat ( "SFX Volume" , sfxVolumeValue );
        PlayerPrefs.SetFloat ( "SFX Volume" , sfxVolumeValue );

        _gameMixer.SetFloat ( "Music" , musicVolumeValue );
        PlayerPrefs.SetFloat ( "Music Volume" , musicVolumeValue );

        QualitySettings.antiAliasing = antiAliasingValue;
        PlayerPrefs.SetInt ( "Anti Aliasing" , antiAliasingValue );

        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt ( "Fullscreen" , fullscreen ? 1 : 0 );

        Resolution resolution = _resolutions [ resolutionValue ];
        Screen.SetResolution ( resolution.width , resolution.height , Screen.fullScreen );
        PlayerPrefs.SetInt ( "Resolution" , resolutionValue );

        _playerCam.m_YAxis.m_DecelTime = camSenseYDecelValue;
        _playerCam.m_YAxis.m_AccelTime = camSenceXAccelValue;
        PlayerPrefs.SetFloat ( "Camera Y Sense" , camSenseYDecelValue );
        PlayerPrefs.SetFloat ( "Camera Y Accel" , camSenseYAccelValue );

        _playerCam.m_XAxis.m_DecelTime = camSenseXDecelValue;
        _playerCam.m_XAxis.m_AccelTime += camSenceXAccelValue;
        PlayerPrefs.SetFloat ( "Camera X Sense" , camSenseXDecelValue );
        PlayerPrefs.SetFloat ( "Camera X Accel" , camSenceXAccelValue );

        foreach(ChangeSettingData csd in connorSettings)
        {
            csd.ChangeSetting();
        }
        if (PulseTextures.Instance!=null)
        {
            PulseTextures.Instance.Refresh();
        }
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

    public void SoundValueUpdate ( )
    {
        _sfxValue.text = sfxVolumeValue.ToString ( );
        _musicValue.text = musicVolumeValue.ToString ( );
    }

    public void CameraSensitivityXDecel ( float camXSense)
    {
        camSenseXDecelValue = camXSense;
    }

    public void CameraSensitivityXAccel (float camXSense )
    {
        camSenceXAccelValue = camXSense;
    }

    public void CameraSensitivityYDecel ( float camYSense )
    {
        camSenseYDecelValue = camYSense;        
    }

    public void CameraSensitivityYAccel(float camYSense ) 
    {
        camSenseYAccelValue = camYSense;
    }

    public void CameraValueUpdate ( )
    {
        _camXDecelValue.text = camSenseXDecelValue.ToString ( );
        _camXAccelValue.text = camSenceXAccelValue.ToString ( );
        _camYDecelValue.text = camSenseYDecelValue.ToString ( );
        _camYAccelValue.text = camSenseYAccelValue.ToString ( );
    }

   
}
