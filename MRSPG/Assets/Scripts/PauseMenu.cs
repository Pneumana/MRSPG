using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    #region Variables
    [SerializeField]
    Image _pauseMenu, _settingsMenu, _howToMenu, _creditsMenu;

    [SerializeField]
    GameObject _continueFirst, _continueSecond, _continueThird, _continueFourth;

    public LockOnSystem lockOn;
    public Controller control;

    #endregion


    private void Update ( )
    {
        if ( control.controls.Gameplay.PauseGame.IsPressed ( ) )
        {
            Paused ( );
        }
    }

    public void Paused ( )
    {
        lockOn.paused = true;
        Cursor.visible = false;
        _pauseMenu.gameObject.SetActive ( true );
        EventSystem.current.SetSelectedGameObject ( _continueFirst );
        Time.timeScale = 0f;
    }

    public void Resume ( )
    {
        lockOn.paused= false;
        _pauseMenu.gameObject .SetActive ( false );
        _settingsMenu.gameObject.SetActive ( false );
        _howToMenu.gameObject.SetActive ( false );
        _creditsMenu.gameObject.SetActive ( false );
        Time.timeScale = 1f;
    }

    public void Settings ( )
    {
        lockOn.paused= true;
        _settingsMenu.gameObject.SetActive ( true );
        EventSystem.current.SetSelectedGameObject(_continueSecond);
        _pauseMenu.gameObject.SetActive( false );
        _howToMenu.gameObject.SetActive ( false );
        _creditsMenu.gameObject.SetActive( false );
    }

    public void HowTo ( )
    {
        lockOn.paused=true;
        _howToMenu.gameObject .SetActive ( true );
        EventSystem.current.SetSelectedGameObject ( _continueThird );
        _pauseMenu.gameObject.SetActive(false);
        _settingsMenu .gameObject.SetActive( false );
        _creditsMenu .gameObject.SetActive( false );
    }

    public void Credits ( )
    {
        lockOn.paused = true;
        _creditsMenu.gameObject .SetActive ( true );
        EventSystem.current.SetSelectedGameObject ( _continueFourth );
        _pauseMenu .gameObject.SetActive( false );
        _settingsMenu.gameObject .SetActive( false );
        _howToMenu .gameObject.SetActive( false );
    }

}
