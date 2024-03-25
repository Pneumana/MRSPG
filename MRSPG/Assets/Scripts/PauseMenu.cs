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
    Image _pauseMenu;

    [SerializeField]
    Button _contineFirst;

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
        lockOn.paused = true;
        Cursor.visible = false;
        _pauseMenu.gameObject.SetActive ( true );
        Time.timeScale = 0f;
    }

    public void Resume ( )
    {
        lockOn.paused= false;
        _pauseMenu.gameObject .SetActive ( false );
        Time.timeScale = 1f;
    }

}
