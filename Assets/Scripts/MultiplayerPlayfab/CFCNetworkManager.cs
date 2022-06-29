using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;

public class CFCNetworkManager : NetworkManager
{
    public static CFCNetworkManager Instance;

    //public AgoraHome agora;

    public CinemachineVirtualCamera dollyCam;
    public CinemachineVirtualCamera followCam;

    public TimerManager timer;

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
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        PlayerBehaviour.playerNames.Remove((string)conn.authenticationData);

        base.OnServerDisconnect(conn);
    }

    #endregion

    #region Client

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        // you can send the message here, or wherever else you want
        CharacterCustomizationMsg characterMessage = new CharacterCustomizationMsg()
        {
            skinName = Character_Manager.Instance.GetCurrentCharacter.name
        };

        dollyCam.Priority = 0;
        followCam.Priority = 10;

        NetworkClient.Send(characterMessage);
        MenuManager.Instance.ShowTutorial();
        timer.deadlyAreaManager.StartDeadly();
        //timer.StartTime();
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
