using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAnimTrigger : MonoBehaviour
{
    public void ForceJump()
    {
        InputControls.instance.ApplyJump();
    }
}
