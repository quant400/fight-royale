using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mirror;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance;

    [Header("Variables")]
    public int timeOutTime = 60;
    public int preGameTime = 5;
    public int inGameTime = 600;
    public int postGameTime = 5;

    public int minPlayersToPlay = 2;

    public bool isEndingGameTime = false;

    [Header("Components")]
    public MatchManager match;
    public TimerManager timer;
    public CloseAreaManager closeArea;
    public TimerBehavior timerBehavior;
    public TimerBehavior gameTimeoutTimerBehavior;
    public AnalyticsManager analytics;
    public LevelManager level;
    public PowerUpManager powerUp;

    // Commented for Photon
    /*[SyncVar]*/
    public int currentMatchState;

    [HideInInspector] public PhotonView player;
    public Dictionary<NetworkIdentity, string[]> wearablesWorn = new Dictionary<NetworkIdentity, string[]>();


    void Awake()
    {
        Debug.Log("GameManager instance already created");
        if (Instance == null) Instance = this;
        //if (!isServer) return;

        Subscribe();

        //GameObject GMObj = GameManager.Instance.gameObject;
        ActivateObjects(this.gameObject);
    }

    private void ActivateObjects(GameObject gm)
    {
        gm.SetActive(true);

        foreach (Transform child in gm.transform)
        {
            ActivateObjects(child.gameObject);
        }
    }

    public void Subscribe()
    {
        //Base
        match.onChangeState.AddListener(/*(aux) => currentMatchState = aux*/ (aux) => SetMatchState(aux));
        
        //PreGame
        match.onPreGameState.AddListener(PreGame);
        match.onPreGameState.AddListener(analytics.InitScore);

        match.onInGameState.AddListener(StartGameTimeout);

        //PostGame
        match.onPostGameState.AddListener(PostGame);
        match.onPostGameState.AddListener(() => Invoke("SendScore", 2f));

    }

    public void SetMatchState(int newState)
    {
        currentMatchState = newState;
        player.RPC("SyncMatchState", RpcTarget.AllBuffered, newState);
    }

    // Transferred PUN RPC to PlayerBehaviour.cs
    //[PunRPC]
    //private void SyncMatchState(int newState)
    //{
    //    currentMatchState = newState;
    //}
    // End Transfer

    public void SendScore()
    {
        analytics.SendScore();
    }

    public void OnClientConnect(PhotonView netId)
    {
        player = netId;
        //if (/*isServer*/PhotonNetwork.IsMasterClient)
        //{
        //    match.OnPlayersChanges();
        //}
        match.OnPlayersChanges();
    }
    
    public void OnClientDisconnect()
    {
        if (match.currentState == MatchManager.MatchState.InGame)
        {
            CheckWinner();
        }
        else
        {
            match.OnPlayersChanges();
        }

        // Commented for Photon
        //if (/*isServer*/PhotonNetwork.IsMasterClient)
        //{
        //    if (match.currentState == MatchManager.MatchState.InGame)
        //    {
        //        //TODO Suleman: Uncomment Later
        //        CheckWinner();
        //    }
        //    else 
        //    {
        //        match.OnPlayersChanges();
        //    }

        //}
    }
    
    #region Game

    public void PreGame()
    {
        CloseLobby();
    }


    public void UpdateLobbyPlayer(string serverId, int countPlayer)
    {
        try
        {
            Debug.Log(" Try Update Lobby Player");
            if (!string.IsNullOrEmpty(serverId))
            {
                Connection_Manager.Instance.Api_PlayfabMatchmaking.SetPlayersOnServer(serverId, countPlayer);
            }

        }
        catch (System.Exception e)
        {
            Debug.Log("UpdateLobbyPlayer Error - " + e.Message);
        }

    }

    public void CloseLobby()
    {
        KickDemoPlayers();
        Connection_Manager.Instance.SendCloseLobby();
        
        // Closing current room so that new players cannot join this room
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;

        //TODO Suleman: Uncomment Later, done
        player.RPC("RpcSetupStartGameTimer", RpcTarget.AllBuffered, preGameTime);
        MrTime_Manager.Instance.SetTimer(preGameTime, UpdatePreGameTimer, StartGame);
    }

    public void KickDemoPlayers()
    {
        var demoPlayers = FindObjectsOfType<PlayerBehaviour>();
        foreach (var player in demoPlayers.Where(aux => aux.isDemo))
        {
            //TODO Suleman: Uncomment Later
            //player.TargetQuitMatch(/*player.connectionToClient*/);
            if(player.photonView.IsMine)
            {
                player.photonView.RPC("TargetQuitMatch", RpcTarget.All);
            }
        }
    }

    private void UpdatePreGameTimer(int time)
    {
        //level.ResetPlayers();
        player.RPC("RpcUpdateTimer", RpcTarget.AllBuffered, time);
        Debug.Log("Começando em: "+time);
    }

    // Transferred PUN RPC to PlayerBehaviour.cs
    //[ClientRpc]
    //[PunRPC]
    //public void RpcSetupStartGameTimer(int time)
    //{
    //    timerBehavior.SetupTimer("Game begins in", time);
    //}

    ////[ClientRpc]
    //[PunRPC]
    //public void RpcSetupTimeOutTimer(int time)
    //{
    //    timerBehavior.SetupTimer("Game will start in ", time);
    //}

    ////[ClientRpc]
    //[PunRPC]
    //public void RpcSetupEndingTimer(int time)
    //{
    //    timerBehavior.SetupTimer("Leaving in", time);
    //}

    ////[ClientRpc]
    //[PunRPC]
    //public void RpcUpdateTimer(int time)
    //{
    //    timerBehavior.UpdateTimer(time);
    //}

    ////[ClientRpc]
    //[PunRPC]
    //public void RpcStopTimeOutTimer()
    //{
    //    timerBehavior.StopTimer();
    //}

    ////[ClientRpc]
    ////TODO Suleman: Check if this is getting called
    //[PunRPC]
    //public void RpcRequestStartSession()
    //{
    //    gameplayView.instance.RequestStartSession();
    //}
    // End Transfer

    private void StartGame()
    {
        level.ResetPlayers();
        //Resetar tudo
        match.ChangeState(MatchManager.MatchState.InGame);
        //timer.StartTime();
    }

    private void StartGameTimeout()
    {
        isEndingGameTime = false;
        MrTime_Manager.Instance.SetTimer(inGameTime, UpdateGameTimeout, GameTimeout);
    }

    private void GameTimeout()
    {
        //todo: tela de empate
        //TODO Suleman: Uncomment Later
        player.RPC("RpcDraw", RpcTarget.AllBuffered);
        match.ChangeState(MatchManager.MatchState.PostGame);
    }

    // Transferred PUN RPC to PlayerBehaviour.cs
    //[ClientRpc]
    //[PunRPC]
    //public void RpcDraw()
    //{
    //    MenuManager.Instance.ShowKO();
    //}
    //// End Transfer

    private void UpdateGameTimeout(int time)
    { /*
        if (time < 60 && !isEndingGameTime)
        {
            RpcSetupGameTimeout(inGameTime);
            isEndingGameTime = true;
        }
        */
        //TODO Suleman: Uncomment Later
        player.RPC("RpcEndGameTimeout", RpcTarget.AllBuffered, time);
    }
    //TODO Suleman: Not being used
    //[ClientRpc]
    //public void RpcSetupGameTimeout(int time)
    //{
    //    gameTimeoutTimerBehavior.SetupTimer("Game ending in", time);
    //}

    // Transferred PUN RPC to PlayerBehaviour.cs
    //[ClientRpc]
    //[PunRPC]
    //public void RpcEndGameTimeout(int time)
    //{
    //    gameTimeoutTimerBehavior.UpdateTimer(time);
    //}
    // End Transfer

    // Transferred PUN RPC to PlayerBehaviour.cs
    //[TargetRpc]
    //public void TargetSendUpdatedScore(/*NetworkConnection con,*/int score)
    //{
    //    IngameUIControler.instance.UpdateScore(score);
    //}
    // End Transfer

   /* public void AddWearable(NetworkIdentity netId, string[] vals)
    {
        Debug.Log(10);
        wearablesWorn.Add(netId, vals);
        Debug.Log((netId,wearablesWorn[netId][0]));
       //RpcGetWearablesWorn(netId, vals);
        Debug.Log(30);
    }
  
    public void GetWormWearables(NetworkIdentity netId)
    {
        Debug.Log(300);
        RpcSendWearables(netId, wearablesWorn[netId]);
    }
    [Command]
    public void CmdRemoveWearables(NetworkIdentity netId)
    {
        wearablesWorn.Remove(netId);
        RpcRemoveWorn(netId);
    }
    [Command]
    public void CmdClearWearbles()
    {
        wearablesWorn.Clear();
    }
    [ClientRpc]
    public void RpcSendWearables(NetworkIdentity con, string[] wearables)
    {
        Debug.Log(350);
        Debug.Log((con, wearables));
        if(!wearablesWorn.ContainsKey(con))
            wearablesWorn.Add(con, wearables);

    }
  [ClientRpc]
    public void RpcGetWearablesWorn(NetworkIdentity netId,string[]vals)
    {
        Debug.Log(20);
        wearablesWorn.Add(netId, vals);
        Debug.Log(wearablesWorn[netId][0]);
    }
    [ClientRpc]
    public void RpcRemoveWorn(NetworkIdentity netId)
    {
        wearablesWorn.Remove(netId);
        Debug.Log(wearablesWorn[netId]);
    }*/
   
    #endregion

    #region Ending

    public void CheckWinner()
    {
        if (match.currentState != MatchManager.MatchState.InGame) return;

        if (analytics.GetNumberOfPlayersAlive() <= 1)
        {
            //PostGame();
            match.ChangeState(MatchManager.MatchState.PostGame);
        }
    }

    private void PostGame()
    {
        //STOP GAME
        try
        {
            var winner = analytics.GetWinner();
            winner.gameObject.GetComponent<PlayerBehaviour>().photonView.RPC("RpcWin", RpcTarget.AllBuffered);/* RpcWin();*/
            //TODO Suleman: Uncomment Later
            player.RPC("RpcSetupEndingTimer", RpcTarget.AllBuffered, postGameTime);
            MrTime_Manager.Instance.SetTimer(postGameTime, UpdatePostGameTimer, EndMatch);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Exception PostGame " + ex);
        }
       
    }
    //TODO Suleman: Uncomment Later
    //[TargetRpc]
    // Transferred PUN RPC to PlayerBehaviour.cs
    //[PunRPC]
    //public void TargetRequestEndtSession(/*NetworkConnection con , */int score, int kills)
    //{
    //    gameplayView.instance.RequestEndSession(score, kills);
    //}
    // End Transfer

    private void EndMatch()
    {
        Connection_Manager.Instance.SendOpenLobby();

        var playersIds = new List<int>();

        //Commented for Photon
        //foreach (var conn in NetworkServer.connections)
        //{
        //    Debug.Log($"{conn.Key}: {conn.Value}");
        //    playersIds.Add(conn.Key);
        //    //conn.Value.identity.gameObject.GetComponent<PlayerBehaviour>().RpcQuitMatch();
        //}

        //for (int i = 0; i < playersIds.Count; i++)
        //{
        //    Debug.Log(NetworkServer.connections[playersIds[i]].identity.gameObject.GetComponent<PlayerBehaviour>().pName);
        //    //TODO Suleman: Uncomment Later
        //    NetworkServer.connections[playersIds[i]].identity.gameObject.GetComponent<PlayerBehaviour>().RpcQuitMatch();
        //}

        foreach (var player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"{player.ActorNumber}: {player.NickName}");
            playersIds.Add(player.ActorNumber);
            //conn.Value.identity.gameObject.GetComponent<PlayerBehaviour>().RpcQuitMatch();
        }

        for (int i = 0; i < playersIds.Count; i++)
        {
            //Debug.Log(NetworkServer.connections[playersIds[i]].identity.gameObject.GetComponent<PlayerBehaviour>().pName);
            //TODO Suleman: Uncomment Later
            player.RPC("RpcQuitMatch", RpcTarget.All);
        }

        //CFCNetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
    }

    private void UpdatePostGameTimer(int time)
    {
        //TODO Suleman: Uncomment Later
        player.RPC("RpcUpdateTimer", RpcTarget.AllBuffered, time);
        Debug.Log("Finalizando em: "+time);
    }

    #endregion

}
