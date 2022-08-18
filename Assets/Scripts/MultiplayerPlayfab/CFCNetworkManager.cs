using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using Mirror.Websocket;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CFCNetworkManager : NetworkManager
{
    public static CFCNetworkManager Instance;

    //public AgoraHome agora;

    public CinemachineVirtualCamera dollyCam;
    public CinemachineVirtualCamera followCam;

    public GameManager _gameManager;
    public TimerManager timer;

    public int connectedPlayersCount =>
        NetworkServer.connections.Count;

    public override void Awake()
    {
        base.Awake();
        if (Instance == null) Instance = this;
        
    }

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<CharacterCustomizationMsg>(OnCreateCharacter);
        Connection_Manager.Instance.Api_PlayfabMatchmaking.CreateDataTitle();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        try
        {
            PlayerBehaviour.playerNames.Remove(((CFCAuth.AuthRequestMessage)conn.authenticationData).authUsername);
            _gameManager.analytics.RemovePlayer(conn.identity);
            //_gameManager.CheckWinner();

            base.OnServerDisconnect(conn);

            Debug.Log(_gameManager.match.currentState);
            if (_gameManager.match.currentState == MatchManager.MatchState.InGame)
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
                singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
                singleton.StopServer();
            }
        }
        catch (Exception e)
        {
            Debug.Log("CFCNetworkManager - OnServerDisconnect() -> " + e);
        }
        
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        _gameManager.OnClientConnect(conn.identity);

    }
    
    #endregion

    #region Client

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);


        // you can send the message here, or wherever else you want
        CharacterCustomizationMsg characterMessage = new CharacterCustomizationMsg()
        {
            skinName = Character_Manager.Instance.GetCurrentCharacter.Name
        };

        dollyCam.Priority = 0;
        followCam.Priority = 10;
        NetworkClient.Send(characterMessage);
        MenuManager.Instance.ShowTutorial();
        MenuManager.Instance.ShowLeaderboardsAllTime();
        MenuManager.Instance.ShowLeaderboardsDaily();
        timer.CloseAreaManager.StartTimer();

    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        dollyCam.Priority = 0;
        followCam.Priority = 10;

        ModalWarning.Instance.Show("Client disconnected, retry?", MenuManager.Instance.Reset);
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
