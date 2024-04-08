using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Settings : MonoBehaviour
{
    #region Variables

    [SerializeField] 
    Dropdown _resDropdown;
    [SerializeField]
    Button _mainMenuButton;

    Resolution [ ] resolutions;

    #endregion

    private void Start ( )
    {
        resolutions = Screen.resolutions;

        _resDropdown.ClearOptions ( );

        List<string> options = new List<string> ( );

        int currentResolutionIndex = 0;

        for ( int i = 0 ; i < resolutions.Length ; i++ )
        {
            string option = resolutions [ i ].width + "x" + resolutions [ i ].height;
            options.Add ( option );

            if ( resolutions [i].width==Screen.currentResolution.width && resolutions [i].height==Screen.currentResolution.height )
            {
                currentResolutionIndex = i;
            }
        }

        _resDropdown.AddOptions ( options );
        _resDropdown.value = currentResolutionIndex;
        _resDropdown.RefreshShownValue ( );
    }

    private void Update ( )
    {
        
    }

    public void SetRes (int resolutionIndex )
    {
        Resolution resolution= resolutions [ resolutionIndex ];
        Screen.SetResolution ( resolution.width , resolution.height , Screen.fullScreen );
    }

    public void SetFullScreen ( bool isFullscreen )
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetAntiAliasing ( )
    {

    }
}
