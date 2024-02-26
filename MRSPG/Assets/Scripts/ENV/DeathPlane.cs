using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    public float yStart;
    //start a fade to black here
    public float yEnd;
    public GameObject player;

    private void Update()
    {
        if(player.transform.position.y < yStart)
        {

            //disable player input
            //make camera just look at player

        }
        if(player.transform.position.y < yEnd)
        {
            
            //start fade
            
        }
    }

    IEnumerator FadeToBlack()
    {
        //animate a panel here

        //reset player position/spawnpoint
        //re-enable player input.

        yield return null;
    }
}
