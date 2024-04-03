using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public void BackMainMenu ( )
    {
        SceneManager.LoadScene ( 0 );
    }

    public void NewGame ( )
    {
        SceneManager.LoadScene ( 1 );
    }

    public void Settings ( )
    {
        SceneManager.LoadScene ( 2 );
    }

    public void HowTo ( )
    {
        SceneManager.LoadScene ( 4 );
    }

    public void Credits ( )
    {
        SceneManager.LoadScene ( 6 );
    }

    public void quit ( )
    {
        Application.Quit ();
        Debug.Log ( "You Have Quit" );
    }

}
