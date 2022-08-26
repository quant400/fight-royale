using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MatchManager : NetworkBehaviour
{
    public enum MatchState
    {
        Lobby = 0,
        PreGame = 1,
        InGame = 2,
        PostGame = 3
    }
    
    public MatchState currentState;

    [SerializeField]private GameManager _gameManager;
    [SerializeField]private MrTime_Manager _timer => MrTime_Manager.Instance;

    public UnityEvent onLobbyState;
    public UnityEvent onPreGameState;
    public UnityEvent onInGameState;
    public UnityEvent onPostGameState;
    public UnityEvent<int> onChangeState;

    public void ChangeState(MatchState newState)
    {
        currentState = newState;
        
        onChangeState?.Invoke((int)currentState);

        switch (currentState)
        {
            case MatchState.Lobby:
                LobbyInit();
                onLobbyState?.Invoke();
                break;
            case MatchState.PreGame:
                PreGameInit();
                onPreGameState?.Invoke();
                break;
            case MatchState.InGame:
                InGameInit();
                onInGameState?.Invoke();
                break;
            case MatchState.PostGame:
                PostGameInit();
                onPostGameState?.Invoke();
                break;
        }
        
        
    }

    #region InitStates

    void LobbyInit()
    {
        Debug.Log("LobbyInit");
    }
    
    void PreGameInit()
    {
        Debug.Log("PreGameInit");
    }

    void InGameInit()
    {
        Debug.Log("InGameInit");
    }

    void PostGameInit()
    {
        Debug.Log("PostGameInit");
    }
    
    #endregion

    #region Server

    private bool isEnoughPlayerTime = false;
    
    public void OnPlayersChanges()
    {
        if (currentState != MatchState.Lobby && currentState != MatchState.PreGame) return;
        
        if (CFCNetworkManager.Instance._gameManager.analytics.GetNumberOfPlayers >= CFCNetworkManager.Instance.maxConnections) //Quantidade maxima atingida
        {
            Debug.Log("Inicio por sala cheia");

            _timer.StopTimer();
            
            ChangeState(MatchState.PreGame);

            isEnoughPlayerTime = false;
        }
        else if (CFCNetworkManager.Instance._gameManager.analytics.GetNumberOfPlayers >= _gameManager.minPlayersToPlay && !isEnoughPlayerTime) //Quantidade minima atingida para começar a contar o tempo
        {
            Debug.Log("Inicio por tempo");
            
            _gameManager.RpcSetupTimeOutTimer(_gameManager.timeOutTime);
            _timer.SetTimer(_gameManager.timeOutTime,
                UpdateTimer,
                () => ChangeState(MatchState.PreGame));
            isEnoughPlayerTime = true;
        }
        else if(CFCNetworkManager.Instance._gameManager.analytics.GetNumberOfPlayers < _gameManager.minPlayersToPlay) //Quantidade insuficiente
        {
            Debug.Log("Sem jogadores suficientes");

            _timer.StopTimer();
            
            _gameManager.RpcStopTimeOutTimer();
            
            ChangeState(MatchState.Lobby);
            
            isEnoughPlayerTime = false;
        }
    }

    private void UpdateTimer(int time)
    {
        _gameManager.RpcUpdateTimer(time);
        Debug.Log("Esperando: "+time);
    }

    #endregion
}
