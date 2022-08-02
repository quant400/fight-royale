using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class API_CryptoFightClub : MonoBehaviour
{
    [SerializeField] private string baseURL = "https://api.cryptofightclub.io/";
    private UnityWebRequest CreateRequestGET(string urlToCall)
    {
        UnityWebRequest www = UnityWebRequest.Get(urlToCall);
        return www;
    }
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
        return www;
    }

    #region METAMASK

#if UNITY_WEBGL
    async public void OnSignIn(Action<string> onSuccess, Action<string> onFail)
    {
        try
        {
            string message = "Connecting MetaMask in CFC...";
            string contract = await Web3GL.Sign(message);
            int countTries = 0;
            int maxTries = 15;
            float periodTries = 1.0f;

            while (contract == "" && countTries < maxTries)
            {
                Debug.Log(contract + "Try " + countTries + "/" + maxTries);
                await new WaitForSeconds(periodTries);
                countTries++;
            };

            if (countTries < maxTries)
                onSuccess?.Invoke(contract);
            else
                onFail?.Invoke("504 Gateway Timeout");

        }
        catch (Exception e)
        {
            onFail?.Invoke(e.Message);
        }
    }
#endif

    #endregion

    #region CFC
    public void GetAccount(Action<string> onSuccess, Action<string> onFail)
    {
        StartCoroutine(ActionGetAccount(onSuccess, onFail));
    }
    private IEnumerator ActionGetAccount(Action<string> onSuccess, Action<string> onFail)
    {
        string urlToCall = string.Format(baseURL + "game/sdk/{0}", Data_Manager.Instance.accountId);

        UnityWebRequest www = CreateRequestGET(urlToCall);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            onFail?.Invoke(www.responseCode.ToString());
        }
        else
        {
            string json = "{ \"accounts\": " + www.downloadHandler.text + "}";
            onSuccess?.Invoke(json);
        }
    }
    public void PostScore(Action<string> onSuccess, Action<string> onFail,string data)
    {
        StartCoroutine(ActionPostScore(onSuccess, onFail, data));
    }
    private IEnumerator ActionPostScore(Action<string> onSuccess, Action<string> onFail, string data)
    {
        string urlToCall = string.Format(baseURL + "game/sdk/pvp/score");
        
        UnityWebRequest www = CreateRequestPOST(urlToCall, data);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            onFail?.Invoke(www.error.ToString());
        }
        else
        {
            onSuccess?.Invoke(www.downloadHandler.text);
        }
    }
    public void PostStartSession(string data, Action<string> onSuccess = null, Action<string> onFail = null)
    {
        StartCoroutine(ActionPostStartSession(data, onSuccess, onFail));
    }
    private IEnumerator ActionPostStartSession(string data, Action<string> onSuccess = null, Action<string> onFail = null)
    {
        string urlToCall = string.Format(baseURL + "game/sdk/pvp/start-session");

        UnityWebRequest www = CreateRequestPOST(urlToCall, data);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            onFail?.Invoke(www.error.ToString());
        }
        else
        {
            onSuccess?.Invoke(www.downloadHandler.text);
        }
    }
    public void PostEndSession(string data, Action<string> onSuccess = null, Action<string> onFail = null)
    {
        StartCoroutine(ActionPostEndSession(data, onSuccess, onFail));
    }
    private IEnumerator ActionPostEndSession(string data, Action<string> onSuccess = null, Action<string> onFail = null)
    {
        string urlToCall = string.Format(baseURL + "game/sdk/pvp/end-session");

        UnityWebRequest www = CreateRequestPOST(urlToCall, data);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            onFail?.Invoke(www.error.ToString());
        }
        else
        {
            onSuccess?.Invoke(www.downloadHandler.text);
        }
    }
    public void GetDaily(Action<string> onSuccess, Action<string> onFail)
    {
        StartCoroutine(ActionGetDaily(onSuccess, onFail));
    }
    private IEnumerator ActionGetDaily(Action<string> onSuccess, Action<string> onFail)
    {
        string urlToCall = string.Format(baseURL + "game/sdk/pvp/leaderboard/daily");

        UnityWebRequest www = CreateRequestGET(urlToCall);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            onFail?.Invoke(www.error);
        }
        else
        {
            onSuccess?.Invoke(www.downloadHandler.text);
        }
    }
    public void GetAllTime(Action<string> onSuccess, Action<string> onFail)
    {
        StartCoroutine(ActionGetAllTime(onSuccess, onFail));
    }
    private IEnumerator ActionGetAllTime(Action<string> onSuccess, Action<string> onFail)
    {
        string urlToCall = string.Format(baseURL + "game/sdk/pvp/leaderboard/alltime");

        UnityWebRequest www = CreateRequestGET(urlToCall);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            onFail?.Invoke(www.error);
        }
        else
        {
            onSuccess?.Invoke(www.downloadHandler.text);
        }
    }
    public void GetReset(Action<string> onSuccess, Action<string> onFail)
    {
        StartCoroutine(ActionGetReset(onSuccess, onFail));
    }
    private IEnumerator ActionGetReset(Action<string> onSuccess, Action<string> onFail)
    {
        var code = "175";
        string urlToCall = string.Format(baseURL + "game/sdk/pvp/reset{0}", code);
        UnityWebRequest www = CreateRequestGET(urlToCall);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            onFail?.Invoke(www.error);
        }
        else
        {
            onSuccess?.Invoke(www.downloadHandler.text);
        }
    }

    #endregion

}

