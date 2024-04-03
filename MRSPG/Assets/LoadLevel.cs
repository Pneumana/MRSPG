using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Image blackout;
    [SerializeField] TextMeshProUGUI text;

    public string sceneName = "Tutorial";

    public static LoadLevel instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(transform.parent.gameObject);
        GetComponent<Canvas>().enabled = false;
    }

    // Update is called once per frame
    public void Activate(string level)
    {
        sceneName = level;
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        GetComponent<Canvas>().enabled = true;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            blackout.color = new Color(0, 0, 0, t);
            yield return new WaitForSeconds(0);
        }
        image.enabled = true;
        text.enabled = true;

        AsyncOperation loadAction = SceneManager.LoadSceneAsync(sceneName);

        while(!loadAction.isDone)
        {
            image.fillAmount = Mathf.Clamp01(loadAction.progress/0.9f);
            text.text = (Mathf.Round(Mathf.Clamp01(loadAction.progress / 0.9f) * 100)) + "%";
            yield return null;
        }
    }

}
