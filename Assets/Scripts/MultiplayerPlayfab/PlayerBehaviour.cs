using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CFC;
using Cinemachine;
using Mirror;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerBehaviour : NetworkBehaviour
{
    [Header("Components")]
    public Transform _camTarget;
    public PlayerInput _pInput;
    public CFCInputs _cfcInputs;
    public CharacterController _cControler;
    public ThirdPersonController _tpControler;
    public TPFightingController _tpFightingControler;
    public PlayerStatsController _pStatsController;
    public BillboardFX _pStatsBillBoard;
    public Skin_Controller _skinController;
    public PlayerAttributes _pAttributes;
    public Animator anim;

    public static readonly HashSet<string> playerNames = new HashSet<string>();
    [Header("Data")]
    [SyncVar(hook = nameof(OnPlayerNameChanged))] public string pName;
    [SyncVar(hook = nameof(OnPlayerSkinChanged))] public string pSkin;
    [SyncVar(hook = nameof(OnHealthChanged))] public float pHealth = 100f;
    [SyncVar] public Color32 pColor;

    [SyncVar(hook = nameof(OnPlayerBlock))] public bool pIsBlocking;

    private void OnPlayerBlock(bool old, bool value)
    {
        Debug.Log(value);
        //_tpControler.enabled = !value;

    }

    #region Unity Callbacks

    void Start()
    {
        if (isClient)
            SetupComponents();

        _pStatsController.SetHealth(pHealth);
        if (isLocalPlayer)
        {
            _skinController.SetUpSkin(Character_Manager.Instance.GetCurrentCharacter.name);
            pColor = Color_Manager.pallete.RandomPlayerColor();
        }


    }

    void Update()
    {
        if (isLocalPlayer)
        {
        }
    }


    void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "DeadlyArea":
                Death();
                break;
            case "PowerUp":
                _pAttributes.OnPowerUp(other.GetComponent<PowerUpBehavior>().GetPowerUp());
                break;
        }
    }

    #endregion

    private void SetupComponents()
    {
        _pStatsBillBoard.camTransform = Camera.main.transform;

        if (!isLocalPlayer)
        {
            _pInput.enabled = false;
            //_cControler.enabled = false;
            _tpControler.enabled = false;
            _tpFightingControler.enabled = false;
        }
        else
        {
            _pInput.enabled = true;
            Camera_Manager.Instance.followCam.m_Follow = _camTarget;
            ChatGlobal_Manager.Instance.player = this;
            _pAttributes.gameObject.SetActive(true);
            Debug.Log(_pAttributes.category);
        }
    }

    private void OnPlayerNameChanged(string _, string newName)
    {
        _pStatsController.SetName(pName);
    }

    private void OnPlayerSkinChanged(string _, string newSkin)
    {
        _skinController.SetUpSkin(newSkin);
    }

    private void OnHealthChanged(float oldHealth, float newHealth)
    {
        _pStatsController.SetHealth(newHealth);
    }

    /*public void OnDamage(float damage, bool block = false)
    {
        Debug.Log("OnDamage");
        if (!isServer) return;
        if (pHealth - damage > 0)
        {
            pHealth -= damage;
            if (block) RpcBlock(damage);
            else RpcDamage(damage);
        }
        else
        {
            pHealth = 0;
            RpcDeath();
        }
    }*/

    public void OnDeath()
    {
        if (isLocalPlayer)
            MenuManager.Instance.ShowKO();
    }

    private void Death()
    {
        _tpFightingControler.Die();
        _cControler.enabled = false;
        _pInput.enabled = false;
        OnDeath();
    }

    public void DoCall(string callId)
    {
        CmdCall(callId);
    }

    public void OnCall(string callId)
    {
        //CFCNetworkManager.Instance.agora?.onJoin(false, callId);
    }

    public void SendMessage(string playerName, Color32 color, string message)
    {
        CmdOnSendGlobalMessage(playerName, color, message);
    }

    public override void OnStartServer()
    {
        pName = (string)connectionToClient.authenticationData;
    }

    #region Commands

    [ClientRpc]
    void RpcDamage(float damage)
    {
        Debug.Log($"RpcDamage: Receive {damage} on {pName}");

        var finalDamage = pIsBlocking ? _pAttributes.Block(damage) : damage;

        var currentHealth = pHealth;

        if (currentHealth - finalDamage > 0)
        {
            currentHealth -= finalDamage;

            if (pIsBlocking) _tpFightingControler.BlockDamage();
            else _tpFightingControler.TakeDamage();
        }
        else
        {
            currentHealth = 0;
            Death();
        }

        Debug.Log($"CmdHealth? {pName} {isServer}");

        if (hasAuthority)
            CmdSetHealth(currentHealth);

    }

    /*[ClientRpc]
    void RpcBlock(float damage)
    {
        _tpFightingControler.BlockDamage();
    }*/

    /*[ClientRpc]
    void RpcDeath()
    {
        Death();
    }*/

    [Command]
    public void CmdSetHealth(float newHealth)
    {
        pHealth = newHealth;
    }

    [Command]
    public void CmdOnDamage(NetworkIdentity targetIdentity, float damage)
    {
        OnDamage(targetIdentity, damage);
    }
    
    public void OnDamage(NetworkIdentity targetIdentity, float damage)
    {
        var target = targetIdentity.GetComponent<PlayerBehaviour>();
        Debug.Log($"CmdOnDamage: Receive {damage} damage on {target.pName}");
        if (!isServer) return;

        target.RpcDamage(damage);
    }

    [Command]
    public void CmdBlocking(bool blocking)
    {
        pIsBlocking = blocking;
    }

    [Command]
    void CmdCall(string callId)
    {
        RpcCall(callId);
    }

    [ClientRpc]
    void RpcCall(string callId)
    {
        Debug.Log($"Receive call: {callId}");
        OnCall(callId);
    }

    [Command]
    void CmdOnSendGlobalMessage(string playerName, Color32 color, string message)
    {
        RpcMessageCreated(playerName, color, message);
    }

    [ClientRpc]
    void RpcMessageCreated(string playerName, Color32 color, string message)
    {
        ChatGlobal_Manager.Instance.CreateMessage(playerName, color, message);
    }
    
    #region Throwable
    
    [Command]
    public void CmdCarry(NetworkIdentity item)
    {
        item.AssignClientAuthority(connectionToClient);
        RpcCarry(item);
    }
    
    [Command]
    public void CmdThrow(NetworkIdentity item)
    {
        item.RemoveClientAuthority(connectionToClient);
    }

    [ClientRpc]
    public void RpcCarry(NetworkIdentity item)
    {
        _tpFightingControler.Carry(item.GetComponent<ThrowableBehavior>());
    }

    #endregion

    #endregion
}
