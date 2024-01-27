using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InputControls : MonoBehaviour
{
    #region Variables
    [Header("Player Variables")]
    public CharacterController controller;
    public float speed;
    public float jump;
    public float dash;
    private Vector3 velocity;
    private Vector3 moveDirection;

    [Header("Camera")]
    public Transform cam;
    private float smoothAngle = 0.1f;
    private float smoothedVelocity;

    [Header("Ground Variables")]
    public float gravityMultiplier;
    private float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;
    private bool isGrounded = false;
    #endregion

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.X))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        ApplyGravity();
        ApplyMovement();
    }

    public void ApplyGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    public void ApplyMovement()
    {
        //Get user input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 movePlayer = new Vector3(horizontal, 0f, vertical).normalized;

        velocity.y += gravity * gravityMultiplier * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("Jump");
            velocity.y = Mathf.Sqrt(jump * -2f * gravity);
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            movePlayer = movePlayer * 2f;
        }

        if (movePlayer.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movePlayer.x, movePlayer.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothedVelocity, smoothAngle);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * Time.deltaTime * speed);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, 0.4f);
    }
}
