using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Settings : MonoBehaviour
{
    #region Variables

    [SerializeField] Dropdown _resDropdown;
    [SerializeField] Toggle _noAA;
    [SerializeField] Toggle _2xAA;
    [SerializeField] Toggle _4xAA;
    [SerializeField] Toggle _8xAA;
    [SerializeField] Toggle _fullscreenToggle;

    Resolution [ ] _resolutions;

    #endregion

    private void Start ( )
    {
        GetResolutions ( );

        _fullscreenToggle.isOn = Screen.fullScreen;
    }

    void GetResolutions ( )
    {
        _resolutions = Screen.resolutions;

        _resDropdown.ClearOptions ( );

        List<string> options = new List<string> ( );

        int currentResolutionIndex = 0;

        for ( int i = 0 ; i < _resolutions.Length ; i++ )
        {
            string option = _resolutions [ i ].width + "x" + _resolutions [ i ].height;
            options.Add ( option );

            if ( _resolutions [ i ].width == Screen.currentResolution.width && _resolutions [ i ].height == Screen.currentResolution.height )
            {
                currentResolutionIndex = i;
            }
        }
        _resDropdown.AddOptions ( options );
        _resDropdown.value = currentResolutionIndex;
        _resDropdown.RefreshShownValue ( );
    }

    public void SetRes (int resolutionIndex )
    {
        Resolution resolution= _resolutions [ resolutionIndex ];
        Screen.SetResolution ( resolution.width , resolution.height , Screen.fullScreen );
    }

    public void SetFullScreen (bool isFullscreen )
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetAntiAliasingNone ( )
    {
        QualitySettings.antiAliasing = 1;

        if ( _noAA.isOn == true )
        {
            _2xAA.isOn = false;
            _4xAA.isOn = false;
            _8xAA.isOn = false;
        }
    }

    public void SetAntiAliasingTwo ( )
    {
        QualitySettings.antiAliasing = 2;

        if ( _2xAA.isOn == true )
        {
            _noAA.isOn = false;
            _4xAA.isOn = false;
            _8xAA.isOn = false;
        }
    }

    public void SetAntiAliasingFour ( )
    {
        QualitySettings.antiAliasing = 4;

        if ( _4xAA.isOn == true )
        {
            _noAA.isOn = false;
            _2xAA.isOn = false;
            _8xAA.isOn = false;
        }
    }

    public void SetAntiAliasingEight ( )
    {
        QualitySettings.antiAliasing = 8;

        if ( _8xAA.isOn == true )
        {
            _noAA.isOn = false;
            _2xAA.isOn = false;
            _4xAA.isOn = false;
        }
    }
}
