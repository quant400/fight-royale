using System.Collections;
using System.Collections.Generic;
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
    public int postGameTime = 5;
    
    public int minPlayersToPlay = 2;
    
    [Header("Components")] 
    public MatchManager match;
    public TimerManager timer;
    public CloseAreaManager closeArea;
    public TimerBehavior timerBehavior;
    public AnalyticsManager analytics;
    public LevelManager level;

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
            match.OnPlayersChanges();
            CheckWinner();
        }
    }
    
    #region Game
    
    public void PreGame()
    {
        CloseLobby();
    }
    
    public void CloseLobby()
    {
        Connection_Manager.Instance.SendCloseLobby();
        RpcSetupStartGameTimer(preGameTime);
        MrTime_Manager.Instance.SetTimer(preGameTime, UpdatePreGameTimer, StartGame);
    }
    
    private void UpdatePreGameTimer(int time)
    {
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
        timer.StartTime();
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
        var winner = analytics.GetWinner();
        winner.gameObject.GetComponent<PlayerBehaviour>().RpcWin();

        RpcSetupEndingTimer(postGameTime);
        MrTime_Manager.Instance.SetTimer(postGameTime, UpdatePostGameTimer, EndMatch);
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
