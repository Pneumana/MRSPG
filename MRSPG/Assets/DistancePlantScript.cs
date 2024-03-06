using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistancePlantScript : MonoBehaviour
{
    [SerializeField] MeshRenderer targetMesh;
    Transform player;
    float change;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerObj").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (player!=null)
        {
            var dist = Vector3.Distance(transform.position, player.position);
            if (dist < 5)
            {
                if (change < 1)
                    change += Time.deltaTime;
            }
            else
            {
                if(change > 0)
                    change -= Time.deltaTime;
            }
            change = Mathf.Clamp01(change);
            var f = Mathf.Lerp(0, 1, change);
            targetMesh.materials[1].SetFloat("_Float", f);
        }

    }
}
