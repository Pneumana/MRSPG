using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    #region variables

    [SerializeField]
    Button _newGame, _settings, _howTo, _credits, _quit;

    #endregion


    public void NewGame ( )
    {
        SceneManager.LoadScene ( 1 );
    }

    public void Settings ( )
    {
        SceneManager .LoadScene ( 2 );
    }

    public void HowTo ( )
    {
        SceneManager .LoadScene ( 3 );
    }

    public void Credits ( )
    {
        SceneManager .LoadScene ( 4 );
    }

    public void quit ( )
    {
        Application.Quit ();
        Debug.Log ( "You Have Quit" );
    }
}
