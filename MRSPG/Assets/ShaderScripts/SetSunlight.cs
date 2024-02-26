using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSunlight : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Shader.SetGlobalVector("_SunDirection", transform.forward);
    }

}
