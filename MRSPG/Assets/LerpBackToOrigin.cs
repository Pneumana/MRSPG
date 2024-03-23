using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpBackToOrigin : MonoBehaviour
{
    Vector3 origin;
    bool goingBack;

    [SerializeField] float waitTime;

    private void Start()
    {
        origin = transform.position;
    }

    private void Update()
    {
        if (transform.hasChanged && !goingBack)
        {
            goingBack = true;
            StartCoroutine(GoBack());
        }
    }
    IEnumerator GoBack()
    {
        yield return new WaitForSeconds(waitTime);

        do
        {
            var lerp = Vector3.Lerp(transform.position, origin, 2f * Time.deltaTime);
            var move = Vector3.MoveTowards(transform.position, origin, Time.deltaTime);
            Vector3 change;
            if(Vector3.Distance(lerp, transform.position) > Vector3.Distance(move, transform.position))
                change = lerp;
            else
                change = move;
            transform.position = change;
            yield return new WaitForSeconds(0);
        } while (transform.position != origin);
        goingBack = false;
        yield return null;
    }
}
