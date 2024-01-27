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
        Vector3 rots = transform.rotation.eulerAngles;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rots.x -= speed * Time.unscaledDeltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rots.x += speed * Time.unscaledDeltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rots.y -= speed * Time.unscaledDeltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rots.y += speed * Time.unscaledDeltaTime;
        }
        transform.rotation = Quaternion.Euler(rots);
    }
}
