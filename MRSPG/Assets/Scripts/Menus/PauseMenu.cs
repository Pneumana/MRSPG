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
    GameObject _pauseContinue, _settingsContinue, _howToContinue, _creditsContinue;

    bool _settingsOpen, _howToOpen, _creditsOpen;

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
        if ( _settingsOpen == false && _howToOpen == false && _creditsOpen == false )
        {
            lockOn.paused = true;
            Cursor.visible = false;
            _pauseMenu.gameObject.SetActive ( true );
            EventSystem.current.SetSelectedGameObject ( _pauseContinue );
            _settingsMenu.gameObject.SetActive ( false );
            _howToMenu.gameObject.SetActive ( false );
            _creditsMenu.gameObject.SetActive ( false );
            Time.timeScale = 0f;
        }
        else
        {
            lockOn.paused = true;
            Cursor.visible = false;
            _pauseMenu.gameObject.SetActive ( true );
            EventSystem.current.SetSelectedGameObject ( _pauseContinue );
            _settingsMenu.gameObject.SetActive ( false );
            _howToMenu.gameObject.SetActive ( false );
            _creditsMenu.gameObject.SetActive ( false );
            _settingsOpen = false;
            _howToOpen = false;
            _creditsOpen = false;
            Time.timeScale = 0f;
        }
    }

    public void Resume ( )
    {
        lockOn.paused= false;
        _pauseMenu.gameObject .SetActive ( false );
        _settingsMenu.gameObject.SetActive ( false );
        _howToMenu.gameObject.SetActive ( false );
        _creditsMenu.gameObject.SetActive ( false );
        _settingsOpen = false;
        _howToOpen = false;
        _creditsOpen = false;
        Time.timeScale = 1f;
    }

    public void Settings ( )
    {        
        lockOn.paused= true;
        _settingsMenu.gameObject.SetActive ( true );
        _settingsOpen = true;
        _howToOpen= false;
        _creditsOpen=false;
        EventSystem.current.SetSelectedGameObject(_settingsContinue);
        _pauseMenu.gameObject.SetActive( false );
        _howToMenu.gameObject.SetActive ( false );
        _creditsMenu.gameObject.SetActive( false );
        Time.timeScale = 0f;
    }

    public void HowTo ( )
    {
        lockOn.paused=true;
        _howToMenu.gameObject .SetActive ( true );
        _howToOpen=true;
        _settingsOpen = false;
        _creditsOpen = false;
        EventSystem.current.SetSelectedGameObject ( _howToContinue );
        _pauseMenu.gameObject.SetActive(false);
        _settingsMenu .gameObject.SetActive( false );
        _creditsMenu .gameObject.SetActive( false );
        Time.timeScale = 0f;
    }

    public void Credits ( )
    {
        lockOn.paused = true;
        _creditsMenu.gameObject .SetActive ( true );
        _creditsOpen = true;
        _settingsOpen = false;
        _howToOpen = false;
        EventSystem.current.SetSelectedGameObject ( _creditsContinue );
        _pauseMenu .gameObject.SetActive( false );
        _settingsMenu.gameObject .SetActive( false );
        _howToMenu .gameObject.SetActive( false );
        Time.timeScale = 0f;
    }

    public void BackToPause ( )
    {
        Paused ( );
    }

}
