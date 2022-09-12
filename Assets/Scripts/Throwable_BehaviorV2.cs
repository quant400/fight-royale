using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable_BehaviorV2 : NetworkBehaviour
{

    [SerializeField] private Transform defaultParent;
    [SerializeField] private float throwForce;

    private Rigidbody rb;
    private MeshCollider collider;


    //Carry variables
    private Transform carrierTarget;
    private NetworkIdentity carrierNetId;
    public bool hasCarrier = false;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<MeshCollider>();
    }

    public void Update()
    {
        if (carrierTarget!=null) 
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        transform.position = carrierTarget.position;
        transform.rotation = carrierTarget.rotation;
    }

    public void Carry(NetworkIdentity carrier, Transform carryTarget) 
    {
        carrierTarget = carryTarget;
        carrierNetId = carrier;
        hasCarrier = true;

        Physics.IgnoreCollision(collider, carrierNetId.GetComponent<CharacterController>(), true);
        //Debug.Log("Reset: " + Physics.GetIgnoreCollision(collider, carrierNetId.GetComponent<CharacterController>()));


        SetNoPhysics(true);
    }

    public void Throw() 
    {
        carrierTarget = null;

        SetNoPhysics(false);

        var force = (carrierNetId.transform.forward + (Vector3.up / 10)) * throwForce;
        rb.AddForce(force);

        carrierNetId.GetComponent<PlayerBehaviour>().CmdThrow(netIdentity);

        //collider.isTrigger = false;
    }

    public void SetNoPhysics(bool isTrigger) 
    {
        collider.isTrigger = isTrigger;
        rb.isKinematic = isTrigger;
    }

    void OnCollisionEnter(Collision other)
    {

        //Debug.Log("OnCollisionEnter: " + other.gameObject.name);

        if (!hasCarrier) return;

        //if (isServer || !isLocalPlayer) return;

        var player = other.gameObject.GetComponent<PlayerBehaviour>();

        if (player != null) 
        {
            if (player.netId == carrierNetId.netId)
            {
                return;
            }
            else 
            {
                var carrier = carrierNetId.GetComponent<PlayerBehaviour>();
                carrier.OnDamage(carrier.netIdentity, player.netIdentity, 15);
            }
        }

        ResetChair();
        carrierNetId = null;
        hasCarrier = false;
    }

    public void ResetChair() 
    {
        try
        {
            //if (isLocalPlayer)
            Physics.IgnoreCollision(collider, carrierNetId.GetComponent<CharacterController>(), false);
            Debug.Log("Reset: " + Physics.GetIgnoreCollision(collider, carrierNetId.GetComponent<CharacterController>()));
        }
        catch (Exception e) 
        {
            Debug.Log("Try: "+e);
        }
        
    }
}
