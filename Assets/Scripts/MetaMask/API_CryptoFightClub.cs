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
    public void PostScore(string id,Action<string> onSuccess, Action<string> onFail)
    {
        string data = "{\"id\":" + id +"}";
        StartCoroutine(ActionPostScore(onSuccess, onFail, id));
    }
    private IEnumerator ActionPostScore(Action<string> onSuccess, Action<string> onFail, string data)
    {

        /*string urlToCall = string.Format(baseURL + "game/sdk/pvp/score");
        Debug.Log(1.2);
        UnityWebRequest www = CreateRequestPOST(urlToCall, data);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(1.3);
            Debug.Log(www.error.ToString());
            onFail?.Invoke(www.error.ToString());
        }
        else
        {
            Debug.Log(1.4);
            onSuccess?.Invoke(www.downloadHandler.text);
        }*/
        // to make it work with free mint
        leaderboardModel.userGetDataModel idData = new leaderboardModel.userGetDataModel();
        idData.id = data;
        string idJsonData = JsonUtility.ToJson(idData);
        using (UnityWebRequest request = UnityWebRequest.Put("https://api.cryptofightclub.io/game/sdk/pvp/score", idJsonData))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(idJsonData);
            request.method = "POST";
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.error == null)
            {
                string result = Encoding.UTF8.GetString(request.downloadHandler.data);
                Debug.Log(result);
                onSuccess?.Invoke(request.downloadHandler.text);

            }
            else
            {
                Debug.Log(request.error.ToString());
                onFail?.Invoke(request.error.ToString());
            }


        }
    }
    public void PostStartSession(string data, Action<string> onSuccess = null, Action<string> onFail = null)
    {
        if(Data_Manager.Instance.isValidAccount())
        {
            StartCoroutine(KeyMaker.instance.startSessionApi(data));
        }
            
    }
    /*private IEnumerator ActionPostStartSession(string data, Action<string> onSuccess = null, Action<string> onFail = null)
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
    }*/
    public void PostEndSession(string data, Action<string> onSuccess = null, Action<string> onFail = null)
    {
        if (Data_Manager.Instance.isValidAccount())
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

    [Obsolete("Don't use anymore", true)]
    public void GetSessions(int nftId, Action<string> onSuccess, Action<string> onFail)
    {
        StartCoroutine(ActionGetSessions(nftId, onSuccess, onFail));
    }
    private IEnumerator ActionGetSessions(int nftId, Action<string> onSuccess, Action<string> onFail)
    {
        string urlToCall = string.Format(baseURL + "game/sdk/status/sessions/{0}", nftId);
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

