using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnorCamHolder : MonoBehaviour
{
    public GameObject player;

    public float maxCameraDist;

    public float backstep;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        RaycastHit hit;
        Physics.Raycast(player.transform.position, -Camera.main.gameObject.transform.forward, out hit, maxCameraDist);
        Debug.DrawRay(player.transform.position, -Camera.main.gameObject.transform.forward, Color.red, Time.deltaTime);
        Debug.Log(hit.point);
        if(hit.point == Vector3.zero)
        {
            transform.position = player.transform.position - Camera.main.gameObject.transform.forward * maxCameraDist;
        }
        else
        {
            transform.position = hit.point + (Camera.main.gameObject.transform.forward * backstep);
        }


        //raycast backwards from the player object

        //set this to the raycast position - some distance idk so it doesnt clip into walls and stuff

        //transform.position =
    }
}
