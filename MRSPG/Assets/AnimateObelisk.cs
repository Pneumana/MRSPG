using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateObelisk : MonoBehaviour
{
    [SerializeField] GameObject obeliskPivot;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] Vector3 targetRotation;

    [SerializeField] ParticleSystem emitter;

    Vector3 startPos;
    Vector3 startRot;
    private void Start()
    {
        startPos = obeliskPivot.transform.position;
        startRot = obeliskPivot.transform.rotation.eulerAngles;
    }
    public IEnumerator AnimateWakeUp()
    {
        emitter.Play();
        float animTime = 0;
        do
        {
            animTime+=Time.deltaTime;
            var current = obeliskPivot.transform.position;
            obeliskPivot.transform.position = Vector3.Lerp(startPos, startPos + targetPosition, animTime);
            obeliskPivot.transform.rotation = Quaternion.Euler(Vector3.Lerp(startRot, targetRotation, animTime));
            yield return new WaitForSeconds(0);
        } while (animTime < 1);
        Debug.Log("wake animation is done");
        StartCoroutine(AnimateIdle());
        yield return null;
    }
    public IEnumerator AnimateIdle()
    {
        Debug.Log("starting idle loop");
        float animTime = 0;
        do
        {
            animTime += Time.deltaTime;
            var targetPos = startPos + targetPosition;
            var yoffset = Mathf.Sin(animTime) *0.5f;
            targetPos.y += yoffset;
            obeliskPivot.transform.rotation = Quaternion.Euler(new Vector3(0, animTime * Mathf.Rad2Deg, 0));
            obeliskPivot.transform.position = targetPos;
            yield return new WaitForSeconds(0);
        } while (animTime < Mathf.PI * 2);
        StartCoroutine(AnimateIdle());
        yield return null;
    }
}
