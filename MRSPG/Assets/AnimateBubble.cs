using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimateBubble : MonoBehaviour
{
    [SerializeField] Transform[] bubbles;

    List<Vector3> orbits = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform tf in bubbles)
        {
            var roll1 = Random.Range(-1f,1f);
            var roll2 = Random.Range(-1f, 1f);
            var roll3 = Random.Range(-1f, 1f);

            orbits.Add(new Vector3(roll1, roll2, roll3));
        }
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float time = 0;

        while(time < 1)
        {
            time += Time.deltaTime;
            for (int i = 0; i < bubbles.Length; i++)
            {
                bubbles[i].Rotate(orbits[i], Time.deltaTime * 90);
            }

            if (time >= 1)
                time = 0;

            yield return new WaitForSeconds(0);
        }

        


        yield return null;
    }

}
