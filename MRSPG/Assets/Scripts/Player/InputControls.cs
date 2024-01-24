using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InputControls : MonoBehaviour
{
    #region Variables
    public CharacterController controller;
    public float speed;
    private float smoothAngle = 0.1f;
    private float smoothedVelocity;
    private Vector3 velocity;
    private Vector3 moveDirection;
    public float jump = 10f;

    public Transform cam;
    #endregion

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.X))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        //Get user input
        /////fix the jump!!!!
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        if (!controller.isGrounded) // falling
        {
            velocity.y -= 9.81f * Time.fixedDeltaTime;
        }
        else if (controller.isGrounded && Input.GetKey(KeyCode.Space))
        {
            Debug.Log("jump");
            velocity.y += jump;
            controller.Move(velocity* Time.fixedDeltaTime * speed);

        }
        else if (controller.isGrounded) velocity.y = 0; 
        Vector3 movePlayer = new Vector3(horizontal, velocity.y, vertical).normalized;
        if (movePlayer.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movePlayer.x, movePlayer.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothedVelocity, smoothAngle);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            moveDirection.y = velocity.y;

            controller.Move(moveDirection.normalized * Time.fixedDeltaTime * speed);
         }
    }
}
