using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TimerManager : NetworkBehaviour
{

    private MenuManager Menu => MenuManager.Instance;

    public enum GameState
    {
        Fight,
        Escape
    }

    [Header("Essencial")]
    public CloseAreaManager CloseAreaManager;
    [Header("Timer")]
    public int MaxTime = 60;
    public int CriticalTime = 15;
    public int CloseTime = 5;
    
    [SyncVar] public GameState State;
    [SyncVar(hook = nameof(OnCurrentTimeChange))] public int CurrentTime;
    [SyncVar(hook = nameof(OnCurrentRoundChange))] public int currentRound = 0;
    private Coroutine _coroutineTimer;

    void Start()
    {
        Menu.ShowTime(true);
        //if(isServer && false) StartTime();
    }
    
    

    public void StartTime()
    {
        if (!isServer) return;
        ResetTime();
        

        if (currentRound >= CloseAreaManager.maxRound-1)
        {
            Console.WriteLine($"Acabou!");
            return;
            RpcResetDoors();
            currentRound = 0;
        }
        //else
            //RpcCloseDoor(currentRound);
            
        Console.WriteLine($"{currentRound} > {CloseAreaManager.maxRound}");

        _coroutineTimer = StartCoroutine(LoopTime(1, () =>
        {
            StartNextArea();
        }));
    }

    private void ResetTime()
    {
        State = GameState.Fight;
        CurrentTime = MaxTime;
        SetDefaultTheme();
    }

    private void SetDefaultTheme()
    {
        Menu.CustomTime(CurrentTime.ToString(), MenuManager.ThemeTime.Default);
    }

    private IEnumerator LoopTime(int decrementValue, Action onFinish)
    {
        do
        {
            yield return new WaitForSeconds(decrementValue);
            CurrentTime -= decrementValue;
        } while (CurrentTime > 0);

        onFinish?.Invoke();
    }

    public void OnCurrentRoundChange(int oldValue, int newValue)
    {
        CloseAreaManager.UpdateAreas();
    }

    public void OnCurrentTimeChange(int oldValue, int newValue)
    {
        //Debug.Log($"State: {State} | CurrentTime: {CurrentTime}");
        switch (State)
        {
            case GameState.Fight:
                if (CriticalTime > 0 && CurrentTime <= CriticalTime)
                {
                    Menu.CustomTime(CurrentTime.ToString(), MenuManager.ThemeTime.Critical);
                }
                else
                {
                    Menu.CustomTime(CurrentTime.ToString(), MenuManager.ThemeTime.Default);
                }
                break;
            case GameState.Escape:
                Menu.CustomTime($"Closing Area \n"+CurrentTime, MenuManager.ThemeTime.Alert);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    private void StartNextArea()
    {
        StopCoroutine(_coroutineTimer);
        
        State = GameState.Escape;
        CurrentTime = CloseTime;
        
        _coroutineTimer = StartCoroutine(LoopTime(1, () =>
        {
            StartTime();
            //RpcCloseDoor(currentRound);
            currentRound++;
        }));

    }

    [ClientRpc]
    public void RpcCloseDoor(int doorCount)
    {
        CloseAreaManager.CloseArea(doorCount);
    }
    
    [ClientRpc]
    public void RpcResetDoors()
    {
        CloseAreaManager.ResetAreas();
    }




}
