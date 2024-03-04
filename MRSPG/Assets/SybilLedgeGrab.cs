using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SybilLedgeGrab : MonoBehaviour
{
    [SerializeField] Vector3 grabPosition;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float grabForward;

    bool grabbed;

    [SerializeField] float ledgeJumpStr;
    bool dropping;
    //the way this controls is as close as possible to the ledge grab from Pseudoregalia
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputControls.instance.isGrounded)
        {
            dropping = false;
        }
        if (dropping)
            return;
        if(InputControls.instance.velocity.y < 0)
        {
            var handPos = Physics.CheckBox(transform.position + grabPosition + (grabForward * transform.forward), new Vector3(0.1f, 0.1f, 0.1f), transform.rotation);
            var abovePos = Physics.CheckBox(transform.position + grabPosition + new Vector3(0, 0.1f, 0) + (grabForward * transform.forward), new Vector3(0.1f, 0.1f, 0.1f), transform.rotation);
            if (handPos && !abovePos && !grabbed)
            {
                Debug.Log("grabbed");
                grabbed = true;
                InputControls.instance.gravityMultiplier = 0;
                InputControls.instance.velocity.y = 0;
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
            if (InputControls.instance.playerInput.y > 0)
            {
                JumpLedge();
            }
            else if(InputControls.instance.playerInput.y < 0 || InputControls.instance.playerInput.x != 0)
            {
                //drop
                Debug.Log("dropping ledge");
                dropping = true;
                grabbed = false;
                InputControls.instance.gravityMultiplier = 4;
            }
        }



        //2 box cast when the player is falling downward

        //if the box hits ground and the other hits the air, do a ledge grab thing to grant an additional jump

        //ledge grab disables gravity.
        //and grants a jump

        //pressing space or forward will use a jump -requires button down

        //pressing s, a, or d will cause the player to drop



    }

    void JumpLedge()
    {
        Debug.Log("jumping ledge");
        //jump up
        InputControls.instance.gravityMultiplier = 4;
        grabbed = false;
        InputControls.instance.velocity.y += ledgeJumpStr;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position + grabPosition + (grabForward * transform.forward), new Vector3(0.1f, 0.1f, 0.1f));
        Gizmos.DrawWireCube(transform.position + grabPosition + new Vector3(0, 0.1f, 0) + (grabForward * transform.forward), new Vector3(0.1f, 0.1f, 0.1f));
    }
}
