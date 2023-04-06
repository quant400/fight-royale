using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using PlayFab;
using UnityEngine;

public class Connection_Manager : MonoBehaviour
{
    public static Connection_Manager Instance;
    public API_CryptoFightClub Api_CryptoFightClub;
    public API_PlayfabMatchmaking Api_PlayfabMatchmaking;
    private List<CFC.Serializable.Admin.TitleData.Data> lobbyPrioritized = null;

    public enum LobbyState
    {
        Open = 0,
        Closed = 1
    }

    private string _serverId;
    public string ServerIp => Api_PlayfabMatchmaking.CurrentMultiplayerServerSummary.IPV4Address;
    public ushort ServerPort => (ushort)Api_PlayfabMatchmaking.CurrentMultiplayerServerSummary.Ports[0].Num;
    public string BuildId => Api_PlayfabMatchmaking.BuildId;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
    private void Start()
    {
        Api_PlayfabMatchmaking.GetEntityToken(OnSuccessEntityToken, OnFail);

    }
    private void OnFail(string obj)
    {
        Debug.Log(obj);
    }
    private void OnSuccessEntityToken(string result)
    {
        Api_PlayfabMatchmaking.ListBuildSummariesV2(() => 
        {
            Api_PlayfabMatchmaking.ListMultiplayerServers(() => { 
            GetLobby();
                
             }, 
             OnFail);
        }, OnFail);
            
    }
    private void GetLobby()
    {
        Api_PlayfabMatchmaking.GetTitleData(() => { 
            
            Debug.Log("GetLobby OnSuccess"); 
            CreateListLobby(); 
        
        }, OnFail);
    }
    private IEnumerator ActionSearchAvailableLobby(Action actionOnStart, Action actionOnSuccess, Action actionOnFullLobby= null,Action actionOnFail = null)
    {

        actionOnStart();
        Api_PlayfabMatchmaking.GetBestServerId((lb) => { lobbyPrioritized = lb; });
        yield return new WaitForSeconds(5.25f);
        
        try
        {
            var found = false;
            
            Api_PlayfabMatchmaking.GetTitleData(() => {

                Debug.Log("GetTitleData OnSuccess");
                CreateListLobby();
                found = VerifyAvailableLobby();

                if (found)
                {
                    Debug.Log("Found Lobby");
                    actionOnSuccess();
                }
                else
                {
                    Debug.Log("Not Found Lobby - Try Again");
                    actionOnFullLobby?.Invoke();
                    SearchAvailableLobby(actionOnStart, actionOnSuccess, actionOnFail);
                }
                    

            }, (msg) => {

                Debug.Log("SearchAvailableLobby OnFail");
                OnFail(msg);
                actionOnFail?.Invoke();
            });
        }
        catch (Exception e)
        {
            OnFail(e.Message);
            actionOnFail?.Invoke();
        }
    }
    private void CreateListLobby()
    {
        Debug.Log("CreateListLobby");

        Api_PlayfabMatchmaking.Lobby = new List<CFC.Serializable.Admin.TitleData.Data>();

        foreach (var d in Api_PlayfabMatchmaking.ContentDataTitle.dic)
        {
            Api_PlayfabMatchmaking.Lobby.Add(new CFC.Serializable.Admin.TitleData.Data()
            {
                Key = d.Key,
                Value = d.Value
            });
        }

        Api_PlayfabMatchmaking.CreateDataTitle();
    }
    private bool VerifyAvailableLobby()
    {
        var id = (int)LobbyState.Open;
        var listLobby = Api_PlayfabMatchmaking.Lobby;

        if (lobbyPrioritized != null && lobbyPrioritized.Count > 0 && lobbyPrioritized.Count == listLobby.Count)
        {
            Debug.Log("LobbyPrioritized by player");
            foreach (var dic in lobbyPrioritized)
            {
                dic.Value = listLobby.First(aux => aux.Key == dic.Key).Value;
            }

            listLobby = lobbyPrioritized;

        }

            foreach (var dic in listLobby)
            {
                if (dic.Value.Equals(id.ToString()))
                {
                    Api_PlayfabMatchmaking.CurrentMultiplayerServerSummary = Api_PlayfabMatchmaking.GetMultiplayerServerSummary(dic.Key);

                    if (Api_PlayfabMatchmaking.CurrentMultiplayerServerSummary != null)
                    {
                        return true;
                    }
                }
            }
        

        return false;
    }
    private void UpdateAvailableLobby(string key, string value, Action onFinish = null)
    {
        var postData = new CFC.Serializable.Admin.TitleData.Data()
        {
            Key = key,
            Value = value
        };

        Api_PlayfabMatchmaking.SetTitleData(() => 
        { 
            GetLobby();
            onFinish?.Invoke();
        }
        , OnFail, postData);

    }
    public void SearchAvailableLobby(Action actionOnStart, Action actionOnSuccess, Action actionOnFullLobby = null, Action actionOnFail = null)
    {
        StartCoroutine(ActionSearchAvailableLobby(actionOnStart, actionOnSuccess, actionOnFullLobby,actionOnFail));
    }
    public void SendCloseLobby(Action onFinish = null)
    {
        try
        {
            var config = FindObjectOfType<Configuration>();
            if (config == null) return;
            if (config.buildType != BuildType.REMOTE_SERVER) return;

       
            if (string.IsNullOrEmpty(_serverId))
            {
                var dic = PlayFabMultiplayerAgentAPI.GetConfigSettings();
                if (dic != null)
                    _serverId = dic[PlayFabMultiplayerAgentAPI.ServerIdKey];
            }
            
            if (!string.IsNullOrEmpty(_serverId))
            {
                Debug.Log("SendCloseLobby");
                int valueId = (int)LobbyState.Closed;
                UpdateAvailableLobby(key: _serverId, value: valueId.ToString(), onFinish);
            }
            
        }
        catch (Exception e)
        {
            Debug.Log(e);
            UpdateAvailableLobby("0x00000000000000000000000000000000000000000000000000000000000001", e.Message , onFinish);
        }
        
    }
    public void SendOpenLobby(Action onFinish = null)
    {
        try
        {

            var config = FindObjectOfType<Configuration>();
            if (config == null) return;
            if (config.buildType != BuildType.REMOTE_SERVER) return;

        
            if (string.IsNullOrEmpty(_serverId))
            {
                var dic = PlayFabMultiplayerAgentAPI.GetConfigSettings();
                if (dic != null)
                    _serverId = dic[PlayFabMultiplayerAgentAPI.ServerIdKey];
            }

            if (!string.IsNullOrEmpty(_serverId))
            {
                Debug.Log("SendOpenLobby");
                int _valueId = (int)LobbyState.Open;
                UpdateAvailableLobby(key: _serverId, value: _valueId.ToString(), onFinish);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            UpdateAvailableLobby(key: "0000000000000000000000000000000000000000000000000000000000000001", value: e.Message, onFinish);
        }

    }
    public void GetLeaderboardAllTime(Action actionOnSuccess, Action<string> actionOnFail)
    {
        Api_CryptoFightClub.GetAllTime((data) => {
            try
            {
                var json = "{ \"scoreboards\": " + data + "}";
                Data_Manager.Instance.leaderboard_AllTime = JsonUtility.FromJson<CFC.Serializable.Leaderboard.RootLeaderboard>(json);
                actionOnSuccess();

            }
            catch (Exception e)
            {
                actionOnFail(e.Message);
                Debug.Log(e);
                
            }

        }, actionOnFail);

    }
    public void GetLeaderboardDaily(Action actionOnSuccess, Action<string> actionOnFail)
    {
        Api_CryptoFightClub.GetDaily((data) => {
            try
            {
                var json = "{ \"scoreboards\": " + data + "}";
                Data_Manager.Instance.leaderboard_Daily = JsonUtility.FromJson<CFC.Serializable.Leaderboard.RootLeaderboard>(json);
                actionOnSuccess();

            }
            catch (Exception e)
            {
                Debug.Log(e);
                actionOnFail(e.Message);
            }

        }, actionOnFail);

    }
    public void GetPlayerPvpScore(string nftId, Action<CFC.Serializable.PlayerPvpScore> actionOnSuccess,Action<string> actionOnFail)
    {
        CFC.Serializable.PlayerPvpScore playerPvpScore = null;

        Api_CryptoFightClub.PostScore(nftId, (data) => 
        {
            try
            {
                Debug.Log(data);
                var json = data;
                playerPvpScore = JsonUtility.FromJson<CFC.Serializable.PlayerPvpScore>(json);
                actionOnSuccess(playerPvpScore);
            }
            catch (Exception e)
            {
                Debug.Log(3);
                Debug.Log(e);
                actionOnFail(e.Message);
            }
        }
        , actionOnFail);

    }
}
