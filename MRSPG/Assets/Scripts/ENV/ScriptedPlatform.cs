using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedPlatform : MonoBehaviour, IEnvTriggered
{
    Vector3 startPos;
    public Vector3 endPos;
    public float speed;
    bool canMove;

    Vector3 r;
    Vector3 u;
    Vector3 f;

    private void Start()
    {
        startPos = transform.position;

        r = endPos.x * transform.right;
        u = endPos.y * transform.up;
        f = endPos.z * transform.forward;
    }
    private void Update()
    {
        var ruf = (r + u + f);
        if (canMove && transform.position!= ruf + startPos)
        {
            var delta = Mathf.Lerp(0.1f, speed, Vector3.Distance(transform.position, ruf + startPos) / Vector3.Distance(startPos, startPos + ruf));
            transform.position = Vector3.MoveTowards(transform.position, ruf + startPos, delta * Time.deltaTime);
        }
    }
    public void Activated(float delay)
    {
        canMove = true;
        foreach(ParticleSystem sys in transform.GetComponentsInChildren<ParticleSystem>())
        {
            sys.Play();
        }
        //StartCoroutine(MoveLoop(delay));
    }
    IEnumerator MoveLoop(float delay)
    {
        float animTime = 0;
        var scale = delay / Time.deltaTime;
        do
        {
            
            Vector3.Lerp(startPos, endPos + startPos, animTime);
            animTime+=Time.deltaTime;
            yield return new WaitForSeconds(0);
        } while (animTime < 1);
        yield return null;
    }
    private void OnDrawGizmosSelected()
    {

        r = endPos.x * transform.right;
        u = endPos.y * transform.up;
        f = endPos.z * transform.forward;

        Vector3 n;
        if (startPos == Vector3.zero)
            n = (r + u + f) + transform.position;
        else
            n = startPos + (r + u + f);
        Debug.DrawLine(n, transform.position, Color.cyan, Time.deltaTime);
        Debug.DrawLine(n + Vector3.down, n+Vector3.up, Color.blue, Time.deltaTime);
        Debug.DrawLine(n + Vector3.left, n + Vector3.right, Color.blue, Time.deltaTime);
        Debug.DrawLine(n + Vector3.back, n + Vector3.forward, Color.blue, Time.deltaTime);
    }
}
