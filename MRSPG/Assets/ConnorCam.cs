using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnorCam : MonoBehaviour
{
    public Vector3 lookPos;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 inputDir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            inputDir.z = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputDir.z = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputDir.x = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputDir.x = 1;
        }
        if (inputDir.x != 0 && inputDir.z != 0)
            inputDir *= 0.7f;
        transform.position += (inputDir * speed) * Time.unscaledDeltaTime;
        transform.LookAt(lookPos);
    }
}
