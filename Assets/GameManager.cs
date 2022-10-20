using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
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

    [SyncVar] public int currentMatchState;


    void Awake()
    {
        if (Instance == null) Instance = this;
        //if (!isServer) return;
        
        Subscribe();
    }

    public void Subscribe()
    {
        //Base
        match.onChangeState.AddListener((aux) => currentMatchState = aux);
        
        //PreGame
        match.onPreGameState.AddListener(PreGame);
        match.onPreGameState.AddListener(analytics.InitScore);
        
        match.onInGameState.AddListener(StartGameTimeout);

        //PostGame
        match.onPostGameState.AddListener(PostGame);
        match.onPostGameState.AddListener(()=>Invoke("SendScore", 2f));

    }

    public void SendScore()
    {
        analytics.SendScore();
    }

    public void OnClientConnect(NetworkIdentity netId)
    {
        if (isServer)
        {
            match.OnPlayersChanges();
        }
    }
    
    public void OnClientDisconnect()
    {
        if (isServer)
        {
            if (match.currentState == MatchManager.MatchState.InGame)
            {
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
        RpcSetupStartGameTimer(preGameTime);
        MrTime_Manager.Instance.SetTimer(preGameTime, UpdatePreGameTimer, StartGame);
    }

    public void KickDemoPlayers()
    {
        var demoPlayers = FindObjectsOfType<PlayerBehaviour>();
        foreach (var player in demoPlayers.Where(aux=> aux.isDemo))
        {
            player.TargetQuitMatch(player.connectionToClient);
        }
    }

    private void UpdatePreGameTimer(int time)
    {
        level.ResetPlayers();
        RpcUpdateTimer(time);
        Debug.Log("Começando em: "+time);
    }

    [ClientRpc]
    public void RpcSetupStartGameTimer(int time)
    {
        timerBehavior.SetupTimer("Game begins in", time);
    }
    
    [ClientRpc]
    public void RpcSetupTimeOutTimer(int time)
    {
        timerBehavior.SetupTimer("Wait players to start game in", time);
    }
    
    [ClientRpc]
    public void RpcSetupEndingTimer(int time)
    {
        timerBehavior.SetupTimer("Leaving in", time);
    }

    [ClientRpc]
    public void RpcUpdateTimer(int time)
    {
        timerBehavior.UpdateTimer(time);
    }
    
    [ClientRpc]
    public void RpcStopTimeOutTimer()
    {
        timerBehavior.StopTimer();
    }

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
        RpcDraw();
        match.ChangeState(MatchManager.MatchState.PostGame);
    }

    [ClientRpc]
    public void RpcDraw()
    {
        MenuManager.Instance.ShowKO();
    }

    private void UpdateGameTimeout(int time)
    {
        if (time < 60 && !isEndingGameTime)
        {
            RpcSetupGameTimeout(inGameTime);
            isEndingGameTime = true;
        }

        RpcEndGameTimeout(time);
    }
    
    [ClientRpc]
    public void RpcSetupGameTimeout(int time)
    {
        gameTimeoutTimerBehavior.SetupTimer("Game ending in", time);
    }

    [ClientRpc]
    public void RpcEndGameTimeout(int time)
    {
        gameTimeoutTimerBehavior.UpdateTimer(time);
    }

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
            winner.gameObject.GetComponent<PlayerBehaviour>().RpcWin();
            RpcSetupEndingTimer(postGameTime);
            MrTime_Manager.Instance.SetTimer(postGameTime, UpdatePostGameTimer, EndMatch);
        }
        catch (System.Exception ex)
        {
            Debug.Log("Exception PostGame " + ex);
        }
       
    }

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
            NetworkServer.connections[playersIds[i]].identity.gameObject.GetComponent<PlayerBehaviour>().RpcQuitMatch();
        }
        
        //CFCNetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
    }

    private void UpdatePostGameTimer(int time)
    {
        RpcUpdateTimer(time);
        Debug.Log("Finalizando em: "+time);
    }

    #endregion

}
