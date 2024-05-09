using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    #region Variables

    [Header ( "UI Panels" )]
    [SerializeField] Image _pauseMenu;
    [SerializeField] Image _settingsMenu;
    [SerializeField] Image _howToMenu;
    [SerializeField] Image _creditsMenu;

    [Space ( 10 )]
    [Header ( "Continue Buttons" )]
    [SerializeField] GameObject _pauseContinue;
    [SerializeField] GameObject _settingsContinue;
    [SerializeField] GameObject _howToContinue;
    [SerializeField] GameObject _creditsContinue;

    bool _settingsOpen, _howToOpen, _creditsOpen, _isPaused;

    [Space(10)]
    [Header("Needed Objects")]
    public LockOnSystem lockOn;
    public Controller control;
    public AudioSource lvlMusic, pausedMusic;

    #endregion

    private void Awake ( )
    {        
        lvlMusic.Play ( );
        pausedMusic.Stop ( );
        Resume();
    }

    private void Update ( )
    {
        if ( control.controls.Gameplay.PauseGame.IsPressed ( ) )
        {
            _isPaused = true;
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
            SwitchMusic ( );
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
            SwitchMusic ( );
            Time.timeScale = 0f;
        }
    }

    void SwitchMusic ( )
    {

        if ( _isPaused == true )
        {
            lvlMusic.Stop ( );
            pausedMusic.Play ( );
        }
        else if ( _isPaused == false )
        {
            pausedMusic.Stop ( );
            lvlMusic.Play ( );
        }
    }

    public void Resume ( )
    {
        lockOn.paused= false;
        _isPaused = false;
        _pauseMenu.gameObject .SetActive ( false );
        _settingsMenu.gameObject.SetActive ( false );
        _howToMenu.gameObject.SetActive ( false );
        _creditsMenu.gameObject.SetActive ( false );
        _settingsOpen = false;
        _howToOpen = false;
        _creditsOpen = false;
        SwitchMusic ( );
        Time.timeScale = 1f;
    }

    public void Settings ( )
    {        
        lockOn.paused= true;
        _isPaused = true;
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
        _isPaused = true;
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
        _isPaused = true;
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
