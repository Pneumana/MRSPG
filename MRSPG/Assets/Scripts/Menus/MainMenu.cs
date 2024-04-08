using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{

    private void Awake ( )
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    public void BackMainMenu ( )
    {
        SceneManager.LoadScene ( "Main Menu" );
    }

    public void NewGame ( )
    {
        SceneManager.LoadScene ("Tutorial");
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

}
