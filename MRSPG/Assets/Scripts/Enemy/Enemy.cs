using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemySetting EnemySetting;
    private GameObject _player;

    public void Start()
    {
        _player = GameObject.Find("Player/PlayerObj");
    }
    public void Update()
    {
        Vector3 _target = _player.transform.position;
        float _distFromPlr = Vector3.Distance(transform.position, _target);
        Debug.Log("Player Position: " + _target + "Distance: " + _distFromPlr);
    }

}
