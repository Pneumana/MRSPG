using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region  Variables
    
    [SerializeField]
    public Image pauseMenu;

    [SerializeField]
    public Button continueGame, settings, controls, mainMenu, credits, quit;

    [SerializeField]
    public Button continueSelect, settingsSelect, controlsSelect, mainMenuSelect, creditsSelect, quitSelect;

    public Controller controller;
    public LockOnSystem lockOn;
    public MainMenu mainMenuUI;

    public enum pausedMenu { paused,continuegame, controls, settings, mainmenu, credits, quitgame}
    pausedMenu paused;

    #endregion

    private void Start ( )
    {
        Cursor.visible = false;

        controller = GameObject.Find ( "Controller Detection" ).GetComponent<Controller> ( );

        if ( controller == null )
        {
            Debug.LogError ( "control is NULL" );
        }

        lockOn = GameObject.Find ( "TimeScaler" ).GetComponent<LockOnSystem> ( );

        if( lockOn == null )
        {
            Debug.LogError ( "TimeScaler is NULL" );
        }

        mainMenuUI=GameObject.Find("Canvas").GetComponent<MainMenu> ( );

        if ( mainMenuUI == null )
        {
            Debug.LogError ( "Canvas is NULL" );
        }
    }

    public void Update ()
    {
        if ( controller.controls.Gameplay.Pause.IsPressed ( ) )
        {
            Pause ( );
        }

        if ( controller.controls.Gameplay.MenuSelect.IsPressed ( ) && lockOn.paused == true )
        {
            ButtonPressed ( );
        }

        switch ( paused )
        {
            case pausedMenu.continuegame:
                continueGame.gameObject.SetActive ( false );
                continueSelect.gameObject.SetActive ( true );
                settings.gameObject.SetActive ( true );
                controls.gameObject.SetActive ( true );
                mainMenu.gameObject.SetActive ( true );
                credits.gameObject.SetActive ( true );
                quit.gameObject.SetActive ( true );
                break;
            case pausedMenu.settings:
                settings.gameObject.SetActive ( false );
                settingsSelect.gameObject.SetActive ( true );
                continueGame.gameObject.SetActive ( true );
                controls.gameObject.SetActive(true );
                mainMenu.gameObject.SetActive ( true );
                credits.gameObject.SetActive ( true );
                quit.gameObject.SetActive ( true );
                break;
            case pausedMenu.controls:
                controls.gameObject.SetActive ( false );
                controlsSelect.gameObject.SetActive ( true );
                continueGame.gameObject.SetActive ( true );
                settings.gameObject .SetActive ( true );
                mainMenu.gameObject.SetActive ( true );
                credits.gameObject.SetActive ( true );
                quit.gameObject.SetActive ( true );
                break;
            case pausedMenu.mainmenu:
                mainMenu.gameObject.SetActive ( false );
                mainMenuSelect.gameObject.SetActive ( true );
                continueGame.gameObject.SetActive ( true );
                controls.gameObject.SetActive ( true );
                settings.gameObject.SetActive(true);
                credits.gameObject.SetActive ( true );
                quit.gameObject.SetActive ( true );
                break;
            case pausedMenu.credits:
                credits.gameObject.SetActive ( false );
                creditsSelect.gameObject.SetActive ( true );
                continueGame.gameObject.SetActive ( true );
                settings.gameObject.SetActive ( true );
                controls.gameObject.SetActive ( true );
                mainMenu.gameObject.SetActive ( true );
                quit.gameObject.SetActive ( true );
                break;
            case pausedMenu.quitgame:
                quit.gameObject.SetActive ( false );
                quitSelect.gameObject.SetActive ( true );
                continueGame.gameObject.SetActive ( true );
                settings.gameObject.SetActive ( true );
                controls.gameObject.SetActive ( true );
                mainMenu.gameObject.SetActive ( true );
                credits.gameObject.SetActive ( true );
                break;
            default:
                break;
        }

    }
    
    private void Pause ( )
    {
        lockOn.paused = true;
        pauseMenu.gameObject.SetActive ( true );
        Time.timeScale = 0;
        Cursor.visible = false;
    }

    
    public void MakeSelection ( InputAction.CallbackContext context )
    {

        if ( GameObject.FindWithTag ( "Continue" ) )
        {
            paused = pausedMenu.continuegame;
            Debug.Log ( "Continue Selected" );
        }
        else if ( GameObject.FindWithTag ( "Settings" ) )
        {
            paused = pausedMenu.settings;
            Debug.Log ( "Settings Selected" );
        }
        else if ( GameObject.FindWithTag ( "Controls" ) )
        {
            paused = pausedMenu.controls;
            Debug.Log ( "Controls Selected" );
        }        
        else if( GameObject.FindWithTag ( "MainMenu" ) )
        {
            paused = pausedMenu.mainmenu;
            Debug.Log ( "Main Menu Selected" );
        }        
        else if( GameObject.FindWithTag ( "Credits" ) )
        {
            paused = pausedMenu.credits;
            Debug.Log ( "Credits Selected" );
        }
        else if ( GameObject.FindWithTag ( "Quit" ) )
        {
            paused = pausedMenu.quitgame;
            Debug.Log ( "Quit Selected" );
        }

    }   

    void ButtonPressed ( )
    {
       if( GameObject.FindWithTag ( "Continue" ) )
        {
            ContinueGame ( );
        }       
        else if( GameObject.FindWithTag ( "Settings" ) )
        {
            SettingsMenu ( );
        }       
        else if ( GameObject.FindWithTag ( "Controls" ) )
        {
            ControlsMenu ( );
        }        
        else if (  GameObject.FindWithTag ( "MainMenu" ) )
        {
            MainMenu ( );
        }       
        else if( GameObject.FindWithTag ( "Credits" ) )
        {
            CreditsMenu ( );
        }       
        else if(GameObject.FindWithTag ( "Quit" ) )
        {
            QuitGame ( );
        }
    }    

    public void ContinueGame ( )
    {
        pauseMenu.gameObject.SetActive( false );
        Time.timeScale = 1;
    }

    public void SettingsMenu ( )
    {
       mainMenuUI.Settings ( );
    }

    public void ControlsMenu ( )
    {
        mainMenuUI.HowTo ( );
    }

    public void MainMenu ( )
    {
        mainMenuUI.BackMainMenu ( );
    }

    public void CreditsMenu ( )
    {
        mainMenuUI.Credits ( );
    }

    public void QuitGame ( )
    {
        mainMenuUI.quit ( );
        Debug.Log ( "You have Quit" );
    }

}
