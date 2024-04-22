using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;

public class SybilLedgeGrab : MonoBehaviour
{
    [SerializeField] Vector3 grabPosition;
    [SerializeField] Vector3 grabBounds;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float grabForward;

    [SerializeField] string currentState;

    bool grabbed;

    [SerializeField] float ledgeJumpStr;
    bool dropping;

    [SerializeField] Animator animator;

    //the way this controls is as close as possible to the ledge grab from Pseudoregalia
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputControls.instance.isGrounded)
        {
            DropLedge();
            dropping = false;
            currentState = "grounded";
            return;
        }
        if (dropping)
            return;
        
        if (InputControls.instance.velocity.y < 0)
        {
            Vector3 plusFallspeed = new Vector3(0, InputControls.instance.velocity.y * Time.deltaTime, 0);
            var pos = transform.position + grabPosition + (grabForward * transform.forward);
            var playerUp = transform.position + grabPosition;
            //var handPos = Physics.BoxCast(pos, grabBounds/2, transform.forward, out handGrabbed, transform.rotation, grabBounds.z);
            RaycastHit handHit;
            RaycastHit headCheck;
            var handPos = Physics.CheckBox(pos, (grabBounds + plusFallspeed) / 2, transform.rotation, groundMask, QueryTriggerInteraction.Ignore);
            Physics.Raycast(playerUp + (Vector3.down * (grabBounds.y + plusFallspeed.y / 2)), transform.forward, out handHit, grabBounds.x + grabBounds.z,groundMask, QueryTriggerInteraction.Ignore);
            Physics.Raycast(playerUp + (Vector3.down * (grabBounds.y + plusFallspeed.y / 2)), transform.up, out headCheck, (grabBounds.y) * 2, groundMask, QueryTriggerInteraction.Ignore);

            //var isAboveValid = Physics.CheckBox(pos + new Vector3(0, grabBounds.y + plusFallspeed.y / 2, 0), (grabBounds + plusFallspeed) / 2, transform.rotation, groundMask);
            var abovePos = Physics.CheckBox(pos + new Vector3(0, grabBounds.y + plusFallspeed.y, 0), (grabBounds + plusFallspeed) / 2, transform.rotation, groundMask, QueryTriggerInteraction.Ignore);
            //var abovePos = Physics.BoxCast(pos + new Vector3(0, grabBounds.y, 0), grabBounds/2, transform.forward, transform.rotation, grabBounds.z);
            if (handPos && !abovePos && !grabbed)
            {
                if(headCheck.distance < grabBounds.y * 2 && headCheck.collider!=null)
                {
                    return;
                }
                var start = transform.position + grabPosition + (grabForward * transform.forward);
                var closeLowerLeft = new Vector3(grabBounds.x, -grabBounds.y, -grabBounds.z);
                var closeLowerRight = new Vector3(-grabBounds.x, -grabBounds.y, -grabBounds.z);
                //Debug.DrawLine(start + closeLowerLeft, start + closeLowerRight, Color.green, 10);
                var hand = handHit.point;
                if (hand == Vector3.zero)
                    hand = (playerUp + (Vector3.down * (grabBounds.y + plusFallspeed.y / 2))) + transform.forward * (grabBounds.x + grabBounds.z);
                Debug.DrawLine(playerUp + (Vector3.down * (grabBounds.y + plusFallspeed.y / 2)), hand, Color.blue, 10);
                if(handHit.collider!=null)
                    transform.parent.forward = -handHit.normal;
                else
                {
                    var tries = 1000;
                    while(handHit.collider == null)
                    {
                        if (tries <= 0)
                        {
                            Debug.Log("exhausted attempts to get grab normal");
                            break;
                        }
                        tries--;


                        //check left and right to get normal
                        var r = playerUp + (Vector3.down * (grabBounds.y + plusFallspeed.y / 2)) + (transform.right * Time.deltaTime * (1001 - tries));
                        var l = playerUp + (Vector3.down * (grabBounds.y + plusFallspeed.y / 2)) - (transform.right * Time.deltaTime * (1001 - tries));
                        Physics.Raycast(r, transform.forward, out handHit, grabBounds.x + grabBounds.z, groundMask, QueryTriggerInteraction.Ignore);
                        Debug.DrawLine(r, r + transform.forward, Color.cyan, 5);
                        if(handHit.collider == null)
                        {
                            Physics.Raycast(playerUp + (Vector3.down * (grabBounds.y + plusFallspeed.y / 2)) - (transform.right * Time.deltaTime * (1001 - tries)), transform.forward, out handHit, grabBounds.x + grabBounds.z, groundMask, QueryTriggerInteraction.Ignore);
                            Debug.DrawLine(l, l + transform.forward, Color.blue, 5);
                        }
                    }
                    if (handHit.collider != null)
                    {
                        transform.parent.forward = -handHit.normal;
                    }
                }
                DrawBoxLines(pos, pos + transform.forward * grabBounds.z, grabBounds, Color.green);
                DrawBoxLines(pos + new Vector3(0, grabBounds.y, 0), pos + new Vector3(0, grabBounds.y, 0) + transform.forward * grabBounds.z, grabBounds, Color.red);
                //if (handGrabbed.collider!=null)
                //Debug.Log("grabbed " + handGrabbed.collider.name);
                grabbed = true;
                currentState = "grabbed";
                InputControls.instance.canJump = false;
                InputControls.instance.gravityMultiplier = 0;
                InputControls.instance.velocity.y = 0;
                InputControls.instance.doMovement = false;
                animator.SetBool("LedgeGrabbed", true);
                //Controller.inst.controls.Gameplay.Move.Can
            }
            if (!handPos)
            {
                //DrawBoxLines(pos, pos + transform.forward * grabBounds.z, grabBounds, Color.red);
            }
            if (abovePos)
            {
                //DrawBoxLines(pos + new Vector3(0, grabBounds.y, 0), pos + new Vector3(0, grabBounds.y, 0) + transform.forward * grabBounds.z, grabBounds, Color.green);
            }
            else
            {
                
            }
        }
        
