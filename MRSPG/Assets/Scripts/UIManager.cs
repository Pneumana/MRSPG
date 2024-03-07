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

    public Controller control;

    #endregion

    private void Start ( )
    {
        
        control=GameObject.Find("Controller Detection").GetComponent<Controller>();

        if ( control == null )
        {
            Debug.LogError ( "control is NULL" );
        }
    }

    public void Update ()
    {
        if ( control.controls.Gameplay.Pause.IsPressed ( ) )
        {
            Pause ( );
        }

        if ( control.controls.Gameplay.NavMenu.IsPressed ( ) )
        {
            MakeSelection ( );
        }

        if ( control.controls.Gameplay.MenuSelect.IsPressed ( ) )
        {
            ButtonPressed ( );
        }
    }

    private void Pause ( )
    {
        pauseMenu.gameObject.SetActive ( true );
        Time.timeScale = 0;
        Cursor.visible = false;
    }

    void MakeSelection ( )
    {
       
    }

    void ButtonPressed ( )
    {
       
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
