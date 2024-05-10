using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class MainMenu : MonoBehaviour
{
    #region

    [Header("Gameobj Ref")]
    [SerializeField] GameObject _lvlPanel;
    [SerializeField] GameObject _tutorialButton;
    [SerializeField] GameObject _newGameButton;


    bool _controllerConnected = false, _isPlayingGame;

    #endregion

    private void OnApplicationFocus ( bool focus )
    {
        //Checks the computers focus
        _isPlayingGame = focus;
    }

    private void Awake ( )
    {
        //Makes sure the computers focus is the game
        _isPlayingGame = true;

        if(_isPlayingGame == true )
        {
            //Makes sure the correct button is the the first one selected
            EventSystem.current.SetSelectedGameObject ( _newGameButton );

            StartCoroutine ( CheckForControllers ( ) );

            //If a controller is detected hides and locks the cursor
            if ( _controllerConnected == true )
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    private void Update ( )
    {
        if ( _isPlayingGame == true )
        {
            StartCoroutine ( CheckForControllers ( ) );

            //If a controller is detected hides and locks the cursor
            if ( _controllerConnected == true )
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }    

    public void BackMainMenu ( )
    {
        SceneManager.LoadScene ( "Main Menu" );
    }

    public void NewGame ( )
    {
        _lvlPanel.SetActive ( true );
        EventSystem.current.SetSelectedGameObject ( _tutorialButton );
    }

    public void Tutorial ( )
    {
        SceneManager.LoadScene ( "Tutorial" );
    }

    public void LVLOne ( )
    {
        SceneManager.LoadScene ( "Level 1" );
    }

    public void LVLOneHalf ( )
    {
        SceneManager.LoadScene ( "Level 1.5" );
    }

    public void LVLTwo ( )
    {
        SceneManager.LoadScene ( "Level 2" );
    }

    public void LVLTwoB ( )
    {
        SceneManager.LoadScene ( "Level 2B" );
    }

    public void Settings ( )
    {
        SceneManager.LoadScene ( "Settings" );
    }

    public void HowTo ( )
    {
        SceneManager.LoadScene ( "HowToPlay" );
    }

    public void Credits ( )
    {
        SceneManager.LoadScene ( "Credits" );
    }

    public void quit ( )
    {
        Application.Quit ();
        Debug.Log ( "You Have Quit" );
    }

    IEnumerator CheckForControllers ( )
    {
        while ( true )
        {
            var controllers = Input.GetJoystickNames ( );

            if ( !_controllerConnected && controllers.Length > 0 )
            {
                _controllerConnected = true;
            }
            else if ( _controllerConnected && controllers.Length == 0 )
            {
                _controllerConnected = false;
            }
            yield return new WaitForSeconds ( 1f );
        }
    }
}
