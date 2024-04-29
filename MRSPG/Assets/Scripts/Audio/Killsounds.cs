using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killsounds : MonoBehaviour
{
    #region

    AudioSource sounds;

    #endregion

    private void Start ( )
    {
        sounds = GetComponent<AudioSource>();
    }

    private void Update ( )
    {
        if ( !sounds.isPlaying )
        {
            Destroy ( gameObject );
        }
    }
}
