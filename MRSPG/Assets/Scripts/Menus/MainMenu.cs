using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class MainMenu : MonoBehaviour
{
    #region Variables

    [Header("Gameobj Ref")]
    [SerializeField] GameObject _lvlPanel;
    [SerializeField] GameObject _tutorialButton;
    [SerializeField] GameObject _newgameButton;

    [SerializeField] GameObject[] disableOtherUI;

    bool _controllerConnected = false, _isPlaying=true;

    #endregion

    private void OnApplicationFocus ( bool focus )
    {
        _isPlaying = focus;
    }

    private void Awake ( )
    {
        EventSystem.current.SetSelectedGameObject ( _newgameButton );

        if (_isPlaying==true )
        {
            Cursor.visible = false;
        }

        StartCoroutine ( CheckForControllers ( ) );

        //If a controller is detected hides and locks the cursor
        if ( _controllerConnected == true )
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if ( _controllerConnected == false )
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void Update ( )
    {
        StartCoroutine ( CheckForControllers ( ) );

        if (_isPlaying==true)
        {
            Cursor.visible = false;
        }

        //If a controller is detected hides and locks the cursor
        if ( _controllerConnected == true )
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if ( _controllerConnected == false )
        {
            Cursor.visible= false;
            Cursor.lockState = CursorLockMode.None;
        }
    }    

    public void BackMainMenu ( )
    {
        SceneManager.LoadScene ( "Main Menu" );
    }

    public void NewGame ( )
    {
        _lvlPanel.SetActive ( true );

        if ( _controllerConnected == true )
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if ( _controllerConnected == false )
        {
            Cursor.lockState = CursorLockMode.None;
        }

        foreach (GameObject obj in disableOtherUI)
        {
            obj.SetActive(false);
        }

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
