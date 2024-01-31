using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static UnityEngine.InputSystem.InputAction;

public class Controller : MonoBehaviour
{
    public string MouseX;
    public string MouseY;

    public string Fire1;
    public string Fire2;
    public string Fire3;

    public InputAction fireAction;


    // Start is called before the first frame update
    public static Controller inst;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        fireAction.performed += Fire;
        InputSystem.onDeviceChange +=
    (device, change) =>
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                Debug.Log("Device added: " + device);
                ChangeInputHandler();
                break;
            case InputDeviceChange.Removed:
                Debug.Log("Device removed: " + device);
                ChangeInputHandler();
                break;
            case InputDeviceChange.ConfigurationChanged:
                Debug.Log("Device configuration changed: " + device);
                break;
        }
    };
        /*InputSystem.onActionChange +=
    (obj, change) =>
    {
        //CallbackContext.Control.device.deviceId
        Debug.Log($"{((InputAction)obj).name} {change}");
    };*/
    }

    void Start()
    {
        ChangeInputHandler();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(MouseX + ", " + MouseY + " : " + Input.GetAxis(MouseX) + ", " + Input.GetAxis(MouseY));
/*        if (Gamepad.current != null)
        {
            var buttonPressed = Gamepad.current.allControls.Any(x => x is ButtonControl button && x.IsPressed() && !x.synthetic);
            if (buttonPressed==false)
            {
                Debug.Log();
            }
        }*/

    }
    void Fire(CallbackContext ctx)
    {
        Debug.Log(ctx.control.device.deviceId);
    }
    /*IEnumerator CheckForGamepad()
    {
        if(Gamepad.current == null)
        {
            ChangeInputHandler();
        }
    }*/

    void ChangeInputHandler()
    {

        //Debug.Log(Gamepad.current.displayName);
        if(Gamepad.current == null)
        {
            Debug.Log("back to keyboard");
            MouseX = "Mouse X";
            MouseY = "Mouse Y";
            return;
        }
        switch(Gamepad.current.displayName)
        {
            case "Xbox Controller":
                //go back to keyboard
                Debug.Log(Gamepad.current.displayName);
                MouseX = "XBMouse X";
                MouseY = "XBMouse Y";
                break;
            case "DualSense Wireless Controller":
                Debug.Log(Gamepad.current.displayName);
                MouseX = "PSMouse X";
                MouseY = "PSMouse Y";
                break;
            default:
                
                break;
        }
    }
}
