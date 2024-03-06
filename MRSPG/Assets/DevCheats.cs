using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevCheats : MonoBehaviour
{
    GameObject playerObj;
    public Vector3 grayboxPos;
    public Vector3 devtestPos;

    private void Start()
    {
        playerObj = GameObject.Find("PlayerObj");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.BackQuote))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                playerObj.GetComponent<CharacterController>().enabled = false;
                playerObj.transform.position = grayboxPos;
                Debug.Log("warping to graybox");
                playerObj.GetComponent<CharacterController>().enabled = true;
                //warp to the start of the grayboxed level
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                playerObj.GetComponent<CharacterController>().enabled = false;
                playerObj.transform.position = devtestPos;
                Debug.Log("warping to devtest");
                playerObj.GetComponent<CharacterController>().enabled = true;
                //warp to devtest area
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                //refresh the current scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                Debug.Log("reloading scene");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawLine(grayboxPos + Vector3.left, grayboxPos + Vector3.right, Color.blue);
        Debug.DrawLine(grayboxPos + Vector3.up, grayboxPos + Vector3.down, Color.blue);
        Debug.DrawLine(grayboxPos + Vector3.forward, grayboxPos + Vector3.back, Color.blue);

        Debug.DrawLine(devtestPos + Vector3.left, devtestPos + Vector3.right, Color.blue);
        Debug.DrawLine(devtestPos + Vector3.up, devtestPos + Vector3.down, Color.blue);
        Debug.DrawLine(devtestPos + Vector3.forward, devtestPos + Vector3.back, Color.blue);
    }
}
