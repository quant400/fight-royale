using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour/*NetworkBehaviour*/
{
    [SerializeField] private List<PropsBehavior> _props;
    [SerializeField] private List<Throwable_BehaviorV2> _throwables;

    void Start()
    {
        // Commented for Photon
        //if (!isServer) return;
        
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
        //TODO Suleman: Uncomment Later
        foreach (var item in _throwables.Where(x => x.HasCarrier))
        {
            Debug.Log(item.photonView.ViewID);
            var target = item.carrierNetIdentity.GetComponent<PlayerBehaviour>();
            target._tpFightingControler.StolenObject();
            //target.RpcStealObject(target.photonView.ViewID);
            target.photonView.RPC("RpcStealObject", RpcTarget.All, target.photonView.ViewID);
        }

        foreach (var item in _throwables)
        {
            item.ResetPosition();
            //item.RpcResetPosition();
            item.photonView.RPC("RpcResetPosition", RpcTarget.AllBuffered/*, item.photonView.ViewID*/);
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
            player.netIdentity.gameObject.GetComponent<PlayerBehaviour>().photonView.RPC("RpcChangePlayerPosition", RpcTarget.All, spawnPoints[cont].transform.position); //RpcChangePlayerPosition(spawnPoints[cont].transform.position);

            cont++;

        }
    }
}
