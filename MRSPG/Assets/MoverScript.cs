using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoverScript : MonoBehaviour
{
    public Vector3 pos1;
    public Vector3 pos2;
    public float speed;
    bool reverse;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!reverse)
        {
            transform.position = Vector3.MoveTowards(transform.position, pos1, Time.deltaTime * speed);
            if(transform.position == pos1)
            {
                reverse = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, pos2, Time.deltaTime * speed);
            if (transform.position == pos2)
            {
                reverse = false;
            }
        }


    }
    private void OnDrawGizmosSelected()
    {
        Debug.DrawLine(pos1 + Vector3.left, pos1 - Vector3.left, Color.green, Time.deltaTime);
        Debug.DrawLine(pos1 + Vector3.up, pos1 - Vector3.up, Color.green, Time.deltaTime);
        Debug.DrawLine(pos1 + Vector3.forward, pos1 - Vector3.forward, Color.green, Time.deltaTime);

        Debug.DrawLine(pos2 + Vector3.left, pos2 - Vector3.left, Color.red, Time.deltaTime);
        Debug.DrawLine(pos2 + Vector3.up, pos2 - Vector3.up, Color.red, Time.deltaTime);
        Debug.DrawLine(pos2 + Vector3.forward, pos2 - Vector3.forward, Color.red, Time.deltaTime);
    }
}
