using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InputControls : MonoBehaviour
{
    #region Variables
    public CharacterController controller;
    public float speed;
    [SerializeField] private float smoothAngle = 1f;
    private float smoothedVelocity;

    public Transform cam;
    #endregion

    private void Update()
    {
        //Lock the mouse
        if(Input.GetKey(KeyCode.Space))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }else if (Input.GetKey(KeyCode.X))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        
        //Get user input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 movePlayer = new Vector3(horizontal, 0f, vertical).normalized;
        if(movePlayer.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movePlayer.x, movePlayer.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothedVelocity, smoothAngle);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * Time.deltaTime * speed);
        }

    }
}
