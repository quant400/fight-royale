using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : NetworkBehaviour
{
    [SerializeField] private List<PropsBehavior> _props;

    void Start()
    {
        if (!isServer) return;
        
        _props = FindObjectsOfType<PropsBehavior>().ToList();
        
        Subscribe();
    }

    void Subscribe()
    {
        GameManager.Instance.match.onLobbyState.AddListener(ResetProps);
        //GameManager.Instance.match.onLobbyState.AddListener(ResetPowerUps);
        //GameManager.Instance.match.onPreGameState.AddListener(ResetPowerUps);
        GameManager.Instance.match.onInGameState.AddListener(ResetLevel);
    }

    void ResetLevel()
    {
        Debug.Log("ResetLevel");
        
        ResetProps();
        ResetPowerUps();
        ResetPlayers();
    }

    void ResetProps()
    {
        Debug.Log("ResetProps");
        
        foreach (var prop in _props)
        {
            prop.ResetPosition();
        }
    }

    void ResetPowerUps()
    {
        Debug.Log("ResetPowerUps");
        GameManager.Instance.powerUp.SpawnPowerUps();
    }

    void ResetPlayers()
    {
        var spawnPoints = FindObjectsOfType<NetworkStartPosition>().ToList();
        foreach (var conn in NetworkServer.connections)
        {
            Debug.Log(conn.Value.identity.gameObject.GetComponent<PlayerBehaviour>().pName);
            int randomIndex = Random.Range(0, spawnPoints.Count-1);
            conn.Value.identity.gameObject.GetComponent<PlayerBehaviour>().TargetChangePlayerPosition(spawnPoints[randomIndex].transform.position);
            spawnPoints.RemoveAt(randomIndex);
        }
    }
}
