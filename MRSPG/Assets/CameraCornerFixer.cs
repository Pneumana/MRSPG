using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCornerFixer : MonoBehaviour
{
    public List<Vector3> directions = new List<Vector3>();
    public CinemachineCameraOffset offset;
    public LayerMask mask;

    public float raycastDistance;

    List<RaycastHit> hits = new List<RaycastHit>();
    public Transform playerObj;

    Vector3 local;
    // Start is called before the first frame update
    void Start()
    {
        //convert from world to local

        //playerObj = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        //cast +x -x +z -z in world coords with lets say 3 unit rays. any result move away from them.
        hits.Clear();
        List<Vector3> localDirs = new List<Vector3>();

        for (int i = 0; i < directions.Count; i++)
        {
            localDirs.Add(playerObj.InverseTransformDirection(directions[i]));
        }
        for (int i = 0; i< localDirs.Count; i++)
        {
            RaycastHit temp;
            Physics.Raycast(playerObj.position, localDirs[i], out temp, raycastDistance, mask);
            hits.Add(temp);
        }
        bool noHits = true;
        for (int i = 0; i < hits.Count; i++)
        {
            if (hits[i].collider!=null)
            {
                noHits = false;
                if (Vector3.Distance(hits[i].point, transform.position) < 2.9f)
                    transform.position = Vector3.MoveTowards(transform.position, playerObj.position + (directions[i] * raycastDistance), 5 * Time.deltaTime);
            }
        }
        if (noHits)
        {
            if (transform.position != Vector3.zero)
                transform.position = Vector3.MoveTowards(transform.position, playerObj.position, 5 * Time.deltaTime);
        }
/*        if((Vector3.Distance(playerObj.position, transform.position) != 0))
        {
            transform.position = Vector3.MoveTowards(transform.position, playerObj.position, 5 * Time.deltaTime);
        }*/
        //offset.m_Offset = transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        if(playerObj==null)
            playerObj = transform.parent;
        for (int i = 0; i < directions.Count; i++)
        {
            Debug.DrawLine(playerObj.position, playerObj.position + (directions[i] * raycastDistance), Color.red, Time.deltaTime);
        }
    }

}
