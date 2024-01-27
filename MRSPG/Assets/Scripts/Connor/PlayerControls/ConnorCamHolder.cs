using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConnorCamHolder : MonoBehaviour
{
    public GameObject player;

    public float maxCameraDist;

    public float backstep;

    public float mouseSense;
    public float controllerSense;

    public Vector3 offset;

    public float lookX;
    float lookY;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Update()
    {
        var mouseInputX = Input.GetAxis("Mouse X");
        float ctrlInputX = 0;

        var mouseInputY = Input.GetAxis("Mouse Y");
        float ctrlInputY = 0;

        if (Gamepad.current != null)
        {
            ctrlInputX = Gamepad.current.rightStick.x.value;
            ctrlInputY = Gamepad.current.rightStick.y.value;
        }

        var decidedXDelta = 0f;
        var decidedYDelta = 0f;
        //get if the player is using the controller or the mouse
        if (Mathf.Abs(mouseInputX) >= Mathf.Abs(ctrlInputX))
            decidedXDelta = mouseInputX * mouseSense;
        if (Mathf.Abs(ctrlInputX) >= Mathf.Abs(mouseInputX))
            decidedXDelta = ctrlInputX * controllerSense;
        if (Mathf.Abs(mouseInputY) >= Mathf.Abs(ctrlInputY))
            decidedYDelta = mouseInputY * mouseSense;
        if (Mathf.Abs(ctrlInputY) >= Mathf.Abs(mouseInputY))
            decidedYDelta = ctrlInputY * controllerSense;

        lookX += decidedXDelta;
        lookY -= decidedYDelta;

        lookY = Mathf.Clamp(lookY, -90f, 90f);

        Camera.main.gameObject.transform.rotation = Quaternion.Euler(new Vector3(lookY, lookX, 0));
    }
    // Update is called once per frame
    void LateUpdate()
    {
        RaycastHit hit;
        var pos = player.transform.position + offset;
        Physics.Raycast(pos, -Camera.main.gameObject.transform.forward, out hit, maxCameraDist);
        Debug.DrawRay(pos, -Camera.main.gameObject.transform.forward, Color.red, Time.deltaTime);

        if(hit.point == Vector3.zero)
        {
            transform.position = pos - Camera.main.gameObject.transform.forward * maxCameraDist;
        }
        else
        {
            transform.position = hit.point + (Camera.main.gameObject.transform.forward * backstep) + (Vector3.up * 0.01f);
        }


        //raycast backwards from the player object

        //set this to the raycast position - some distance idk so it doesnt clip into walls and stuff

        //transform.position =
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.DrawWireSphere(player.transform.position + offset, 0.1f);
        }
    }
}
