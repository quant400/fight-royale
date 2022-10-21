using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Throwable_BehaviorV2 : NetworkBehaviour
{
    private Vector3 _defaultPos;
    private Quaternion _defaultRot;
    [SerializeField] private float throwForce;

    private Rigidbody rb;
    private MeshCollider collider;


    //Carry variables
    private Transform carrierTarget;
    public NetworkIdentity carrierNetIdentity;

    public bool HasCarrier => carrierTarget != null;

    public void Awake()
    {
        _defaultPos = transform.position;
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<MeshCollider>();
    }

    public void Update()
    {
        if (carrierTarget != null && !isServer)// && carrierNetIdentity.isLocalPlayer) 
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        transform.position = carrierTarget.position;
        transform.rotation = carrierTarget.rotation;
    }

    [ClientRpc]
    public void RpcResetPosition() 
    {
        ResetPosition();
    }

    public void ResetPosition() 
    {
        transform.position = _defaultPos;
        transform.rotation = _defaultRot;
    }

    public bool PickUp(NetworkIdentity carrier, Transform carryTarget) 
    {
        SetNoPhysics(true);

        Physics.IgnoreCollision(collider, carrier.GetComponent<CharacterController>(), true);

        if (carrierNetIdentity != null) return false;

        SetVariable(carrier, carryTarget);

        return true;
    }

    public void SetVariable(NetworkIdentity carrier, Transform carryTarget) 
    {
        carrierTarget = carryTarget;
        carrierNetIdentity = carrier;
    }

    public void SetNoPhysics(bool isTrigger)
    {
        collider.isTrigger = isTrigger;
        rb.isKinematic = isTrigger;
    }

    public void ResetChair() 
    {
        SetVariable(carrierNetIdentity, null);
        SetNoPhysics(false);
    }

    public void Throw() 
    {
        SetNoPhysics(false);

        if (carrierNetIdentity != null) 
        {
            var force = (carrierNetIdentity.transform.forward + (Vector3.up / 10)) * throwForce;
            rb.AddForce(force, ForceMode.Force);
        }       

        carrierTarget = null;
    }



    void OnCollisionEnter(Collision other)
    {
        if (carrierNetIdentity == null || !isServer) return;

        var player = other.gameObject.GetComponent<PlayerBehaviour>();
        var carrier = carrierNetIdentity.GetComponent<PlayerBehaviour>();

        if (player != null) 
        {
            if (player.netId == carrierNetIdentity.netId)
            {
                return;
            }
            else 
            {
                carrier.OnDamage(carrier.netIdentity, player.netIdentity, 15);

            }
        }

        Physics.IgnoreCollision(collider, carrier.GetComponent<CharacterController>(), false);
        RpcResetCollision(carrierNetIdentity);

        carrierNetIdentity = null;
    }

    [ClientRpc]
    public void RpcResetCollision(NetworkIdentity carrier) 
    {
        var lastCarrier = carrier.GetComponent<PlayerBehaviour>();
        ResetCollision(lastCarrier);
    }

    public void ResetCollision(PlayerBehaviour lastCarrier) 
    {
        Physics.IgnoreCollision(collider, lastCarrier._cControler, false);
        carrierNetIdentity = null;
    }
}
