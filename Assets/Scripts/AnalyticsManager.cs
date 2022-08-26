using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CFC.Serializable.Admin.TitleData;
using Mirror;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    [Header("Data")] [SerializeField] private List<Player> players = new List<Player>();

    [SerializeField] private int _pointsPerKill = 10;
    
    [SerializeField] private bool showDebug = false;

    #region Connection

    public void AddPlayer(int id, NetworkIdentity netIdentity)
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

    public void  RemovePlayer(NetworkIdentity netIdentity)
    {
        var auxPlayer = GetPlayerById(((CFCAuth.AuthRequestMessage) netIdentity.connectionToClient.authenticationData).nftId);
        auxPlayer.isConnected = false;
    }

    public bool CheckWallet(string nftIds)
    {
        var nftIdsList = nftIds.Split(';').ToList();
        return !players.Where(aux=> aux.isConnected).Any(auxPlayer => nftIdsList.Contains(auxPlayer.id.ToString()));
    }

    #endregion
    
    #region Game Rules
    
    #region Sets

    public void SetPlayerDead(NetworkIdentity netIdentity)
    {
        var auxPlayer = GetPlayerByNetIdentity(netIdentity);
        if (auxPlayer != null) auxPlayer.isDead = true;
        
    }

    
    #endregion

    #region Gets

    public int GetNumberOfPlayersAlive() => players.Count(aux => aux.isConnected && !aux.isDead);
    public int GetNumberOfPlayers => players.Count();

    public NetworkIdentity GetWinner()
    {
        NetworkIdentity playerIdentity = null;
        if (players.Count(aux => !aux.isDead && aux.isConnected) == 1)
        {
            var auxPlayer = players.First(aux => aux.isConnected && !aux.isDead);
            if (auxPlayer != null) playerIdentity =  auxPlayer.netIdentity;
        }

        return playerIdentity;
    }

    #endregion
    
    #endregion
    
    #region Score

    public void InitScore()
    {
        foreach (var player in players)
        {
            RequestStartSession(player);
        }
    }

    public void AddDamageDealt(NetworkIdentity netIdentity, float damage)
    {
        var player = GetPlayerByNetIdentity(netIdentity);
        player.damageDealt += damage;
        AddScore(netIdentity, (int)damage);
        _Debug($"AddDamageDealt: playerDealer={player.id}");

    }
    
    public void AddDamageReceived(NetworkIdentity netIdentity, float damage)
    {
        var player = GetPlayerByNetIdentity(netIdentity);
        player.damageReceived += damage;
        //AddScore(netId, (int)damage);
        _Debug($"AddDamageReceived: playerHit={player.id}");
    }
    
    public void AddKill(NetworkIdentity killerIdentity, NetworkIdentity deadIdentity)
    {
        //Killer
        var killer = GetPlayerByNetIdentity(killerIdentity);
        killer.kills++;
        AddScore(killer.netIdentity, _pointsPerKill);
        
        //Dead
        var dead = GetPlayerByNetIdentity(deadIdentity);
        dead.killerId = killer.id;
        SetPlayerDead(deadIdentity);
        
        _Debug($"AddKill: killer={killer.id}, dead={dead.id}");
        
    }

    private void AddScore(NetworkIdentity netId, int score)
    {
        var player = GetPlayerByNetIdentity(netId);
        player.score += score;
        _Debug($"AddScore: player={player.id}, score={score}");
    }

    public void SendScore()
    {
        foreach (var player in players)
        {
            RequestEndSession(player);
        }
        //EndSession
    }

    private void RequestStartSession(Player player)
    {
        RequestStartSession request = new RequestStartSession
        {
            id = player.id
        };
        
        _Debug($"RequestStartSession: id={request.id}");
        
        Connection_Manager.Instance.Api_CryptoFightClub.PostStartSession(JsonUtility.ToJson(request),(aux)=>SuccessRequestStartSession(request.id, aux), (aux)=>FailRequestStartSession(request.id, aux));
    }
    
    private void SuccessRequestStartSession(int id, string result)
    {
        Debug.Log("Sucasss: start session: "+result);
        //RequestEndSession(GetPlayerByNetId(id));
    }

    private void FailRequestStartSession(int id, string erro)
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

    private void SuccessEndSendRequest(int id, string result)
    {
        Debug.Log("Success: end session: "+result);
    }

    private void FailEndSendRequest(int id, string erro)
    {
        Debug.Log("Error: Fail to end session");
        //RequestEndSession(GetPlayerByNetId(id));
    }

    #endregion

    #region Utils
    
    private Player GetPlayerById(int id)
    {
        return players.FirstOrDefault(aux => aux.id == id);
    }

    private Player GetPlayerByNetIdentity(NetworkIdentity netIdentity)
    {
        return players.FirstOrDefault(aux => aux.netIdentity.netId == netIdentity.netId);
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
    public int id;
    public NetworkIdentity netIdentity;
    public bool isConnected = true;
    public bool isDead = false;
    public float damageDealt = 0;
    public float damageReceived = 0;
    public int killerId;
    public int kills = 0;
    public int score = 0;

    public Player(int id, NetworkIdentity netIdentity)
    {
        this.id = id;
        this.netIdentity = netIdentity;
    }
}
