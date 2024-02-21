using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;

public class DecalPainter : MonoBehaviour
{
    [SerializeField] private DecalTextureData decalData;
    [SerializeField] private GameObject decalProjector;

    [Serializable]
    public class DecalTextureData
    {
        public Sprite sprite;
        public Vector3 size;
    }

    public IEnumerator PaintDecal(Vector3 point, Vector3 normal, Collision collision)
    {
        GameObject decal = Instantiate(decalProjector, point, Quaternion.identity);
        DecalProjector projector = decal.GetComponent<DecalProjector>();
        projector.material.SetTexture("Base_Map", decalData.sprite.texture);
        projector.size = decalData.size;
        decal.transform.forward = normal;
        Debug.Log($"<color=green>Painted decal on: </color>" + collision);
        yield return new WaitForSeconds(5f);
        Destroy(decal);
    }


}
