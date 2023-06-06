using Mirror;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Throwable_BehaviorV2 : MonoBehaviourPun /*NetworkBehaviour*/
{
    private Vector3 _defaultPos;
    private Quaternion _defaultRot;
    [SerializeField] private float throwForce;

    private Rigidbody rb;
    private MeshCollider collider;


    //Carry variables
    private Transform carrierTarget;
    public PhotonView carrierNetIdentity;

    public bool HasCarrier => carrierTarget != null;

    public void Awake()
    {
        _defaultPos = transform.position;
        _defaultRot = transform.rotation;
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<MeshCollider>();
    }

    public void Update()
    {
        if (carrierTarget != null/* && !isServer*/)// && carrierNetIdentity.isLocalPlayer) 
        {
            MoveToTarget();
        }
    }

    private void MoveToTarget()
    {
        transform.position = carrierTarget.position;
        transform.rotation = carrierTarget.rotation;
    }

    // Transferred PUN RPC to PlayerBehaviour.cs
    // TODO Suleman: Uncomment later
    //[ClientRpc]
    [PunRPC]
    public void RpcResetPosition()
    {
        ResetPosition();
    }
    // End Transfer

    public void ResetPosition() 
    {
        transform.position = _defaultPos;
        transform.rotation = _defaultRot;
    }

    public bool PickUp(PhotonView carrier, Transform carryTarget) 
    {
        SetNoPhysics(true);

        Physics.IgnoreCollision(collider, carrier.GetComponent<CharacterController>(), true);

        if (carrierNetIdentity != null) return false;

        SetVariable(carrier, carryTarget);

        return true;
    }

    public void SetVariable(PhotonView carrier, Transform carryTarget) 
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

    //[PunRPC]
    //public void Throw()
    //{
    //    SetNoPhysics(false);

    //    if (carrierNetIdentity != null)
    //    {
    //        var force = (carrierNetIdentity.transform.forward + (Vector3.up / 10)) * throwForce;
    //        rb.AddForce(force, ForceMode.Force);
    //    }

    //    carrierTarget = null;
    //}



    void OnCollisionEnter(Collision other)
    {
        if (carrierNetIdentity == null/* || !isServer*/) return;

        var player = other.gameObject.GetComponent<PlayerBehaviour>();
        var carrier = carrierNetIdentity.GetComponent<PlayerBehaviour>();

        //TODO Suleman: Uncomment Later
        if (player != null)
        {
            if (player.photonView.ViewID == carrierNetIdentity.ViewID)
            {
                return;
            }
            else
            {
                carrier.OnDamage(carrier.photonView, player.photonView, 15);

            }
        }

        Physics.IgnoreCollision(collider, carrier.GetComponent<CharacterController>(), false);
        //RpcResetCollision(carrierNetIdentity);
        /*carrier.*/photonView.RPC("RpcResetCollision", RpcTarget.AllBuffered, carrierNetIdentity.ViewID);

        carrierNetIdentity = null;
    }

    // Transferred PUN RPC to PlayerBehaviour.cs
    // TODO Suleman: Uncomment later
    //[ClientRpc]
    [PunRPC]
    public void RpcResetCollision(int carrierID)
    {
        Debug.Log("Pick and Throw -> RpcResetCollision -> ThrowableBehaviourv2");
        PhotonView carrier = PhotonView.Find(carrierID);
        var lastCarrier = carrier.GetComponent<PlayerBehaviour>();
        ResetCollision(lastCarrier);
    }
    // End Transfer

    public void ResetCollision(PlayerBehaviour lastCarrier) 
    {
        Physics.IgnoreCollision(collider, lastCarrier._cControler, false);
        carrierNetIdentity = null;
    }
}
