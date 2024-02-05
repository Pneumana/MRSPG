using Mono.Cecil.Cil;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputControls : MonoBehaviour
{
    #region Variables
    [Header("Player Variables")]
    public CharacterController controller;
    public float speed;
    public float jump;
    [Header("Dash")]
    public float dashSpeed;
    public float dashTime;
    private bool canDash = true;
    public float dashCooldown;

    private Vector3 velocity;
    private Vector3 moveDirection;
    private Vector3 movePlayer;
    [HideInInspector] public Vector2 playerInput;

    [Header("Camera")]
    public Transform cam;
    private float smoothAngle = 0.05f;
    private float smoothedVelocity;

    [Header("Ground Variables")]
    public float gravityMultiplier;
    private float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;
    public bool isGrounded = false;

    //Controller Support:
    ControllerSupport controls;
    private bool isJumping;
    #endregion



    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.X) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        ApplyGravity();
        MovePlayer(movePlayer);
        if(isGrounded) { isJumping = false; }
    }

    public void ApplyGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    public IEnumerator ApplyDash(Vector3 direction)
    {
        canDash = false;
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothedVelocity, smoothAngle);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 targetDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            // Subtract the camera from the direction to stop the weird movement
            controller.Move(targetDirection.normalized * dashSpeed * Time.deltaTime);
            yield return null;
        }
    }


    IEnumerator Waiter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        canDash = true;
    }
    public void MovePlayer(Vector3 movePlayer)
    {
        movePlayer = new Vector3(playerInput.x, 0f, playerInput.y).normalized;

        velocity.y += gravity * gravityMultiplier * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (movePlayer.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movePlayer.x, movePlayer.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothedVelocity, smoothAngle);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * Time.deltaTime * speed);
        }
    }


#region Actions:
    public void OnJump(InputAction.CallbackContext context)
    {
        if(!isJumping)
        {
            Debug.Log("jumping");
            velocity.y = Mathf.Sqrt(jump * -2f * gravity);
            isJumping = true;
            isGrounded = false;
        }

    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if(canDash)
        {
            StartCoroutine(ApplyDash(movePlayer));
            StartCoroutine(Waiter(dashCooldown));
        }
    }



#endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, 0.4f);
    }
}
