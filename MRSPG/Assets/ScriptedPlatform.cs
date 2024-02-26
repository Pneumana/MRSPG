using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedPlatform : MonoBehaviour, IEnvTriggered
{
    Vector3 startPos;
    public Vector3 endPos;
    public float speed;
    bool canMove;
    private void Start()
    {
        startPos = transform.position;
    }
    private void Update()
    {
        if (canMove && transform.position!=endPos + startPos)
        {
            var delta = Mathf.Lerp(0.1f, speed, Vector3.Distance(transform.position, endPos + startPos) / Vector3.Distance(startPos, startPos + endPos));
            transform.position = Vector3.MoveTowards(transform.position, endPos + startPos, delta * Time.deltaTime);
        }
    }
    public void Activated(float delay)
    {
        Debug.Log("triggered");
        canMove = true;
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
        Vector3 n;
        if (startPos == Vector3.zero)
            n = endPos + transform.position;
        else
            n = startPos + endPos;
        Debug.DrawLine(n, transform.position, Color.cyan, Time.deltaTime);
        Debug.DrawLine(n + Vector3.down, n+Vector3.up, Color.blue, Time.deltaTime);
        Debug.DrawLine(n + Vector3.left, n + Vector3.right, Color.blue, Time.deltaTime);
        Debug.DrawLine(n + Vector3.back, n + Vector3.forward, Color.blue, Time.deltaTime);
    }
}
