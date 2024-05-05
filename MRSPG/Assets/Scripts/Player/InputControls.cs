using Cinemachine.Utility;
//using Mono.Cecil.Cil;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using Unity.VisualScripting;

public class InputControls : MonoBehaviour
{
    #region Variables
    [Header("Player Variables")]
    public CharacterController controller;
    public GameObject player;
    public Transform playerObj;
    private MeleeHitbox meleeHitbox;
    public float speed;
    public float jump;

    [Header("Dash")]
    public float dashSpeed;
    public float dashTime;
    public bool canDash = true;
    public bool dashing = false;
    public float dashCooldown;
    //public float dashBoost;
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

    GameObject[] Bounds;

    bool playedLand;
    bool playedJump;
    int noGroundFrames;

    Coroutine combatSlowLoop;

    public float movespeedMultiplier = 1;
    public float CombatTimer = 10;
    public float CombatMultiplier = 0.75f;

    [SerializeField] Animator animator;

    public static InputControls instance;

    private CutsceneLogic cutsceneLogic;

    #endregion

    private void Start()
    {
        Bounds = GameObject.FindGameObjectsWithTag("Boundary");
        cam = Camera.main.transform;
        player = GameObject.Find("Player");
        meleeHitbox = GameObject.Find("MeleeHitbox").GetComponent<MeleeHitbox>();
        cutsceneLogic = player.GetComponent<CutsceneLogic>();
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
        
        if (!cutsceneLogic.ActiveCutscene) MovePlayer(movePlayer);
        foreach(GameObject obj in Bounds)
        {
            if(obj.GetComponent<BattleBounds>().distance < obj.GetComponent<BattleBounds>().boundSize)
            {
                obj.GetComponent<BattleBounds>().PlayerWithinBoundary = true;
                StopCoroutine(obj.GetComponent<BattleBounds>().DrainHP());
            }
            else
            {
                obj.GetComponent<BattleBounds>().PlayerWithinBoundary = false;
            }
        }
    }

    public void ApplyGravity()
    {
        if (!doGravity)
            return;
        var _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        RaycastHit hit;
        var dist = Mathf.Abs((velocity.y * Time.deltaTime) * 1);
        dist = Mathf.Max(dist, 10 * Time.deltaTime);
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, out hit, dist , groundMask);
        if(!isGrounded)
            noGroundFrames++;
        else
        {
            noGroundFrames = 0;
        }
        if (noGroundFrames > 15)
            
        Debug.DrawLine(groundCheck.position, groundCheck.position + (Vector3.up * ((velocity.y * Time.deltaTime) * 1)), Color.cyan, Time.deltaTime);
        Debug.DrawLine(groundCheck.position, hit.point, Color.blue, Time.deltaTime);


        if (isGrounded && velocity.y < 0 || _isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            animator.SetBool("Falling", false);
            canJump = true;
            //dashBoost = 0;
            if (!playedLand)
            {
                groundedParticles.Play();
                playedLand = true;
            }
        }

        if(!canJump)
        {
            Waiter(1f);
            animator.SetBool("Falling", true);
            playedLand = false;
        }
    }

    public IEnumerator ApplyDash(Vector3 direction, float speed, float time, bool liveUpdate, string type)
    {
        animator.SetTrigger("Dash");
        float startTime = Time.time;
        if (type == "Movement")
        {
            canDash = false;
            dashing = true;
            DashParticle.Play();
            //Sounds.instance.PlaySFX ( "Dash" );
        }
        while (Time.time < startTime + time)
        {
            if (liveUpdate)
            {
                targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + transform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothedVelocity, smoothAngle);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 targetDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
                controller.Move(targetDirection.normalized * speed * movespeedMultiplier * Time.deltaTime);
            }
            else
            {
                controller.Move(direction.normalized * speed * movespeedMultiplier * Time.deltaTime);
            }

            if (type == "Movement")
            {
                //Dash into enemy here:
                Collider[] hitEnemies = Physics.OverlapSphere(playerObj.position, 2.5f, enemyLayer);
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

            if (type == "MeleeSlide")
            {
                if (meleeHitbox.EnemyInRange())
                {
                    startTime -= 2 * Time.deltaTime;
                }
            }

            yield return null;
        }
        if (type == "Movement")
        {
            DashParticle.Stop();
            targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + transform.eulerAngles.y;
            float extensionAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothedVelocity, smoothAngle);
            /*while (dashBoost > 0)
            {
                targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + transform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothedVelocity, smoothAngle);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 targetDirection = (Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward).normalized;
                controller.Move(targetDirection * speed * dashBoost * Time.deltaTime / 4f);
                dashBoost -= Time.deltaTime * 1.5f;
                if (Mathf.Abs(extensionAngle - angle) > 15) { break; }
                yield return null;
            }*/
            dashing = false;
        }
    }

    IEnumerator Waiter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        canDash = true;
    }
    public void MovePlayer(Vector3 movePlayer)
    {
        if (doMovement)
            movePlayer = new Vector3(playerInput.x, 0f, playerInput.y);

        velocity.y += gravity * gravityMultiplier * Time.deltaTime;
        if (controller.enabled)
            controller.Move(velocity * Time.deltaTime * movespeedMultiplier);

        if (movePlayer.magnitude >= 0.1f)
        {
            animator.SetBool("Walking", true);


            float targetAngle = Mathf.Atan2(movePlayer.x, movePlayer.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref smoothedVelocity, smoothAngle);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * playerInput.magnitude;
            if(controller.enabled)
                controller.Move(moveDirection * Time.deltaTime * speed * movespeedMultiplier);
        }
        else
        {
            animator.SetBool("Walking", false);
        }
    }


#region Actions:
    public void OnJump(InputAction.CallbackContext context)
    {
        if (cutsceneLogic.ActiveCutscene) { return; }
        if (canJump)
        {
            animator.SetTrigger("Jump");
            ApplyJump();
            canJump = false;
        }
    }

    public void ApplyJump()
    {
        velocity.y = Mathf.Sqrt(jump * -2f * gravity);
        //if (dashing) { dashBoost = 1; }
        playedLand = false;
        jumpParticles.Play();
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
        if(cutsceneLogic.ActiveCutscene) { return; }
        if(canDash && !dashing)
        {
            StartCoroutine(ApplyDash(movePlayer, dashSpeed, dashTime, true, "Movement"));
            StartCoroutine(Waiter(dashCooldown));
        }
    }
#endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, 0.4f);
        Gizmos.DrawWireSphere(playerObj.position, 2.5f);
    }

    public void CombatMovementSlow()
    {
        if (combatSlowLoop == null)
        {
            combatSlowLoop = StartCoroutine(CombatSlowLoop());
        }
        else
        {
            StopCoroutine(combatSlowLoop);
            combatSlowLoop = StartCoroutine(CombatSlowLoop());
        }
    }
    IEnumerator CombatSlowLoop()
    {
        movespeedMultiplier = CombatMultiplier;

        yield return new WaitForSeconds(CombatTimer);

        movespeedMultiplier = 1;

        yield return null;
    }
}
