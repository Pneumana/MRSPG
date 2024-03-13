using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathPlane : MonoBehaviour
{
    public float yStart;
    //start a fade to black here
    public float yEnd;
    public GameObject player;
    bool fell = false;

    private void Update()
    {
        if(player.transform.position.y < yStart)
        {

            //disable player input
            //make camera just look at player
            GameObject.Find("PlayerCam").GetComponent<CinemachineInputProvider>().enabled = false;
            GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>().LookAt = player.transform;
            GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>().Follow = null;
        }
        if(player.transform.position.y < yEnd)
        {
            //start fade
            if (!fell)
            {
                StartCoroutine(FadeToBlack());
                    fell = true;
            }

        }
    }

    IEnumerator FadeToBlack()
    {
        //create black image here

        var b = new GameObject();
        b.name = "FadeOut";
        b.AddComponent<RectTransform>();
        b.AddComponent<Image>();
        b.transform.SetParent(GameObject.Find("Canvas").transform);

        var brec = b.GetComponent<RectTransform>();
        brec.anchoredPosition = Vector2.zero;
        brec.sizeDelta = new Vector2(Screen.width, Screen.height);

        var bimg = b.GetComponent<Image>();

        bimg.color = Color.clear;
        float t = 0;
        do
        {
            t+=Time.deltaTime;
            bimg.color = new Color(0, 0, 0, t);
            yield return new WaitForSeconds(0);
        } while (t < 1);

        //animate a panel here

        //reset player position/spawnpoint
        //re-enable player input.


        GameObject.Find("PlayerCam").GetComponent<CinemachineInputProvider>().enabled = true;
        GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>().LookAt = player.transform;
        GameObject.Find("PlayerCam").GetComponent<CinemachineFreeLook>().Follow = player.transform;

        player.GetComponentInParent<Health>().Die();

        fell = false;
        do
        {
            t -= Time.deltaTime;
            bimg.color = new Color(0, 0, 0, t);
            yield return new WaitForSeconds(0);
        } while (t > 0);
        Destroy(b);
        yield return null;
    }
}
