using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsBehavior : MonoBehaviour
{
    private Vector3 _pos;
    private Quaternion _rot;

    void Awake()
    {
        this.gameObject.SetActive(true);
        Setup();
    }

    private void Setup()
    {
        _pos = transform.position;
        _rot = transform.rotation;
    }

    public void ResetPosition()
    {
        transform.position = _pos;
        transform.rotation = _rot;
        gameObject.SetActive(true);
    }
}