/*        if (handPos && InputControls.instance.velocity.y > 0)
        {
            Debug.Log("grabbing a ledge");
        }*/
        if (grabbed)
        {
            if (Controller.inst.controls.Gameplay.Jump.WasPressedThisFrame())
            {
                JumpLedge();
            }

            if (!Controller.inst.controls.Gameplay.Move.WasPressedThisFrame())
                return;
            if (InputControls.instance.playerInput.y > 0 && !Controller.inst.controls.Gameplay.Jump.WasPressedThisFrame())
            {
                JumpLedge();
            }
            else if(InputControls.instance.playerInput.y < 0 || InputControls.instance.playerInput.x != 0)
            {
                //drop
                DropLedge();
            }
        }



        //2 box cast when the player is falling downward

        //if the box hits ground and the other hits the air, do a ledge grab thing to grant an additional jump

        //ledge grab disables gravity.
        //and grants a jump

        //pressing space or forward will use a jump -requires button down

        //pressing s, a, or d will cause the player to drop



    }
    void DropLedge()
    {
        //Debug.Log("dropping ledge");
        dropping = true;
        grabbed = false;
        InputControls.instance.gravityMultiplier = 4;
        InputControls.instance.doMovement = true;
        animator.SetBool("LedgeGrabbed", false);
        currentState = "dropping";
    }
    void JumpLedge()
    {
        //Debug.Log("jumping ledge");
        //jump up
        InputControls.instance.gravityMultiplier = 4;
        grabbed = false;
        InputControls.instance.velocity.y += ledgeJumpStr;
        InputControls.instance.doMovement = true;
        animator.SetBool("LedgeGrabbed", false);
        currentState = "jumping";
    }

    private void OnDrawGizmos()
    {
        var pos = transform.position + grabPosition + (grabForward * transform.forward);
        //DrawBoxLines(pos, pos + transform.forward * grabBounds.z, grabBounds, true);
        //Gizmos.DrawCube(transform.position + grabPosition + (grabForward * transform.forward), grabBounds);
        //Gizmos.DrawWireCube(transform.position + grabPosition + new Vector3(0, grabBounds.y, 0) + (grabForward * transform.forward), grabBounds);
    }

    protected void DrawBoxLines(Vector3 p1, Vector3 p2, Vector3 extents, Color color)

    {

        var length = (p2 - p1).magnitude;

        var halfExtents = extents / 2;

        var halfExtentsZ = transform.forward * halfExtents.z;

        var halfExtentsY = transform.up * halfExtents.y;

        var halfExtentsX = transform.right * halfExtents.x;

        /*if (boxes)

        {

            var matrix = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(p1, transform.rotation, Vector3.one);

            Gizmos.DrawWireCube(Vector3.zero, extents);

            Gizmos.matrix = Matrix4x4.TRS(p2, transform.rotation, Vector3.one);

            Gizmos.DrawWireCube(Vector3.zero, extents);

            Gizmos.matrix = matrix;

        }*/

        // draw connect lines 1

        Debug.DrawLine(p1 - halfExtentsX - halfExtentsY - halfExtentsZ, p2 - halfExtentsX - halfExtentsY - halfExtentsZ, color, 10);

        Debug.DrawLine(p1 + halfExtentsX - halfExtentsY - halfExtentsZ, p2 + halfExtentsX - halfExtentsY - halfExtentsZ, color, 10);

        Debug.DrawLine(p1 - halfExtentsX + halfExtentsY - halfExtentsZ, p2 - halfExtentsX + halfExtentsY - halfExtentsZ, color, 10);

        Debug.DrawLine(p1 + halfExtentsX + halfExtentsY - halfExtentsZ, p2 + halfExtentsX + halfExtentsY - halfExtentsZ, color, 10);

        // draw connect lines 2

        Debug.DrawLine(p1 - halfExtentsX - halfExtentsY + halfExtentsZ, p2 - halfExtentsX - halfExtentsY + halfExtentsZ,color, 10);

        Debug.DrawLine(p1 + halfExtentsX - halfExtentsY + halfExtentsZ, p2 + halfExtentsX - halfExtentsY + halfExtentsZ, color, 10);

        Debug.DrawLine(p1 - halfExtentsX + halfExtentsY + halfExtentsZ, p2 - halfExtentsX + halfExtentsY + halfExtentsZ, color, 10);

        Debug.DrawLine(p1 + halfExtentsX + halfExtentsY + halfExtentsZ, p2 + halfExtentsX + halfExtentsY + halfExtentsZ, color, 10);

    }

}
