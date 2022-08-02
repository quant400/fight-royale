using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CloseAreaManager : MonoBehaviour
{

    [SerializeField] private TimerManager _timer;

    [SerializeField] private List<DeadlyArea> _deadlyAreas;

    public int maxRound => _deadlyAreas.Count;

    private DeadlyArea _nextArea;

    public void StartTimer()
    {
        Debug.Log("StartTimer");
        ResetAreas();
    }

    public void UpdateAreas()
    {
        //Debug.Log($"UpdateAreas: {_timer.currentRound}");
        for (int i = 0; i < _timer.currentRound; i++)
        {
            if(_deadlyAreas[i].itsOpen)
                CloseArea(i);
        }
    }

    public void CloseArea(int currentRound)
    {
        //Debug.Log($"CloseArea: {currentRound}");
        _nextArea = _deadlyAreas[currentRound];
        _nextArea.DoorObjects.ForEach(door => door.GetComponent<Animator>().Play("Close"));
        _nextArea.itsOpen = false;
        _nextArea.gameObject.SetActive(true);
    }

    public void ResetAreas()
    {
        Debug.Log("Reset");
        foreach (var area in _deadlyAreas)
        {
            area.itsOpen = true;
            area.DoorObjects.ForEach(door => door.GetComponent<Animator>().Play("Open"));
            area.gameObject.SetActive(false);
        }
    }

}
