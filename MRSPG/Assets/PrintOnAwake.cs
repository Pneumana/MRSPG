using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrintOnAwake : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log(SceneManager.GetActiveScene().name);
    }
}
