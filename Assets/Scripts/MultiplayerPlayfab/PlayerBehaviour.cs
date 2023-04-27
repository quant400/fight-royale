using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CFC;
using Cinemachine;
using Mirror;
using Newtonsoft.Json;
using PlayFab;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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

    public NetworkIdentity spectating;

    public bool isDemo = false;

    public static readonly HashSet<string> playerNames = new HashSet<string>();
    [Header("Data")]
    [SyncVar(hook = nameof(OnPlayerNameChanged))] public string pName;
    [SyncVar(hook = nameof(OnPlayerSkinChanged))] public string pSkin;
    [SyncVar(hook = nameof(OnHealthChanged))] public float pHealth = 100f;
    [SyncVar] public Color32 pColor;
    //[SyncVar(hook = nameof(OnScoreChanged))] public int score;

    [SyncVar(hook = nameof(OnPlayerBlock))] public bool pIsBlocking;

   /* private void OnScoreChanged(int oidScore, int newScore)
    {
        IngameUIControler.instance.UpdateScore(newScore);
    }*/
    private void OnPlayerBlock(bool old, bool value)
    {
        Debug.Log(value);
        //_tpControler.enabled = !value;
    }
    private void OnDestroy()
    {
        if(!netIdentity.isLocalPlayer)
            IngameUIControler.instance.RemovePlayer(netIdentity);
    }

    #region Unity Callbacks

    void Start()
    {
        Debug.Log(isClient);
        if (!isServer)
        {
            SetupComponents();
            Debug.Log("ok");
        }
        
        //Debug.Log(_pAttributes.category);

        _pStatsController.SetHealth(pHealth);
        if (isLocalPlayer)
        {
            _skinController.SetUpSkin(Character_Manager.Instance.GetCurrentCharacter.name);
            pColor = Color_Manager.Instance.pallete.RandomPlayerColor();
        }


    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "DeadlyArea":
                Death(null);
                break;
            case "PowerUp":
                /*if(hasAuthority && isClient)
                    CmdPowerUp(other.GetComponent<NetworkIdentity>());
                Debug.Log(other.GetComponent<NetworkIdentity>().netId);*/

                if (hasAuthority && isClient)
                    CmdPowerUp(other.GetComponent<PowerUpBehavior>().id);

                //_pAttributes.OnPowerUp(other.GetComponent<PowerUpBehavior>().GetPowerUp());
                break;
        }
    }

    [Command]
    private void CmdPowerUp(int powerUpId)
    {
        Debug.Log("CmdPowerUp: "+powerUpId);

        var powerUp = PowerUpManager.Instance.GetPowerUpById(powerUpId);

        if (powerUp != null && powerUp.isAvailable) 
        {
            powerUp.isAvailable = false;
            RpcPowerUp(powerUpId);
        }
    }

    [ClientRpc]
    private void RpcPowerUp(int powerUpId)
    {

        Debug.Log("RpcPowerUp: " + powerUpId);

        var powerUp = PowerUpManager.Instance.GetPowerUpById(powerUpId);

        if (powerUp != null) 
        {
            powerUp.isAvailable = false;
            _pAttributes.OnPowerUp(powerUp.GetPowerUp());
        }
    }

    #endregion

    private void SetupComponents()
    {
        _pStatsBillBoard.camTransform = Camera.main.transform;

        if (!isLocalPlayer)
        {
            Debug.LogError("!Local");
            _pInput.enabled = false;
            //_cControler.enabled = false;
            _tpControler.enabled = false;
            _tpFightingControler.enabled = false;
            IngameUIControler.instance.AddPlayer(netIdentity, gameObject);
        }
        else
        {
            Debug.LogError("Local");
            _pInput.enabled = true;
            Camera_Manager.Instance.followCam.m_Follow = _camTarget;
            ChatGlobal_Manager.Instance.player = this;
            _pAttributes.gameObject.SetActive(true);
            IngameUIControler.instance.AddLocalPlayer(netIdentity);
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

    public void OnDeath(NetworkIdentity killerIdentity)
    {
        if (isLocalPlayer)
        {
            MenuManager.Instance.ShowKO();
            CmdOnDeath(killerIdentity);
        }

    }

    private void Death(NetworkIdentity killerIdentity)
    {
        _tpFightingControler.Die();
        _cControler.enabled = false;
        _pInput.enabled = false;
        OnDeath(killerIdentity);
    }
    
    public void OnWin()
    {
        if (isLocalPlayer)
        {
            MenuManager.Instance.ShowWinner();
        }

    }

    private void Win()
    {
        _tpFightingControler.Win();
        _cControler.enabled = false;
        _pInput.enabled = false;
        OnWin();
    }

    [ClientRpc]
    public void RpcWin()
    {
        Win();
    }

    public void DoCall(string callId)
    {
        CmdCall(callId);
    }

    public void OnCall(string callId)
    {
        //CFCNetworkManager.Instance.agora?.onJoin(false, callId);
    }

    public void SendMessage(NetworkIdentity playerIdentity, Color32 color, string message)
    {
        CmdOnSendGlobalMessage(playerIdentity, color, message);
    }

    public override void OnStartServer()
    {
        pName = ((CFCAuth.AuthRequestMessage)connectionToClient.authenticationData).authUsername;
        if (!((CFCAuth.AuthRequestMessage) connectionToClient.authenticationData).nftWallet.Equals("Demo"))
        {
            GameManager.Instance.analytics.AddPlayer(((CFCAuth.AuthRequestMessage)connectionToClient.authenticationData).nftId,netIdentity);
            isDemo = false;
        }
        else
        {
            isDemo = true;
        }
    }

    #region Commands

    [ClientRpc]
    void RpcDamage(NetworkIdentity dealerIdentity, float damage)
    {
        Debug.Log($"RpcDamage: Receive {damage} on {pName}: {dealerIdentity}");

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
            Death(dealerIdentity);
        }

        //Debug.Log($"CmdHealth? {pName} {isServer}");

        if (hasAuthority)
            CmdRemoveHealth(dealerIdentity, finalDamage);

    }

    [TargetRpc]
    public void TargetQuitMatch(NetworkConnection target)
    {
        QuitMatch();
    }
    
    [ClientRpc]
    public void RpcQuitMatch()
    {
        QuitMatch();
    }
    
    public void QuitMatch()
    {
        CFCNetworkManager.singleton.StopClient();
        SceneManager.LoadScene("Menu");
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
    public void CmdRemoveHealth(NetworkIdentity dealerIdentity, float value)
    {
        try
        {
            pHealth  = Mathf.Clamp(pHealth-value, 0,100);
            GameManager.Instance.analytics.AddDamageDealt(dealerIdentity, value);
            GameManager.Instance.analytics.AddDamageReceived(netIdentity, value);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
    }
    
    [Command]
    public void CmdAddHealth(float value)
    {
        pHealth = Mathf.Clamp(pHealth+value, 0,120);
    }

    [Command]
    public void CmdOnDamage(NetworkIdentity dealerIdentity, NetworkIdentity targetIdentity, float damage)
    {
        OnDamage(dealerIdentity, targetIdentity, damage);
    }
    
    public void OnDamage(NetworkIdentity dealerIdentity, NetworkIdentity targetIdentity, float damage)
    {
        if (GameManager.Instance.match.currentState != MatchManager.MatchState.InGame)
        { 
            damage = 0.0f; 
        }

        var target = targetIdentity.GetComponent<PlayerBehaviour>();
        var dealer = dealerIdentity.GetComponent<PlayerBehaviour>();
        Debug.Log($"CmdOnDamage: Receive {damage} damage on {target.pName} from {dealer.pName}");
        if (!isServer) return;

        target.RpcDamage(dealerIdentity, damage);
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
    void CmdOnSendGlobalMessage(NetworkIdentity playerIdentity, Color32 color, string message)
    {
        RpcMessageCreated(playerIdentity, color, message);
    }

    [ClientRpc]
    void RpcMessageCreated(NetworkIdentity playerIdentity, Color32 color, string message)
    {
        ChatGlobal_Manager.Instance.CreateMessage(playerIdentity, color, message);
    }
    
    public void ResetServer()
    {
        CmdResetServer();
    }

    [Command]
    private void CmdResetServer()
    {
        _ResetServer();
    }

    private void _ResetServer()
    {
        CFCNetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
        CFCNetworkManager.singleton.StopServer();
    }

    [Command]
    private void CmdOnDeath(NetworkIdentity killerIdentity)
    {
        GameManager.Instance.analytics.AddKill(killerIdentity, netIdentity);
        GameManager.Instance.CheckWinner();
    }
    
    [ClientRpc]
    public void RpcChangePlayerPosition(Vector3 pos)
    {
        if(Vector3.Distance(transform.position, pos) > 0.2f)
            transform.position = pos;
    }

    [TargetRpc]
    public void TargetChangeSpectatorCamera(NetworkIdentity dead, NetworkIdentity killer) 
    {
        if (spectating == null || spectating == dead)
        {
            spectating = killer;
            Camera_Manager.Instance.followCam.m_Follow = spectating.GetComponent<PlayerBehaviour>()._camTarget;
        }
    }

    #region Pick and Throw

    [Command]
    public void CmdStealObject(NetworkIdentity carrier)
    {
        var auxCarrier = carrier.GetComponent<PlayerBehaviour>();
        auxCarrier._tpFightingControler.StolenObject();

        RpcStealObject(carrier);
    }

    [ClientRpc]
    public void RpcStealObject(NetworkIdentity carrier)
    {
        var auxCarrier = carrier.GetComponent<PlayerBehaviour>();
        auxCarrier._tpFightingControler.StolenObject();
    }

    [Command]
    public void CmdAnimationPickUp(bool isPickUp) 
    {
        //anim.SetLayerWeight(anim.GetLayerIndex("UpperBody"), isPickUp?1:0);
        RpcAnimationPickUp(isPickUp);
    }

    [ClientRpc]
    public void RpcAnimationPickUp(bool isPickUp)
    {
        anim.SetLayerWeight(anim.GetLayerIndex("UpperBody"), isPickUp ? 1 : 0);
    }

    [Command]
    public void CmdPickUp(NetworkIdentity item)
    {
        item.RemoveClientAuthority();
        item.AssignClientAuthority(netIdentity.connectionToClient);

        var pickUpObject = item.GetComponent<Throwable_BehaviorV2>();
        if (pickUpObject.PickUp(netIdentity, _tpFightingControler._throwTargetTransform))
        {
            _tpFightingControler._carryingObject = pickUpObject;

            RpcPickUp(item);
        }
        else 
        {
            RpcStealObject(netIdentity);
        }
        
    }

    [ClientRpc]
    public void RpcPickUp(NetworkIdentity item) 
    {
        var pickUpObject = item.GetComponent<Throwable_BehaviorV2>();
        pickUpObject.PickUp(netIdentity, _tpFightingControler._throwTargetTransform);
        _tpFightingControler._carryingObject = pickUpObject;
        //pickUpObject.SetNoPhysics(true);
    }

    [Command]
    public void CmdThrow(NetworkIdentity item)
    {
        var pickUpObject = item.GetComponent<Throwable_BehaviorV2>();
        pickUpObject.ResetChair();
        _tpFightingControler._carryingObject = null;

        RpcThrow(item);
    }

    [ClientRpc]
    public void RpcThrow(NetworkIdentity item)
    {
        var pickUpObject = item.GetComponent<Throwable_BehaviorV2>();
        pickUpObject.ResetChair();
        _tpFightingControler._carryingObject = null;
    }

    #endregion

    #endregion
}
