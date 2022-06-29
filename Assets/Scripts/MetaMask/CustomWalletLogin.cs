using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CustomWalletLogin : MonoBehaviour
{

    private string baseURL = "https://api.cryptofightclub.io/";

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
    public void GetAccount(Action<string> onSuccess, Action<string> onFail)
    {
        StartCoroutine(ActionGetAccount(onSuccess, onFail));
    }

    private IEnumerator ActionGetAccount(Action<string> onSuccess, Action<string> onFail)
    {
        string urlToCall = string.Format(baseURL + "game/sdk/{0}", Data_Manager.Instance.accountId);

        UnityWebRequest www = CreateRequestGET(urlToCall);
        yield return www.SendWebRequest();
        //Debugp.Log(urlToCall);

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

    private UnityWebRequest CreateRequestGET(string urlToCall)
    {
        UnityWebRequest www = UnityWebRequest.Get(urlToCall);
        return www;
    }
}

