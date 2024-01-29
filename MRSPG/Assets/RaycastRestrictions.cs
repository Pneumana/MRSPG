using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastRestrictions : MonoBehaviour
{
    public GameObject player;
    public Vector3 offset;
    public Cinemachine.CinemachineFreeLook playerCamHolder;

    public float maxCameraDist;

    private void Start()
    {
        //player = GameObject.Find("Player");
    }

    private void LateUpdate()
    {
        RaycastHit up;
        RaycastHit back;
        RaycastHit down;
        //offset = transform.localPosition;
        var pos = player.transform.position;
        var dir = (Camera.main.transform.position - player.transform.position).normalized;

        Physics.Raycast(pos, dir, out back, maxCameraDist);

        Vector3 upResult;
        Vector3 backResult;
        Vector3 downResult;


        if (back.point == Vector3.zero)
        {
            backResult = pos + (dir * maxCameraDist);
        }
        else
        {
            backResult = back.point - (dir * 0.1f) + (Vector3.up * 0.01f);
        }
        offset = backResult;
        Physics.Raycast(backResult + Vector3.up * 0.1f, Vector3.down, out down, maxCameraDist);
        Physics.Raycast(backResult + Vector3.down * 0.1f, Vector3.up, out up, maxCameraDist);

        if (up.point == Vector3.zero)
        {
            upResult = pos + Vector3.up * maxCameraDist;
        }
        else
        {
            upResult = up.point + (Vector3.down * 0.01f);
        }



        if (down.point == Vector3.zero)
        {
            downResult = pos + Vector3.down * maxCameraDist;
        }
        else
        {
            downResult = up.point + (Vector3.up * 0.01f);
        }

        Debug.DrawLine(pos, dir * maxCameraDist, Color.blue, Time.deltaTime);
        Debug.DrawLine(pos, backResult, Color.green, Time.deltaTime);
        /*Debug.DrawLine(back.point, up.point, Color.cyan, Time.deltaTime);
        Debug.DrawLine(back.point, down.point, Color.red, Time.deltaTime);*/
        // the result is the furthest the cam should be allowed to go?
        playerCamHolder.m_Orbits[0].m_Height = Mathf.Clamp(8, 0, Vector3.Distance(pos, upResult));
        playerCamHolder.m_Orbits[1].m_Radius = Mathf.Clamp(10, 0, Vector3.Distance(pos, backResult));
        playerCamHolder.m_Orbits[2].m_Radius = playerCamHolder.m_Orbits[1].m_Radius * 0.8f;

        //2 will remain for the top radius
        //bottom ring is 80% of middle ring

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
