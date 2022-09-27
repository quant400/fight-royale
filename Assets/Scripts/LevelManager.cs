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
    [SerializeField] private List<Throwable_BehaviorV2> _throwables;

    void Start()
    {
        if (!isServer) return;
        
        _props = FindObjectsOfType<PropsBehavior>().ToList();
        _throwables = FindObjectsOfType<Throwable_BehaviorV2>().ToList();


        Subscribe();
    }

    void Subscribe()
    {
        GameManager.Instance.match.onLobbyState.AddListener(ResetProps);
        //GameManager.Instance.match.onLobbyState.AddListener(ResetPowerUps);
        //GameManager.Instance.match.onPreGameState.AddListener(ResetPlayers);
        GameManager.Instance.match.onInGameState.AddListener(ResetLevel);
    }

    void ResetLevel()
    {        
        ResetProps();
        ResetPowerUps();
        ResetPlayers();
    }

    void ResetProps()
    {
        foreach (var item in _throwables.Where(x=> x.HasCarrier))
        {
            Debug.Log(item.netId);
            var target = item.carrierNetIdentity.GetComponent<PlayerBehaviour>();
            target._tpFightingControler.StolenObject();
            target.RpcStealObject(target.netIdentity);
        }

        foreach (var item in _throwables)
        {
            item.ResetPosition();
            item.RpcResetPosition();
        }
    }

    void ResetPowerUps()
    {
        Debug.Log("ResetPowerUps");
        GameManager.Instance.powerUp.SpawnPowerUps();
    }



    public void ResetPlayers()
    {
        var spawnPoints = FindObjectsOfType<NetworkStartPosition>().ToList();
        int cont = 0;
        foreach (var player in GameManager.Instance.analytics.players.Where(aux=> aux.isConnected))
        {
            player.netIdentity.transform.position = spawnPoints[cont].transform.position;
            player.netIdentity.gameObject.GetComponent<PlayerBehaviour>().RpcChangePlayerPosition(spawnPoints[cont].transform.position);

            cont++;

        }
    }
}
