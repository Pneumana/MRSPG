using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraTarget : MonoBehaviour
{
    LayerMask ground;
    public Transform playerObj;
    InputControls ic;

    RaycastHit down;
    Vector3 RaisedCamHeight;
    public float CamSpeed;
    public bool groundcheck;
    float raise = 2f;
    float ledgeRaise = 2.5f;

    private void Start()
    {
        ground = LayerMask.GetMask("Ground");
        Vector3 gp = new Vector3(transform.position.x, transform.position.y - raise, transform.position.z);
        ic = GameObject.FindAnyObjectByType<InputControls>();
    }
    private void Update()
    {
        groundcheck = ic.isGrounded;
        transform.position = new Vector3(playerObj.position.x, transform.position.y, playerObj.position.z);

        RaisedCamHeight = transform.position;
        if (Physics.Raycast(playerObj.position, playerObj.TransformDirection(Vector3.down), out down, raise, ground))
        {
            Debug.DrawRay(playerObj.position, Vector3.down, Color.green);
            float targetHeight;

            targetHeight = down.point.y + raise;
            RaisedCamHeight.y = Mathf.Lerp(transform.position.y, targetHeight, CamSpeed * Time.deltaTime);
            transform.position = RaisedCamHeight;

            if(playerObj.gameObject.GetComponent<Animator>().GetBool("LedgeGrabbed"))
            {
                targetHeight = ic.groundCheck.position.y + ledgeRaise;
                RaisedCamHeight.y = Mathf.Lerp(transform.position.y, targetHeight, CamSpeed * Time.deltaTime);
                transform.position = RaisedCamHeight;
            }
        }
    }

}
