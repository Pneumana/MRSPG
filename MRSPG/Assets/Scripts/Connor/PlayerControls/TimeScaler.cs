using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeScaler : MonoBehaviour
{
    public float minTimeScale;
    public float maxTimeScale = 1;
    public float scaleSpeed;

    float remainingTime = 2;
    float cooldown = 0;
    public float useTime;
    public float cooldownTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(cooldown > 0)
            cooldown -= Time.unscaledDeltaTime;
        //InputEventStayLockOn();
        //InputActionEndLockOn();

    }

    void InputEventStayLockOn()
    {
        if (Gamepad.current == null)
        {
            if (Input.GetKey(KeyCode.Space) && remainingTime > 0 && cooldown <= 0)
            {
                remainingTime -= Time.unscaledDeltaTime;
                if (remainingTime <= 0)
                {
                    cooldown = cooldownTime;
                }
                //slow down time
                Time.timeScale = Mathf.Lerp(Time.timeScale, minTimeScale, Time.unscaledDeltaTime * scaleSpeed);
            }
            else
            {
                if (remainingTime != useTime)
                    remainingTime = useTime;
                //speed up time
                Time.timeScale = Mathf.Lerp(Time.timeScale, maxTimeScale, Time.unscaledDeltaTime * scaleSpeed);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Space) && remainingTime > 0 && cooldown <= 0 || Gamepad.current.rightTrigger.wasPressedThisFrame && remainingTime > 0 && cooldown <= 0)
            {
                remainingTime -= Time.unscaledDeltaTime;
                if (remainingTime <= 0)
                {
                    cooldown = cooldownTime;
                }
                //slow down time
                Time.timeScale = Mathf.Lerp(Time.timeScale, minTimeScale, Time.unscaledDeltaTime * scaleSpeed);
            }
            else
            {
                if (remainingTime != useTime)
                    remainingTime = useTime;
                //speed up time
                Time.timeScale = Mathf.Lerp(Time.timeScale, maxTimeScale, Time.unscaledDeltaTime * scaleSpeed);
            }
        }
        Debug.Log(Time.timeScale);
    }
    void InputActionEndLockOn()
    {
        if (Gamepad.current == null)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                cooldown = cooldownTime;
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Space) || Gamepad.current.rightTrigger.wasReleasedThisFrame)
            {
                cooldown = cooldownTime;
            }
        }
    }
}
