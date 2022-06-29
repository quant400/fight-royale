using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardFX : MonoBehaviour
{
    public Transform camTransform;

    Quaternion originalRotation;

    void Start()
    {
        originalRotation = transform.rotation;
    }

    void Update()
    {
        if (camTransform == null)
            camTransform = Camera.main.transform;
        else
            transform.LookAt(camTransform);
    }
}