using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class LevelManager : NetworkBehaviour
{
    [SerializeField] private List<PropsBehavior> _props;
    [SerializeField] private List<PowerUpBehavior> _powerUps;

    void Start()
    {
        if (!isServer) return;
        
        _props = FindObjectsOfType<PropsBehavior>().ToList();
        _powerUps = FindObjectsOfType<PowerUpBehavior>().ToList();
        
        Subscribe();
    }

    void Subscribe()
    {
        GameManager.Instance.match.onLobbyState.AddListener(ResetProps);
        //GameManager.Instance.match.onPreGameState.AddListener(ResetLevel);
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

        foreach (var powerUp in _powerUps)
        {
            powerUp.SpawnPowerUp(); //Altera no servidor
            powerUp.RpcSpawnPowerUp();  //Altera no client
        }
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
