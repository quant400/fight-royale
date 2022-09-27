using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CFC.Serializable.GetEntityToken;
using CFC.Serializable.ListMultiplayerServers;
using Newtonsoft.Json.Linq;
using PlayFab;
using UnityEngine;
using UnityEngine.Networking;

public class API_PlayfabMatchmaking : MonoBehaviour
{
    [Header("PlayfabAPI")]
    [SerializeField] private string baseURL = "https://C5FA3.playfabapi.com/";
    [SerializeField] private string SecretKey = "3W4J5SK6W8GWHZKMH89GAP3FS1CN5YF83436XAFMXMKGCMJASW";
    [SerializeField] private string Region = "EastUS";

    [SerializeField] private string _BuildId = null;

    public string BuildId => GetBuildId();

    [Header("EntityToken")]
    [Space]
    public string EntityToken;
    public GetEntityTokenResponse EntityTokenResponse;

    [Header("MultiplayerServers")]
    [Space]
    public MultiplayerServerSummary CurrentMultiplayerServerSummary;
    [SerializeField] private ListMultiplayerServersResponse _multiplayerServersResponse;
  
    [Header("BuildSummaries")]
    [Space]
    public CFC.Serializable.MultiplayerServers.Root BuildSummariesV2;

    [Header("TitleData")]
    public CFC.Serializable.Admin.TitleData.DataKey ContentDataTitle;

    [Header("LobbyData")]
    [Space]
    public List<CFC.Serializable.Admin.TitleData.Data> Lobby;

