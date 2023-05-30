using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using Mirror.Websocket;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CFCNetworkManager : MonoBehaviourPunCallbacks
{
    public static CFCNetworkManager Instance;

    //public AgoraHome agora;

    public CinemachineVirtualCamera dollyCam;
    public CinemachineVirtualCamera followCam;

    public GameManager _gameManager;
    public TimerManager timer;

    public int connectedPlayersCount => PhotonNetwork.CurrentRoom.PlayerCount;//NetworkServer.connections.Count;
    public int maxConnections = 5;

    [SerializeField] private GameObject player;


    public /*override*/ void Awake()
    {
        //base.Awake();
        if (Instance == null) Instance = this;
    }

    // TODO Suleman: Added for Photon
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);

        // TODO Suleman: Copied the following here from OnJoinedRoom() because it never got called
        CharacterCustomizationMsg characterMessage = new CharacterCustomizationMsg()
        {
            skinName = Character_Manager.Instance.GetCurrentCharacter.Name
        };

        dollyCam.Priority = 0;
        followCam.Priority = 10;
        //TODO Suleman: Uncomment Later
        //NetworkClient.Send(characterMessage);
        //MenuManager.Instance.ShowTutorial();
        //MenuManager.Instance.ShowLeaderboardsAllTime();
        //MenuManager.Instance.ShowLeaderboardsDaily();
        timer.CloseAreaManager.StartTimer();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    //public override void OnConnectedToMaster()
    //{
    //    base.OnConnectedToMaster();
    //    PhotonNetwork.JoinRandomRoom();
    //}

    #region Server
    //TODO Suleman: Uncomment Later
    //public override void OnStartServer()
    //{
    //    base.OnStartServer();
    //    NetworkServer.RegisterHandler<CharacterCustomizationMsg>(OnCreateCharacter);
    //    Connection_Manager.Instance.Api_PlayfabMatchmaking.CreateDataTitle();
    //}


    private void TryUpdateLobbyPlayer(int ConnectedPlayersCount)
    {
        try
        {
            var dic = PlayFabMultiplayerAgentAPI.GetConfigSettings();
            if (dic != null)
            {
                
                string _serverId = dic[PlayFabMultiplayerAgentAPI.ServerIdKey];
                GameManager.Instance.UpdateLobbyPlayer(_serverId, ConnectedPlayersCount);
            }
        }
        catch (Exception)
        {
            Debug.Log("Try GetConfigSettings");
        }

    }

    //TODO Suleman: Uncomment Later
    // Never gets called
    public override void OnLeftRoom() /*OnServerDisconnect(NetworkConnection conn)*/
    {
        var diffConnectedPlayersCount = connectedPlayersCount - 1;
        if (diffConnectedPlayersCount < 0) { diffConnectedPlayersCount = 0; }
        TryUpdateLobbyPlayer(diffConnectedPlayersCount);

        try
        {

            //Commented for Photon
            //if (((CFCAuth.AuthRequestMessage)conn.authenticationData).nftWallet.Equals("Demo"))
            //{
            //    Debug.Log("Disconnect as Demo");
            //    base.OnServerDisconnect(conn);
            //    return;
            //}

            //Commented for Photon
            //PlayerBehaviour.playerNames.Remove(((CFCAuth.AuthRequestMessage)conn.authenticationData).authUsername);
            _gameManager.analytics.RemovePlayer(photonView/*conn.identity*/);
            //_gameManager.CheckWinner();

            //Commented for Photon
            //base.OnServerDisconnect(conn);

            Debug.Log(_gameManager.match.currentState);
            if (_gameManager.match.currentState == MatchManager.MatchState.InGame ||
                _gameManager.match.currentState == MatchManager.MatchState.PreGame ||
                _gameManager.match.currentState == MatchManager.MatchState.Lobby)
            {
                _gameManager.OnClientDisconnect();
            }

            if (//_gameManager.match.currentState != MatchManager.MatchState.Lobby &&
                NetworkServer.connections.Count <= 0)
            {
                Connection_Manager.Instance.SendOpenLobby();

                //singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
                //singleton.StopServer();
                Debug.Log("entrou");
                //Commented for Photon
                //singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
                //singleton.StopServer();
            }


        }
        catch (Exception e)
        {
            Debug.Log("CFCNetworkManager - OnServerDisconnect() -> " + e);
        }

    }

    //TODO Suleman: Uncomment Later
    //public override void OnServerAddPlayer(NetworkConnection conn)
    //{
    //    base.OnServerAddPlayer(conn);
    //    Debug.Log("OnServerAddPlayer");

    //    if (!((CFCAuth.AuthRequestMessage) conn.authenticationData).nftWallet.Equals("Demo"))
    //    {
    // called the following in ClientStartUp under function SpawnPlayer()
    //        _gameManager.OnClientConnect(conn.identity);
    //    }

    //       TryUpdateLobbyPlayer(connectedPlayersCount);
    //}

    #endregion

    #region Client

    public override void OnJoinedRoom() //OnClientConnect(NetworkConnection conn)
    {
        //base.OnClientConnect(conn);
        base.OnJoinedRoom();

        // you can send the message here, or wherever else you want
        CharacterCustomizationMsg characterMessage = new CharacterCustomizationMsg()
        {
            skinName = Character_Manager.Instance.GetCurrentCharacter.Name
        };

        dollyCam.Priority = 0;
        followCam.Priority = 10;
        //TODO Suleman: Uncomment Later
        //NetworkClient.Send(characterMessage);
        MenuManager.Instance.ShowTutorial();
        MenuManager.Instance.ShowLeaderboardsAllTime();
        MenuManager.Instance.ShowLeaderboardsDaily();
        timer.CloseAreaManager.StartTimer();

    }

    public override void OnDisconnected(DisconnectCause cause) //OnClientDisconnect(NetworkConnection conn)
    {
        //base.OnClientDisconnect(conn);
        base.OnDisconnected(cause);
        Debug.Log("Disconnected: " + cause.ToString());
        dollyCam.Priority = 0;
        followCam.Priority = 10;
        Debug.Log("Disconnect -> OnDisconnected()");

        ModalWarning.Instance.Show("Client disconnected, retry?", MenuManager.Instance.Reset);
        _gameManager.OnClientDisconnect();
    }

    public void OnCreateCharacter(NetworkConnection conn, CharacterCustomizationMsg message)
    {
        // Apply data from the message however appropriate for your game
        // Typically Player would be a component you write with syncvars or properties
        PlayerBehaviour player = conn.identity.GetComponent<PlayerBehaviour>();
        player.pSkin = message.skinName;
    }
    #endregion
}
