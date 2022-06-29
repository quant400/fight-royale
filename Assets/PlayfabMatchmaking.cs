using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CFC.Serializable.GetEntityToken;
using CFC.Serializable.ListMultiplayerServers;
using PlayFab;
using UnityEngine;
using UnityEngine.Networking;

public class PlayfabMatchmaking : MonoBehaviour
{
    [SerializeField] private string baseURL = "https://C5FA3.playfabapi.com/";
    [SerializeField] private string SecretKey = "3W4J5SK6W8GWHZKMH89GAP3FS1CN5YF83436XAFMXMKGCMJASW";
    [SerializeField] private string _BuildId = "27deabec-d6ef-4419-8a93-7d2801da6201";
    [SerializeField] private string Region = "EastUS";

    [Header("EntityToken")]
    public string EntityToken;
    public GetEntityTokenResponse EntityTokenResponse;
    [Header("MultiplayerServers")]
    private ListMultiplayerServersResponse _multiplayerServersResponse;
    public MultiplayerServerSummary CurrentMultiplayerServerSummary;
    public string BuildId => _BuildId;

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
        Debug.Log(data);


        UnityWebRequest www = CreateRequestPOST(urlToCall, data);
        www.SetRequestHeader("X-EntityToken", EntityToken);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            onFail?.Invoke(www.error.ToString());
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            var entityData = JsonUtility.FromJson<ListMultiplayerServersResponse>(www.downloadHandler.text);
            _multiplayerServersResponse = entityData;
            CurrentMultiplayerServerSummary = _multiplayerServersResponse.data.MultiplayerServerSummaries[0];
            onSuccess?.Invoke();
        }
    }



}