    private UnityWebRequest CreateRequestPOST(string urlToCall, string data)
    {

        UnityWebRequest www = new UnityWebRequest(urlToCall, UnityWebRequest.kHttpVerbPOST)
        {
            downloadHandler = new DownloadHandlerBuffer(),
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data))
            {
                contentType = "application / json"
            }
        };

        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Host", "C5FA3.playfabapi.com");

        return www;
    }
    public void GetEntityToken(Action<string> onSuccess, Action<string> onFail)
    {
        StartCoroutine(ActionGetEntityToken(onSuccess, onFail));
    }
    private IEnumerator ActionGetEntityToken(Action<string> onSuccess, Action<string> onFail)
    {
        string urlToCall = string.Format(baseURL + "Authentication/GetEntityToken");

        var postData = "{}";

        UnityWebRequest www = CreateRequestPOST(urlToCall, postData);
        www.SetRequestHeader("X-SecretKey", SecretKey);
        www.SetRequestHeader("Content-Length", "0");
        yield return www.SendWebRequest();


        if (www.isNetworkError || www.isHttpError)
        {
            onFail?.Invoke(www.error.ToString());
        }
        else
        {
            var entityData = JsonUtility.FromJson<GetEntityTokenResponse>(www.downloadHandler.text);
            EntityTokenResponse = entityData;
            EntityToken = entityData.data.EntityToken;
            onSuccess?.Invoke(entityData.data.EntityToken);

        }
    }
    public void ListMultiplayerServers(Action onSuccess, Action<string> onFail)
    {
        StartCoroutine(ActionListMultiplayerServers(onSuccess, onFail));
    }
    private IEnumerator ActionListMultiplayerServers(Action onSuccess, Action<string> onFail)
    {
        string urlToCall = string.Format(baseURL + "MultiplayerServer/ListMultiplayerServers");

        var postData = new PostData()
        {
            BuildId = this.BuildId,
            Region = this.Region
        };

        var data = JsonUtility.ToJson(postData);
        //Debug.Log(data);


        UnityWebRequest www = CreateRequestPOST(urlToCall, data);
        www.SetRequestHeader("X-EntityToken", EntityToken);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            onFail?.Invoke(www.error.ToString());
        }
        else
        {
            //Debug.Log(www.downloadHandler.text);
            var entityData = JsonUtility.FromJson<ListMultiplayerServersResponse>(www.downloadHandler.text);
            _multiplayerServersResponse = entityData;
            //CurrentMultiplayerServerSummary = _multiplayerServersResponse.data.MultiplayerServerSummaries[0];
            onSuccess?.Invoke();
        }
    }
    public void ListBuildSummariesV2(Action onSuccess, Action<string> onFail)
    {
        StartCoroutine(ActionListBuildSummariesV2(onSuccess, onFail));
    }
    private IEnumerator ActionListBuildSummariesV2(Action onSuccess, Action<string> onFail)
    {
        string urlToCall = string.Format(baseURL + "MultiplayerServer/ListBuildSummariesV2");
        var size = 10;
        var data = "{\"PageSize\":" + size + "}";

        UnityWebRequest www = CreateRequestPOST(urlToCall, data);
        www.SetRequestHeader("X-EntityToken", EntityToken);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            onFail?.Invoke(www.error.ToString());
        }
        else
        {
            //Debug.Log(www.downloadHandler.text);
            var entityData = JsonUtility.FromJson<CFC.Serializable.MultiplayerServers.Root>(www.downloadHandler.text);
            BuildSummariesV2 = entityData;
            onSuccess?.Invoke();
        }
    }

    #region TitleData
    public void GetTitleData(Action onSuccess, Action<string> onFail)
    {
        StartCoroutine(ActionGetTitleData(onSuccess, onFail));
    }
    private IEnumerator ActionGetTitleData(Action onSuccess, Action<string> onFail)
    {
        string urlToCall = string.Format(baseURL + "Admin/GetTitleData");

        var data = JsonUtility.ToJson(
         Connection_Manager.Instance.Api_PlayfabMatchmaking._multiplayerServersResponse.data.MultiplayerServerSummaries
             .Select(aux => aux.ServerId));


        UnityWebRequest www = CreateRequestPOST(urlToCall, data);
        www.SetRequestHeader("X-SecretKey", SecretKey);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            onFail?.Invoke(www.error.ToString());
        }
        else
        {
            JObject json = JObject.Parse(www.downloadHandler.text);
            Dictionary<string, string> dic = json["data"]["Data"].ToObject<Dictionary<string, string>>();

            ContentDataTitle = new CFC.Serializable.Admin.TitleData.DataKey()
            {
                dic = dic
            };

            onSuccess?.Invoke();
        }
    }
    public void SetTitleData(Action onSuccess, Action<string> onFail, CFC.Serializable.Admin.TitleData.Data postData)
    {
        StartCoroutine(ActionSetTitleData(onSuccess, onFail, postData));
    }
    private IEnumerator ActionSetTitleData(Action onSuccess, Action<string> onFail, CFC.Serializable.Admin.TitleData.Data postData)
    {
        string urlToCall = string.Format(baseURL + "Admin/SetTitleData");
        var data = JsonUtility.ToJson(postData);
       // Debug.Log(data);

        UnityWebRequest www = CreateRequestPOST(urlToCall, data);
        www.SetRequestHeader("X-SecretKey", SecretKey);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            onFail?.Invoke(www.error.ToString());
        }
        else
        {
            yield return new WaitForSeconds(0.20f);
            onSuccess?.Invoke();
        }
    }
    #endregion

    #region InternalTitleData
    private void SetTitleInternalData(Action onSuccess, Action<string> onFail, CFC.Serializable.Admin.TitleData.Data postData)
    {
        StartCoroutine(ActionSetTitleInternalData(onSuccess, onFail, postData));
    }
    private IEnumerator ActionSetTitleInternalData(Action onSuccess, Action<string> onFail, CFC.Serializable.Admin.TitleData.Data postData)
    {
        string urlToCall = string.Format(baseURL + "Server/SetTitleInternalData");
        var data = JsonUtility.ToJson(postData);

        yield return new WaitForSeconds(1.0f);

        // Debug.Log(data);

        UnityWebRequest www = CreateRequestPOST(urlToCall, data);
        www.SetRequestHeader("X-SecretKey", SecretKey);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            onFail?.Invoke(www.error.ToString());
        }
        else
        {
            yield return new WaitForSeconds(0.20f);
            onSuccess?.Invoke();
        }
    }
    private void GetTitleInternalData(Action<List<CFC.Serializable.Admin.TitleData.Data>> onSuccess, Action<string> onFail)
    {
        StartCoroutine(ActionGetTitleInternalData(onSuccess, onFail));
    }
    private IEnumerator ActionGetTitleInternalData(Action<List<CFC.Serializable.Admin.TitleData.Data>> onSuccess, Action<string> onFail)
    {
        string urlToCall = string.Format(baseURL + "Server/GetTitleInternalData");

        var data = JsonUtility.ToJson(
         Connection_Manager.Instance.Api_PlayfabMatchmaking._multiplayerServersResponse.data.MultiplayerServerSummaries
             .Select(aux => aux.ServerId));


        UnityWebRequest www = CreateRequestPOST(urlToCall, data);
        www.SetRequestHeader("X-SecretKey", SecretKey);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            onFail?.Invoke(www.error.ToString());
        }
        else
        {
            JObject json = JObject.Parse(www.downloadHandler.text);
            Dictionary<string, string> dic = json["data"]["Data"].ToObject<Dictionary<string, string>>();

            var dicData = new CFC.Serializable.Admin.TitleData.DataKey()
            {
                dic = dic
            };

            var internalDataServer = new List<CFC.Serializable.Admin.TitleData.Data>();

            foreach (var d in dicData.dic)
            {
                internalDataServer.Add(new CFC.Serializable.Admin.TitleData.Data()
                {
                    Key = d.Key,
                    Value = d.Value
                });
            }

            onSuccess?.Invoke(internalDataServer);
        }
    }
    #endregion

    private string GetBuildId()
    {
        if (string.IsNullOrEmpty(_BuildId))
        {
            return BuildSummariesV2.data.BuildSummaries[0].BuildId;
        }
        else
        {
            return _BuildId;
        }
    }
    public MultiplayerServerSummary GetMultiplayerServerSummary(string serverId)
    {
        return _multiplayerServersResponse.data.MultiplayerServerSummaries.FirstOrDefault(aux => aux.ServerId.ToLower().Equals(serverId.ToLower()));
    }
    public void CreateDataTitle()
    {
        var listTitle = _multiplayerServersResponse.data.MultiplayerServerSummaries.Where(aux => !Lobby.Select(auxLobby => auxLobby.Key).Contains((aux.ServerId)));
        foreach (var p in listTitle)
        {
            SetTitleData(()=> { Debug.Log("Success Create DataTitle = " + p.ServerId );},
                        (msg)=> { Debug.Log("Fail Create DataTitle " + msg); }, 
                        new CFC.Serializable.Admin.TitleData.Data 
                        {
                        Key = p.ServerId,
                        Value = "0"
                        });
        }
    }
    public void SetPlayersOnServer(string serverId,int countPlayer)
    {
        var data = new CFC.Serializable.Admin.TitleData.Data
        {
            Key = serverId,
            Value = (countPlayer).ToString()
        };

        SetTitleInternalData(
            ()=> { Debug.Log("SetPlayersOnServer [Key : " + data.Key + " ] - " + "[ Value : " + data.Value + " ]"); },
            (error) => { Debug.Log("SetPlayersOnServerError" + error); },
            data
            );
    }
    public List<CFC.Serializable.Admin.TitleData.Data> GetBestServerId()
    {
        List<CFC.Serializable.Admin.TitleData.Data> serverByPlayer = null;
        GetTitleInternalData(
            (data) =>
            {
                serverByPlayer = data.OrderByDescending(aux => aux.Value).ToList();
            }, 
            (error) =>
            {
                Debug.Log(error);
            }
            );

        return serverByPlayer;
    }

}
