using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossEnd : MonoBehaviour
{
    private void OnDestroy()
    {
        Debug.Log("Ooops");
        //SceneManager.LoadScene("Level 1.5");
    }
}
