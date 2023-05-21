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


    void Awake()
    {
        Debug.Log("GameManager instance already created");
        if (Instance == null) Instance = this;
        //if (!isServer) return;
        Debug.Log("GameManager instance created");
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
        match.onPostGameState.AddListener(()=>Invoke("SendScore", 2f));

    }

    public void SetMatchState(int newState)
    {
        currentMatchState = newState;
        player.RPC("SyncMatchState", RpcTarget.All, newState);
    }

    // Transferred PUN RPC to PlayerBehaviour.cs
    [PunRPC]
    private void SyncMatchState(int newState)
    {
        currentMatchState = newState;
    }
    // End Transfer

    public void SendScore()
    {
        analytics.SendScore();
    }

    public void OnClientConnect(PhotonView netId)
    {
        Debug.Log("GameManager - OnClientConnect()");

        player = netId;
        if (/*isServer*/PhotonNetwork.IsMasterClient)
        {
            match.OnPlayersChanges();
        }
        match.OnPlayersChanges();
    }
    
    public void OnClientDisconnect()
    {
        if (/*isServer*/PhotonNetwork.IsMasterClient)
        {
            if (match.currentState == MatchManager.MatchState.InGame)
            {
                //TODO Suleman: Uncomment Later
                CheckWinner();
            }
            else 
            {
                match.OnPlayersChanges();
            }

        }
    }
    
    #region Game
    
    public void PreGame()
    {
        CloseLobby();
    }
    

    public void UpdateLobbyPlayer(string serverId,int countPlayer)
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
        //TODO Suleman: Uncomment Later
        player.RPC("RpcSetupStartGameTimer", RpcTarget.All, preGameTime);
        MrTime_Manager.Instance.SetTimer(preGameTime, UpdatePreGameTimer, StartGame);
    }

    public void KickDemoPlayers()
    {
        var demoPlayers = FindObjectsOfType<PlayerBehaviour>();
        foreach (var player in demoPlayers.Where(aux=> aux.isDemo))
        {
            //TODO Suleman: Uncomment Later
            //player.TargetQuitMatch(/*player.connectionToClient*/);
        }
    }

    private void UpdatePreGameTimer(int time)
    {
        level.ResetPlayers();
        //TODO Suleman: Uncomment Later
        player.RPC("RpcUpdateTimer", RpcTarget.All, time);
        Debug.Log("Começando em: "+time);
    }

    // Transferred PUN RPC to PlayerBehaviour.cs
    //TODO Suleman: Uncomment Later
    //[ClientRpc]
    [PunRPC]
    public void RpcSetupStartGameTimer(int time)
    {
        timerBehavior.SetupTimer("Game begins in", time);
    }

    //[ClientRpc]
    [PunRPC]
    public void RpcSetupTimeOutTimer(int time)
    {
        timerBehavior.SetupTimer("Game will start in ", time);
    }

    //[ClientRpc]
    [PunRPC]
    public void RpcSetupEndingTimer(int time)
    {
        timerBehavior.SetupTimer("Leaving in", time);
    }

    //[ClientRpc]
    [PunRPC]
    public void RpcUpdateTimer(int time)
    {
        timerBehavior.UpdateTimer(time);
    }

    //[ClientRpc]
    [PunRPC]
    public void RpcStopTimeOutTimer()
    {
        timerBehavior.StopTimer();
    }

    //[ClientRpc]
    //TODO Suleman: Check if this is getting called
    [PunRPC]
    public void RpcRequestStartSession()
    {
        gameplayView.instance.RequestStartSession();
    }
    // End Transfer

    private void StartGame()
    {
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
        player.RPC("RpcDraw", RpcTarget.All);
        match.ChangeState(MatchManager.MatchState.PostGame);
    }

    // Transferred PUN RPC to PlayerBehaviour.cs
    //TODO Suleman: Uncomment Later
    //[ClientRpc]
    [PunRPC]
    public void RpcDraw()
    {
        MenuManager.Instance.ShowKO();
    }
    // End Transfer

    private void UpdateGameTimeout(int time)
    { /*
        if (time < 60 && !isEndingGameTime)
        {
            RpcSetupGameTimeout(inGameTime);
            isEndingGameTime = true;
        }
        */
        //TODO Suleman: Uncomment Later
        player.RPC("RpcEndGameTimeout", RpcTarget.All, time);
    }
    //TODO Suleman: Uncomment Later
    //[ClientRpc]
    //public void RpcSetupGameTimeout(int time)
    //{
    //    gameTimeoutTimerBehavior.SetupTimer("Game ending in", time);
    //}

    // Transferred PUN RPC to PlayerBehaviour.cs
    //[ClientRpc]
    [PunRPC]
    public void RpcEndGameTimeout(int time)
    {
        gameTimeoutTimerBehavior.UpdateTimer(time);
    }
    // End Transfer

    //[TargetRpc]
    //public void TargetSendUpdatedScore(/*NetworkConnection con,*/int score)
    //{
    //    IngameUIControler.instance.UpdateScore(score);
    //}
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
            winner.gameObject.GetComponent<PlayerBehaviour>().photonView.RPC("RpcWin", RpcTarget.All);/* RpcWin();*/
            //TODO Suleman: Uncomment Later
            player.RPC("RpcSetupEndingTimer", RpcTarget.All, postGameTime);
            MrTime_Manager.Instance.SetTimer(postGameTime, UpdatePostGameTimer, EndMatch);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Exception PostGame " + ex);
        }
       
    }
    //TODO Suleman: Uncomment Later
    //[TargetRpc]
    //public void TargetRequestEndtSession(/*NetworkConnection con , */int score, int kills)
    //{
    //    gameplayView.instance.RequestEndSession(score,kills);
    //}

    private void EndMatch()
    {
        Connection_Manager.Instance.SendOpenLobby();

        var playersIds = new List<int>();
        
        foreach (var conn in NetworkServer.connections)
        {
            Debug.Log($"{conn.Key}: {conn.Value}");
            playersIds.Add(conn.Key);
            //conn.Value.identity.gameObject.GetComponent<PlayerBehaviour>().RpcQuitMatch();
        }

        for (int i = 0; i < playersIds.Count; i++)
        {
            Debug.Log(NetworkServer.connections[playersIds[i]].identity.gameObject.GetComponent<PlayerBehaviour>().pName);
            //TODO Suleman: Uncomment Later
            //NetworkServer.connections[playersIds[i]].identity.gameObject.GetComponent<PlayerBehaviour>().RpcQuitMatch();
        }

        //CFCNetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
    }

    private void UpdatePostGameTimer(int time)
    {
        //TODO Suleman: Uncomment Later
        player.RPC("RpcUpdateTimer", RpcTarget.All, time);
        Debug.Log("Finalizando em: "+time);
    }

    #endregion

}
