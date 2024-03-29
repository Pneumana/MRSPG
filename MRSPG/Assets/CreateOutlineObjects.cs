using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOutlineObjects : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Material mat;
    void Start()
    {
        foreach(Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if (child == transform)
                continue;
            var clone = new GameObject();
            clone.name = child.name;
            clone.transform.parent = child;
            clone.transform.localScale = Vector3.one * 1.1f;
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localRotation = Quaternion.Euler(Vector3.zero);

            clone.AddComponent<MeshFilter>();
            clone.GetComponent<MeshFilter>().mesh = child.GetComponent<SkinnedMeshRenderer>().sharedMesh;

            clone.AddComponent<MeshRenderer>();
            clone.GetComponent<MeshRenderer>().material = mat;

            /*try
            {
                clone.GetComponent<MeshRenderer>().material = mat;
            }
            catch { Debug.Log("No mesh renderer"); }

                for (int i = 0; i <  clone.GetComponent<SkinnedMeshRenderer>().materials.Length; i++)
                {
                    clone.GetComponent<SkinnedMeshRenderer>().materials[i] = mat;
                }*/

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
