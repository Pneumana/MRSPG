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

    #endregion

    private void Start ( )
    {

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
    }

    public void Update ()
    {
        if ( controller.controls.Gameplay.Pause.IsPressed ( ) )
        {
            Pause ( );
        }

        if ( controller.controls.Gameplay.NavMenuup.IsPressed ( ) || controller.controls.Gameplay.NavMenuDown.IsPressed ( ) && lockOn.paused == true )
        {
            MakeSelection ( );
        }

        if ( controller.controls.Gameplay.MenuSelect.IsPressed ( ) && lockOn.paused == true )
        {
            ButtonPressed ( );
        }
    }

    private void Pause ( )
    {
        lockOn.paused = true;
        pauseMenu.gameObject.SetActive ( true );
        Time.timeScale = 0;
        Cursor.visible = false;
    }

    void MakeSelection ( )
    {
        if ( GameObject.Find ( "Continue" ) )
        {
            continueGame.gameObject.SetActive ( false );
            continueSelect.gameObject.SetActive ( true );
        }
        else if ( GameObject.Find ( "Settings" ) )
        {
            settings.gameObject.SetActive ( false );
            settingsSelect.gameObject.SetActive ( true );
        }        
        else if( GameObject.Find ( "Controls" ) )
        {
            controls.gameObject.SetActive ( false );
            controlsSelect.gameObject.SetActive ( true );
        }
        else if( GameObject.Find ( "MainMenu" ) )
        {
            mainMenu.gameObject.SetActive ( false );
            mainMenuSelect.gameObject.SetActive ( true );
        }
        else if( GameObject.Find ( "Credits" ) )
        {
            credits.gameObject.SetActive ( false );
            creditsSelect.gameObject.SetActive ( true );
        }
        else if ( GameObject.Find ( "Quit" ) )
        {
            quit.gameObject.SetActive ( false );
            quitSelect.gameObject.SetActive ( true );
        }

    }

    void ButtonPressed ( )
    {
       if( GameObject.Find ( "Continue" ) )
        {
            ContinueGame ( );
        }       
        else if( GameObject.Find ( "Settings" ) )
        {
            SettingsMenu ( );
        }       
        else if ( GameObject.Find ( "Controls" ) )
        {
            ControlsMenu ( );
        }        
        else if (  GameObject.Find ( "MainMenu" ) )
        {
            MainMenu ( );
        }       
        else if( GameObject.Find ( "Credits" ) )
        {
            CreditsMenu ( );
        }       
        else if(GameObject.Find ( "Quit" ) )
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
        SceneManager.LoadScene ( 2 );
    }

    public void ControlsMenu ( )
    {
        SceneManager .LoadScene ( 3 );
    }

    public void MainMenu ( )
    {
        SceneManager.LoadScene ( 0 );
    }

    public void CreditsMenu ( )
    {
        SceneManager.LoadScene( 4 );
    }

    public void QuitGame ( )
    {
        Application.Quit ();
        Debug.Log ( "You have Quit" );
    }

}
