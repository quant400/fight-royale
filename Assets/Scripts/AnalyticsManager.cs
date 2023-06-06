using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CFC.Serializable.Admin.TitleData;
using Mirror;
using Photon.Pun;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    [Header("Data")] [SerializeField] public List<Player> players = new List<Player>();

    [SerializeField] private int _pointsPerKill = 10;
    
    [SerializeField] private bool showDebug = false;

    #region Connection

    public void AddPlayer(string id, PhotonView netIdentity)
    {
        //var oldPlayer = players.FirstOrDefault(aux => aux.id == id);
        //if (oldPlayer != null)
        //{
        //    oldPlayer.netIdentity = netIdentity;
        //}
        //else
        //{
            players.Add(new Player(id, netIdentity));
            //}

            //Debug.Log(string.Join(", ",players.Select(aux => $"{aux.id}, {aux.netIdentity.netId}")));

    }

    public void  RemovePlayer(PhotonView netIdentity)
    {
        // Commented for Photon
        //var auxPlayer = GetPlayerById(((CFCAuth.AuthRequestMessage) netIdentity.connectionToClient.authenticationData).nftId);
        var auxPlayer = GetPlayerByNetIdentity(netIdentity);
        auxPlayer.isConnected = false;
    }

    public bool CheckWallet(string nftIds)
    {
        return true;
        var nftIdsList = nftIds.Split(';').ToList();
        return !players.Where(aux=> aux.isConnected).Any(auxPlayer => nftIdsList.Contains(auxPlayer.id.ToString()));
    }

    #endregion
    
    #region Game Rules
    
    #region Sets

    public void SetPlayerDead(PhotonView netIdentity)
    {
        var auxPlayer = GetPlayerByNetIdentity(netIdentity);
        if (auxPlayer != null) auxPlayer.isDead = true;
        
    }

    
    #endregion

    #region Gets

    public int GetNumberOfPlayersAlive() => players.Count(aux => aux.isConnected && !aux.isDead);
    public int GetNumberOfPlayers => players.Count(aux => aux.isConnected);

    public PhotonView GetWinner()
    {
        PhotonView playerIdentity = null;
        if (players.Count(aux => !aux.isDead && aux.isConnected) == 1)
        {
            var auxPlayer = players.First(aux => aux.isConnected && !aux.isDead);
            if (auxPlayer != null)
            {
                playerIdentity = auxPlayer.netIdentity;
                //TODO Suleman: Uncomment Later
                //GameManager.Instance.TargetRequestEndtSession(auxPlayer.score, auxPlayer.kills);
                GameManager.Instance.player.RPC("TargetRequestEndtSession", RpcTarget.All, auxPlayer.score, auxPlayer.kills);
            }

        }

        return playerIdentity;
    }

    #endregion
    
    #endregion
    
    #region Score

    public void InitScore()
    {
        /* foreach (var player in players)
         {
             RequestStartSession(player);
         }*/
        // Commented for Photon
        //GameManager.Instance.RpcRequestStartSession();
        //TODO Suleman: Uncomment Later
        GameManager.Instance.player.RPC("RpcRequestStartSession", RpcTarget.All);
    }

    public void AddDamageDealt(PhotonView netIdentity, float damage)
    {
        var player = GetPlayerByNetIdentity(netIdentity);
        player.damageDealt += damage;
        AddScore(netIdentity, (int)damage);
        _Debug($"AddDamageDealt: playerDealer={player.id}");

    }
    
    public void AddDamageReceived(PhotonView netIdentity, float damage)
    {
        var player = GetPlayerByNetIdentity(netIdentity);
        player.damageReceived += damage;
        //AddScore(netId, (int)damage);
        _Debug($"AddDamageReceived: playerHit={player.id}");
    }
    
    public void AddKill(PhotonView killerIdentity, PhotonView deadIdentity)
    {
        //Killer
        var killer = GetPlayerByNetIdentity(killerIdentity);
        killer.kills++;
        // Commented for Photon
        //AddScore(killer.netIdentity, _pointsPerKill);
        AddScore(killerIdentity, _pointsPerKill);



        //Dead
        var dead = GetPlayerByNetIdentity(deadIdentity);
        dead.killerId = killer.id;
        SetPlayerDead(deadIdentity);
        //TODO Suleman: Uncomment Later
        //GameManager.Instance.TargetRequestEndtSession(dead.score, dead.kills);
        GameManager.Instance.player.RPC("TargetRequestEndtSession", RpcTarget.All, dead.score, dead.kills);
        _Debug($"AddKill: killer={killer.id}, dead={dead.id}");

        UpdateSpectatorCamera(dead.netIdentity, killer.netIdentity);
    }

    public void UpdateSpectatorCamera(PhotonView dead, PhotonView killer) 
    {
        foreach (var player in players.Where(aux=> aux.isConnected && aux.isDead).Select(aux=>aux.netIdentity.GetComponent<PlayerBehaviour>()))
        {
            //TODO Suleman: Uncomment Later
            //player.TargetChangeSpectatorCamera(dead, killer);
            player.photonView.RPC("TargetChangeSpectatorCamera", RpcTarget.All, dead.ViewID, killer.ViewID);
        }
    }

    private void AddScore(PhotonView netId, int score)
    {
        var player = GetPlayerByNetIdentity(netId);
        player.score += score;

        //UpdateScore
        var playerBehaviour = netId.GetComponent<PlayerBehaviour>();
        //TODO Suleman: Uncomment Later, done
        //GameManager.Instance.TargetSendUpdatedScore(player.score);

        if (playerBehaviour.photonView.IsMine)
        {
            playerBehaviour.photonView.RPC("TargetSendUpdatedScore", RpcTarget.All, player.score);
        }

        _Debug($"AddScore: player={player.id}, score={score}");
    }

    public void SendScore()
    {
        /*foreach (var player in players)
        {
            RequestEndSession(player);
        }*/
        //EndSession
    }

    private void RequestStartSession(Player player)
    {
        RequestStartSession request = new RequestStartSession
        {
            id = player.id
        };
        
        _Debug($"RequestStartSession: id={request.id}");
        // changed for new api        
        //Connection_Manager.Instance.Api_CryptoFightClub.PostStartSession(JsonUtility.ToJson(request),(aux)=>SuccessRequestStartSession(request.id, aux), (aux)=>FailRequestStartSession(request.id, aux));
        Connection_Manager.Instance.Api_CryptoFightClub.PostStartSession(request.id);
    }
    
    private void SuccessRequestStartSession(string id, string result)
    {
        Debug.Log("Sucasss: start session: "+result);
        //RequestEndSession(GetPlayerByNetId(id));
    }

    private void FailRequestStartSession(string id, string erro)
    {
        Debug.Log("Error: Fail to start session");
        //RequestEndSession(GetPlayerByNetId(id));
    }

    private void RequestEndSession(Player player)
    {
        RequestEndSession request = new RequestEndSession
        {
            id = player.id,
            score = player.score,
            kills = player.kills
        };
        
        _Debug($"RequestEndSession: id={request.id}, score={request.score}, kills={request.kills}, ");
        
        Connection_Manager.Instance.Api_CryptoFightClub.PostEndSession(JsonUtility.ToJson(request),(aux)=>SuccessEndSendRequest(request.id, aux), (aux)=>FailEndSendRequest(request.id, aux));
    }

    private void SuccessEndSendRequest(string id, string result)
    {
        Debug.Log("Success: end session: "+result);
    }

    private void FailEndSendRequest(string id, string erro)
    {
        Debug.Log("Error: Fail to end session");
        //RequestEndSession(GetPlayerByNetId(id));
    }

    #endregion

    #region Utils
    
    private Player GetPlayerById(string id)
    {
        return players.LastOrDefault(aux => aux.id == id);
    }

    private Player GetPlayerByNetIdentity(PhotonView netIdentity)
    {
        return players.LastOrDefault(aux => aux.netIdentity.ViewID == netIdentity.ViewID);
    }

    private void _Debug(string desc)
    {
        if(showDebug)
            Debug.Log(desc);
    }

    #endregion
}

[System.Serializable]
public class Player
{
    public string id;
    public PhotonView netIdentity;
    public bool isConnected = true;
    public bool isDead = false;
    public float damageDealt = 0;
    public float damageReceived = 0;
    public string killerId;
    public int kills = 0;
    public int score = 0;

    public Player(string id, PhotonView netIdentity)
    {
        this.id = id;
        this.netIdentity = netIdentity;
    }
}
