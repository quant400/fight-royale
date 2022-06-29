using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Mirror;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ThrowableBehavior : NetworkBehaviour
{
    private Transform defaultParent;
    
    [SerializeField] private Transform carryTargetTransform;
    [SerializeField] private PlayerBehaviour lastThrower;
    [SerializeField] public Rigidbody rb;
    [SerializeField] private MeshCollider collider;

    [SerializeField] private float _throwForce;
    [SerializeField] private bool _isDanger = false;

    public override void OnStartClient()
    { 
        gameObject.SetActive(true);
        defaultParent = transform.parent;
    }

    public void AdjustPosition(Transform target)
    {
        Vector3 pos = new Vector3(-0.13f, 0.42f, -0.04f);
        Vector3 rot = new Vector3(37,110,37);

        transform.localPosition = pos;
        transform.localRotation = Quaternion.Euler(rot);

        return;
    }

    public void Carry(Transform parent)
    {
        //_networkTransformChild.target = parent;
        rb.isKinematic = true;
        collider.isTrigger = true;
        transform.SetParent(parent);
        AdjustPosition(parent);
        
    }
    
    public void Throw(Transform playerTransform)
    {
        //_networkTransformChild.target = defaultParent;
        var currentPos = transform.position;
        var currentRot = transform.rotation;
        transform.SetParent(defaultParent);
        //Debug.Log(transform.position);
        rb.isKinematic = false;
        collider.isTrigger = false;
        CmdThrow(currentPos, currentRot, playerTransform, _throwForce);
    }

    [Command]
    public void CmdThrow(Vector3 pos, Quaternion rot, Transform playerTransform, float throwForce)
    {
        transform.position = pos;
        transform.rotation = rot;
        transform.localScale = Vector3.one;
        
        var throwDir = (playerTransform.forward + (transform.up /10)) * throwForce;
        rb.AddForce(throwDir);
        
        lastThrower = playerTransform.GetComponent<PlayerBehaviour>();
        _isDanger = true;
        netIdentity.RemoveClientAuthority(connectionToClient);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isServer) return;
        
        if (!_isDanger || collision.transform == lastThrower.transform) return;
        else
        {
            _isDanger = false;

            if (collision.gameObject.tag == "Player")
            {
                var target = collision.gameObject.GetComponent<PlayerBehaviour>();
                lastThrower.OnDamage(target.netIdentity, 15);
            }
            
            lastThrower = null;
            carryTargetTransform = null;
        }
    }



}
