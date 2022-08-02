using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class MrTime_Manager : NetworkBehaviour
{
    public static MrTime_Manager Instance;

    private UnityEvent onFinish;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    private Coroutine _coroutineTest;

    public void SetTimer(int timeInSeconds, Action<int> onSecond, Action onFinish)
    {
        StopTimer();
        _coroutineTest = StartCoroutine(_SetTimer(timeInSeconds, onSecond, onFinish));
    }

    public void StopTimer()
    {
        if (_coroutineTest != null)
        {
            StopCoroutine(_coroutineTest);
            _coroutineTest = null;
        }
    }

    public IEnumerator _SetTimer(int timeInSeconds, Action<int> onSecond, Action onFinish)
    {
        do
        {
            onSecond?.Invoke(timeInSeconds);
            yield return new WaitForSeconds(1);
            timeInSeconds--;
        } while (timeInSeconds >= 0);
        
        onFinish?.Invoke();
    }
}
