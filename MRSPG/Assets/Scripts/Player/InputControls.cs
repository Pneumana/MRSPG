using Cinemachine.Utility;
//using Mono.Cecil.Cil;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class InputControls : MonoBehaviour
{
    #region Variables
    [Header("Player Variables")]
    public CharacterController controller;
    public Transform playerObj;
    public float speed;
    public float jump;

    [Header("Dash")]
    public float dashSpeed;
    public float dashTime;
    public bool canDash = true;
    public float dashCooldown;
    private float targetAngle;
    public LayerMask enemyLayer;
    [SerializeField] ParticleSystem DashParticle;

    public Vector3 velocity;
    public Vector3 moveDirection;
    private Vector3 movePlayer;
    public Vector2 playerInput;

    [Header("Push")]
    public Vector3 pushDirection;
    public float pushSpeed;
    public float pushTime;

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
    public bool isGrounded;

    //Controller Support:
    ControllerSupport controls;
    public bool canJump;
    public bool doMovement = true;
    public bool doGravity = true;

    [SerializeField] ParticleSystem groundedParticles;
    [SerializeField] ParticleSystem jumpParticles;

    #endregion

    public static InputControls instance;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        DashParticle.Stop();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.X) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        ApplyGravity();
        MovePlayer(movePlayer);
    }

    public void ApplyGravity()
    {
        var prev = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && !prev)
            groundedParticles.Play();

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            canJump = true;
            
        }
    }

    public IEnumerator ApplyDash(Vector3 direction, float speed, float time, bool liveUpdate, string type)
    {
        if (type == "Movement") { canDash = false; }
        float startTime = Time.time;
        DashParticle.Play();
        while (Time.time < startTime + time)
        {
            if (liveUpdate)
            {
                targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + transform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothedVelocity, smoothAngle);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 targetDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
                controller.Move(targetDirection.normalized * speed * Time.deltaTime);
            }
            else
            {
                controller.Move(direction.normalized * speed * Time.deltaTime);
            }

            if (type == "Movement")
            {
                //Dash into enemy here:
                Collider[] hitEnemies = Physics.OverlapSphere(playerObj.position, 1f, enemyLayer);
                foreach (Collider enemy in hitEnemies)
                {
                    if (enemy.gameObject.GetComponent<NavMeshAgent>() != null) { enemy.gameObject.GetComponent<NavMeshAgent>().enabled = false; }
                    Vector3 enemyDirection = (enemy.transform.position - playerObj.position).normalized;
                    enemyDirection.y = 0f;
                    var body = enemy.GetComponent<EnemyBody>();
                    if (body != null)
                    {
                        body.HitByPlayerDash(transform);
                    }
                }
            }

            yield return null;
        }
        DashParticle.Stop();
    }

    IEnumerator Waiter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        canDash = true;
    }
    public void MovePlayer(Vector3 movePlayer)
    {
        if(doMovement)
            movePlayer = new Vector3(playerInput.x, 0f, playerInput.y);

        velocity.y += gravity * gravityMultiplier * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (movePlayer.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(movePlayer.x, movePlayer.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothedVelocity, smoothAngle);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * playerInput.magnitude;
            controller.Move(moveDirection * Time.deltaTime * speed);
        }
    }


#region Actions:
    public void OnJump(InputAction.CallbackContext context)
    {
        if (canJump)
        {
            canJump = false;
            velocity.y = Mathf.Sqrt(jump * -2f * gravity);
            jumpParticles.Play();
        }

    }

    public void ForceJump()
    {
        if (canJump)
        {
            canJump = false;
            velocity.y = Mathf.Sqrt(jump * -2f * gravity);
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if(canDash)
        {
            StartCoroutine(ApplyDash(movePlayer, dashSpeed, dashTime, true, "Movement"));
            StartCoroutine(Waiter(dashCooldown));
        }
    }
#endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, 0.4f);
        Gizmos.DrawWireSphere(playerObj.position, 1f);
    }
}
