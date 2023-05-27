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
using Photon.Pun;
using Models;
using Photon.Realtime;

public class PlayerBehaviour : /*NetworkBehaviour*/MonoBehaviourPunCallbacks
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

    public PhotonView spectating;

    public bool isDemo = false;

    public static readonly HashSet<string> playerNames = new HashSet<string>();
    [Header("Data")]
    // Commented for Photon
    //[SyncVar(hook = nameof(OnPlayerNameChanged))] public string pName;
    //[SyncVar(hook = nameof(OnPlayerSkinChanged))] public string pSkin;
    //[SyncVar(hook = nameof(OnHealthChanged))] public float pHealth = 100f;
    //[SyncVar] public Color32 pColor;

    public string pName;
    public string pSkin;
    public float pHealth = 100f;
    //public Color32 pColor;

    //[SyncVar(hook = nameof(OnScoreChanged))] public int score;

    //TODO Suleman: Uncomment Later, done
    //[SyncVar(hook = nameof(OnPlayerBlock))] public bool pIsBlocking;
    public bool pIsBlocking;

    /* private void OnScoreChanged(int oidScore, int newScore)
     {
         IngameUIControler.instance.UpdateScore(newScore);
     }*/
    private void OnPlayerBlock(bool old, bool value)
    {
        Debug.Log(value);
        //_tpControler.enabled = !value;
    }

    public void SetBoolIsBlocking(bool isBlocking)
    {
        pIsBlocking = isBlocking;
        //photonView.RPC("RPCSetBoolIsBlocking", RpcTarget.All, isBlocking);
    }

    // Transferred PUN RPC to PlayerBehaviour.cs
    [PunRPC]
    private void RPCSetBoolIsBlocking(bool isBlocking)
    {
        pIsBlocking = isBlocking;
    }
    private void OnDestroy()
    {
        // Commented for Photon
        //if(!netIdentity.isLocalPlayer)
        //    IngameUIControler.instance.RemovePlayer(netIdentity);

        if (!photonView.IsMine)
            IngameUIControler.instance.RemovePlayer(photonView);
    }

    #region Unity Callbacks

    void Start()
    {
        //Debug.Log(isClient);

        // Commented for Photon
        //if (!isServer)
        //{
        //    SetupComponents();
        //    //Debug.Log("ok");
        //}

        SetupComponents();

        //Debug.Log(_pAttributes.category);

        _pStatsController.SetHealth(pHealth);
        if (photonView.IsMine)
        {
            //_skinController.SetUpSkin(Character_Manager.Instance.GetCurrentCharacter.name);
            //pColor = Color_Manager.Instance.pallete.RandomPlayerColor();
            ChangePlayerName(Character_Manager.Instance.GetCurrentCharacter.name);
            ChangePlayerSkin(Character_Manager.Instance.GetCurrentCharacter.name.Replace("-"," "));
            //ChangePlayerColor(Color_Manager.Instance.pallete.RandomPlayerColor());
        }


    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
        GameManager.Instance.analytics.AddPlayer(photonView.ViewID.ToString(), photonView);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
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

                // Commented for Photon
                //if (hasAuthority && isClient)
                //    CmdPowerUp(other.GetComponent<PowerUpBehavior>().id);

                if (photonView.IsMine)
                    CmdPowerUp(other.GetComponent<PowerUpBehavior>().id);

                //_pAttributes.OnPowerUp(other.GetComponent<PowerUpBehavior>().GetPowerUp());
                break;
        }
    }

    // Commented for Photon
    //[Command]
    //private void CmdPowerUp(int powerUpId)
    //{
    //    Debug.Log("CmdPowerUp: "+powerUpId);

    //    var powerUp = PowerUpManager.Instance.GetPowerUpById(powerUpId);

    //    if (powerUp != null && powerUp.isAvailable) 
    //    {
    //        powerUp.isAvailable = false;
    //        RpcPowerUp(powerUpId);
    //    }
    //}

    //[ClientRpc]
    //private void RpcPowerUp(int powerUpId)
    //{

    //    Debug.Log("RpcPowerUp: " + powerUpId);

    //    var powerUp = PowerUpManager.Instance.GetPowerUpById(powerUpId);

    //    if (powerUp != null) 
    //    {
    //        powerUp.isAvailable = false;
    //        _pAttributes.OnPowerUp(powerUp.GetPowerUp());
    //    }
    //}

    [PunRPC]
    private void CmdPowerUp(int powerUpId)
    {
        Debug.Log("CmdPowerUp: " + powerUpId);

        var powerUp = PowerUpManager.Instance.GetPowerUpById(powerUpId);

        if (powerUp != null && powerUp.isAvailable)
        {
            powerUp.isAvailable = false;
            RpcPowerUp(powerUpId);
        }

        photonView.RPC("RpcPowerUp", RpcTarget.All, powerUpId);
    }


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

        if (!photonView.IsMine)
        {
            //Debug.LogError("!Local");
            _pInput.enabled = false;
            //_cControler.enabled = false;
            _tpControler.enabled = false;
            _tpFightingControler.enabled = false;
            IngameUIControler.instance.AddPlayer(photonView, gameObject);
        }
        else
        {
            //Debug.LogError("Local");
            _pInput.enabled = true;
            Camera_Manager.Instance.followCam.m_Follow = _camTarget;
            ChatGlobal_Manager.Instance.player = this;
            _pAttributes.gameObject.SetActive(true);
            IngameUIControler.instance.AddLocalPlayer(photonView);
        }
        
    }

    // Commented for Photon
    //private void OnPlayerNameChanged(string _, string newName)
    //{
    //    _pStatsController.SetName(pName);
    //}

    //private void OnPlayerSkinChanged(string _, string newSkin)
    //{
    //    _skinController.SetUpSkin(newSkin);
    //}

    //private void OnHealthChanged(float oldHealth, float newHealth)
    //{
    //    _pStatsController.SetHealth(newHealth);
    //}

    [PunRPC]
    private void SetPlayerName(string newName)
    {
        _pStatsController.SetName(newName);
    }

    [PunRPC]
    private void SetPlayerSkin(string newSkin)
    {
        _skinController.SetUpSkin(newSkin);
    }

    [PunRPC]
    private void SetPlayerHealth(float newHealth)
    {
        _pStatsController.SetHealth(newHealth);
    }

    //[PunRPC]
    //private void SetPlayerColor(Color32 newColor)
    //{
    //    pColor = newColor;
    //}

    public void ChangePlayerName(string newName)
    {
        photonView.RPC("SetPlayerName", RpcTarget.AllBuffered, newName);
    }

    public void ChangePlayerSkin(string newSkin)
    {
        photonView.RPC("SetPlayerSkin", RpcTarget.AllBuffered, newSkin);
    }

    public void ChangePlayerHealth(float newHealth)
    {
        photonView.RPC("SetPlayerHealth", RpcTarget.AllBuffered, newHealth);
    }

    //public void ChangePlayerColor(Color32 newColor)
    //{
    //    photonView.RPC("SetPlayerColor", RpcTarget.AllBuffered, newColor);
    //}

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

    public void OnDeath(PhotonView killerIdentity)
    {
        if (photonView.IsMine)
        {
            MenuManager.Instance.ShowKO();
            //TODO Suleman: Uncomment Later
            photonView.RPC("CmdOnDeath", RpcTarget.All, killerIdentity.ViewID);
        }

    }

    private void Death(PhotonView killerIdentity)
    {
        _tpFightingControler.Die();
        _cControler.enabled = false;
        _pInput.enabled = false;
        OnDeath(killerIdentity);
    }
    
    public void OnWin()
    {
        if (photonView.IsMine)
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

    //[ClientRpc]
    [PunRPC]
    public void RpcWin()
    {
        Win();
    }

    public void DoCall(string callId)
    {
        //TODO Suleman: Uncomment Later
        //CmdCall(callId);
    }

    public void OnCall(string callId)
    {
        //CFCNetworkManager.Instance.agora?.onJoin(false, callId);
    }

    public void SendMessage(NetworkIdentity playerIdentity, Color32 color, string message)
    {
        //TODO Suleman: Uncomment Later
        //CmdOnSendGlobalMessage(playerIdentity, color, message);
    }

    // Commented for Photon
    //public override void OnStartServer()
    //{
    //    pName = ((CFCAuth.AuthRequestMessage)connectionToClient.authenticationData).authUsername;
    //    if (!((CFCAuth.AuthRequestMessage) connectionToClient.authenticationData).nftWallet.Equals("Demo"))
    //    {
    //        GameManager.Instance.analytics.AddPlayer(((CFCAuth.AuthRequestMessage)connectionToClient.authenticationData).nftId,netIdentity);
    //        isDemo = false;
    //    }
    //    else
    //    {
    //        isDemo = true;
    //    }
    //}

    //TODO Suleman: Uncomment Later
    //public override void OnEnable()
    //{
    //    base.OnEnable();
    //    //TODO Suleman: Uncomment Later
    //    //pName = ((CFCAuth.AuthRequestMessage)connectionToClient.authenticationData).authUsername;
    //    //if (!((CFCAuth.AuthRequestMessage)connectionToClient.authenticationData).nftWallet.Equals("Demo"))
    //    //{
    //    //    GameManager.Instance.analytics.AddPlayer(((CFCAuth.AuthRequestMessage)connectionToClient.authenticationData).nftId, netIdentity);
    //    //    isDemo = false;
    //    //}
    //    //else
    //    //{
    //    //    isDemo = true;
    //    //}
    //}

    #region Commands
    //TODO Suleman: Uncomment Later
    //[ClientRpc]
    [PunRPC]
    void RpcDamage(int dealerViewID, float damage)
    {
        PhotonView dealerIdentity = PhotonView.Find(dealerViewID);

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

        // Commented for Photon
        //if (hasAuthority)
        //    CmdRemoveHealth(dealerIdentity, finalDamage);

        if (photonView.IsMine)
            photonView.RPC("CmdRemoveHealth", RpcTarget.All, dealerViewID, finalDamage);

    }

    //[TargetRpc]
    [PunRPC]
    public void TargetQuitMatch(/*NetworkConnection target*/)
    {
        if(photonView.IsMine)
        {
            QuitMatch();
        }
    }

    //[ClientRpc]
    [PunRPC]
    public void RpcQuitMatch()
    {
        QuitMatch();
    }

    public void QuitMatch()
    {
        //TODO Suleman: Uncomment Later
        //CFCNetworkManager.singleton.StopClient();
        PhotonNetwork.LeaveRoom();
        Debug.Log("Player has left the room");
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

    //TODO Suleman: Uncomment Later
    //[Command]
    [PunRPC]
    public void CmdRemoveHealth(int dealerViewID, float value)
    {
        try
        {
            PhotonView dealerIdentity = PhotonView.Find(dealerViewID);

            pHealth = Mathf.Clamp(pHealth-value, 0,100);
            ChangePlayerHealth(pHealth);
            GameManager.Instance.analytics.AddDamageDealt(dealerIdentity, value);
            GameManager.Instance.analytics.AddDamageReceived(photonView, value);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
    }

    //TODO Suleman: Uncomment Later
    //[Command]
    public void CmdAddHealth(float value)
    {
        pHealth = Mathf.Clamp(pHealth+value, 0,120);
    }

    //TODO Suleman: Uncomment Later
    //[Command]
    [PunRPC]
    public void CmdOnDamage(int dealerViewID, int targetViewID, float damage)
    {
        PhotonView dealerIdentity = PhotonView.Find(dealerViewID);
        PhotonView targetIdentity = PhotonView.Find(targetViewID);

        OnDamage(dealerIdentity, targetIdentity, damage);
    }
    
    public void OnDamage(PhotonView dealerIdentity, PhotonView targetIdentity, float damage)
    {
        if (GameManager.Instance.match.currentState != MatchManager.MatchState.InGame)
        { 
            damage = 0.0f; 
        }
        var target = targetIdentity.GetComponent<PlayerBehaviour>();
        var dealer = dealerIdentity.GetComponent<PlayerBehaviour>();
        Debug.Log($"CmdOnDamage: Receive {damage} damage on {target.pName} from {dealer.pName}");

        // Commented for Photon
        //if (!isServer) return;
        if (!photonView.IsMine) return;     //TODO Suleman: Could be PhotonNetwork.IsMasterClient also, need to confirm

        //TODO Suleman: Uncomment Later
        target.photonView.RPC("RpcDamage", RpcTarget.All, dealerIdentity.ViewID, damage);
    }

    //TODO Suleman: Uncomment Later
    //[Command]
    // Done the following in SetBoolIsBlocking()
    //public void CmdBlocking(bool blocking)
    //{
    //    pIsBlocking = blocking;
    //}

    //[Command]
    //void CmdCall(string callId)
    //{
    //    RpcCall(callId);
    //}

    //[ClientRpc]
    //void RpcCall(string callId)
    //{
    //    Debug.Log($"Receive call: {callId}");
    //    OnCall(callId);
    //}

    //[Command]
    //void CmdOnSendGlobalMessage(NetworkIdentity playerIdentity, Color32 color, string message)
    //{
    //    RpcMessageCreated(playerIdentity, color, message);
    //}

    //[ClientRpc]
    //void RpcMessageCreated(NetworkIdentity playerIdentity, Color32 color, string message)
    //{
    //    ChatGlobal_Manager.Instance.CreateMessage(playerIdentity, color, message);
    //}

    public void ResetServer()
    {
        //TODO Suleman: Uncomment Later
        //CmdResetServer();
        photonView.RPC("CmdResetServer", RpcTarget.AllBuffered);
    }
    //TODO Suleman: Uncomment Later
    //[Command]
    [PunRPC]
    private void CmdResetServer()
    {
        _ResetServer();
    }

    private void _ResetServer()
    {
        //TODO Suleman: Uncomment Later
        //CFCNetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
        //CFCNetworkManager.singleton.StopServer();
    }
    //TODO Suleman: Uncomment Later
    //[Command]
    [PunRPC]
    private void CmdOnDeath(int killerIdentityViewID)
    {
        PhotonView killerIdentity = PhotonView.Find(killerIdentityViewID);
        GameManager.Instance.analytics.AddKill(killerIdentity, photonView);
        GameManager.Instance.CheckWinner();
    }

    //[ClientRpc]
    [PunRPC]
    public void RpcChangePlayerPosition(Vector3 pos)
    {
        if(Vector3.Distance(transform.position, pos) > 0.2f)
            transform.position = pos;
    }

    //TODO Suleman: Uncomment Later
    //[TargetRpc]
    [PunRPC]
    public void TargetChangeSpectatorCamera(int deadViewID, int killerViewID)
    {
        PhotonView dead = PhotonView.Find(deadViewID);
        PhotonView killer = PhotonView.Find(killerViewID);

        if (spectating == null || spectating == dead)
        {
            if(dead == this)
            {
                spectating = killer;
                Camera_Manager.Instance.followCam.m_Follow = spectating.GetComponent<PlayerBehaviour>()._camTarget;
            }
        }
    }

    #region Pick and Throw
    //TODO Suleman: Uncomment Later, done
    //[Command]
    [PunRPC]
    public void CmdStealObject(int carrierID)
    {
        PhotonView carrier = PhotonView.Find(carrierID);
        var auxCarrier = carrier.GetComponent<PlayerBehaviour>();
        auxCarrier._tpFightingControler.StolenObject();

        photonView.RPC("RpcStealObject", RpcTarget.AllBuffered, carrier.ViewID);
    }

    //[ClientRpc]
    [PunRPC]
    public void RpcStealObject(int carrierID)
    {
        PhotonView carrier = PhotonView.Find(carrierID);
        var auxCarrier = carrier.GetComponent<PlayerBehaviour>();
        auxCarrier._tpFightingControler.StolenObject();
    }

    //[Command]
    [PunRPC]
    public void CmdAnimationPickUp(bool isPickUp)
    {
        //anim.SetLayerWeight(anim.GetLayerIndex("UpperBody"), isPickUp?1:0);
        //RpcAnimationPickUp(isPickUp);
        photonView.RPC("RpcAnimationPickUp", RpcTarget.All, isPickUp);
    }

    //[ClientRpc]
    [PunRPC]
    public void RpcAnimationPickUp(bool isPickUp)
    {
        anim.SetLayerWeight(anim.GetLayerIndex("UpperBody"), isPickUp ? 1 : 0);
    }

    //[Command]
    [PunRPC]
    public void CmdPickUp(/*NetworkIdentity*/ int itemID)
    {
        PhotonView item = PhotonView.Find(itemID);
        //TODO Suleman: Uncomment Later, done
        //item.RemoveClientAuthority();
        //item.AssignClientAuthority(photonView.connectionToClient);
       
        if (item.Owner.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            item.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
        }

        var pickUpObject = item.GetComponent<Throwable_BehaviorV2>();
        if (pickUpObject.PickUp(photonView, _tpFightingControler._throwTargetTransform))
        {
            _tpFightingControler._carryingObject = pickUpObject;

            //photonView.RPC("RpcPickUp", RpcTarget.AllBuffered, item.ViewID);
        }
        else
        {
            //RpcStealObject(photonView);
            photonView.RPC("RpcStealObject", RpcTarget.AllBuffered, photonView.ViewID);
        }

    }

    //[ClientRpc]
    //[PunRPC]
    //public void RpcPickUp(/*NetworkIdentity*/ int itemID)
    //{
    //    PhotonView item = PhotonView.Find(itemID);
    //    //TODO Suleman: Uncomment Later
    //    var pickUpObject = item.GetComponent<Throwable_BehaviorV2>();
    //    pickUpObject.PickUp(photonView, _tpFightingControler._throwTargetTransform);
    //    _tpFightingControler._carryingObject = pickUpObject;
    //    //pickUpObject.SetNoPhysics(true);
    //}

    //[Command]
    [PunRPC]
    public void CmdThrow(int itemID)
    {
        //if(!photonView.IsMine)
        //{
        //    return;
        //}
        PhotonView item = PhotonView.Find(itemID);
        var pickUpObject = item.GetComponent<Throwable_BehaviorV2>();
        pickUpObject.ResetChair();
        _tpFightingControler._carryingObject = null;

        //Commented for Photon
        //RpcThrow(item);
        //photonView.RPC("RpcThrow", RpcTarget.AllBuffered, item.ViewID);
    }

    //[ClientRpc]
    //[PunRPC]
    //public void RpcThrow(/*NetworkIdentity*/ int itemID)
    //{
    //    PhotonView item = PhotonView.Find(itemID);
    //    var pickUpObject = item.GetComponent<Throwable_BehaviorV2>();
    //    pickUpObject.ResetChair();
    //    _tpFightingControler._carryingObject = null;

    //}

    #endregion

    #endregion




    [PunRPC]
    private void SyncMatchState(int newState)
    {
        GameManager.Instance.currentMatchState = newState;
    }

    [PunRPC]
    public void RpcSetupStartGameTimer(int time)
    {
        GameManager.Instance.timerBehavior.SetupTimer("Game begins in", time);
    }

    [PunRPC]
    public void RpcSetupTimeOutTimer(int time)
    {
        GameManager.Instance.timerBehavior.SetupTimer("Game will start in ", time);
    }

    [PunRPC]
    public void RpcSetupEndingTimer(int time)
    {
        GameManager.Instance.timerBehavior.SetupTimer("Leaving in", time);
    }
    
    [PunRPC]
    public void RpcUpdateTimer(int time)
    {
        GameManager.Instance.timerBehavior.UpdateTimer(time);
    }

    [PunRPC]
    public void RpcStopTimeOutTimer()
    {
        GameManager.Instance.timerBehavior.StopTimer();
    }

    [PunRPC]
    public void RpcRequestStartSession()
    {
        gameplayView.instance.RequestStartSession();
    }

    [PunRPC]
    public void RpcDraw()
    {
        MenuManager.Instance.ShowKO();
    }

    [PunRPC]
    public void RpcEndGameTimeout(int time)
    {
        GameManager.Instance.gameTimeoutTimerBehavior.UpdateTimer(time);
    }

    [PunRPC]
    public void TargetSendUpdatedScore(/*NetworkConnection con,*/ int score)
    {
        if (photonView.IsMine)
        {
            IngameUIControler.instance.UpdateScore(score);
        }
    }

    [PunRPC]
    public void RpcPreGameState()
    {
        GameManager.Instance.match.ChangeState(MatchManager.MatchState.PreGame);
    }

    [PunRPC]
    public void TargetRequestEndtSession(/*NetworkConnection con , */int score, int kills)
    {
        gameplayView.instance.RequestEndSession(score, kills);
    }

    //[PunRPC]
    //public void RpcResetPosition(int itemViewID)
    //{
    //    PhotonView item = PhotonView.Find(itemViewID);
    //    item.GetComponent<Throwable_BehaviorV2>().ResetPosition();
    //}

    //[PunRPC]
    //public void RpcResetCollision(int carrierID)
    //{
    //    Debug.Log("Pick and Throw -> RpcResetCollision -> PlayerBehaviour");

    //    PhotonView carrier = PhotonView.Find(carrierID);
    //    Debug.Log("Carrier: " + carrier);
    //    var lastCarrier = carrier.GetComponent<PlayerBehaviour>();
    //    Debug.Log("lastCarrier: " + lastCarrier);
    //    carrier.GetComponent<Throwable_BehaviorV2>().ResetCollision(lastCarrier);
    //}

    [PunRPC]
    public void RpcThrowItem(int itemID)
    {
        PhotonView item = PhotonView.Find(itemID);

        item.GetComponent<Throwable_BehaviorV2>().Throw();

        //SetNoPhysics(false);

        //if (carrierNetIdentity != null)
        //{
        //    var force = (carrierNetIdentity.transform.forward + (Vector3.up / 10)) * throwForce;
        //    rb.AddForce(force, ForceMode.Force);
        //}

        //carrierTarget = null;
    }

}
