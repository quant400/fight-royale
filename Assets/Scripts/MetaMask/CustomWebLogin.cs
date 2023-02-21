using System;
using System.Runtime.InteropServices;
using UnityEngine;


public class CustomWebLogin : MonoBehaviour
{


    [Header("Local AccountId")]
    [SerializeField] private string _account;

    [Header("Start")]
    [SerializeField]
    private GameObject panelStart;
    [SerializeField]
    private GameObject panelTryOut;
    [SerializeField]
    private GameObject panelSelection;
    [Header("Connecting")]
    [SerializeField]
    private GameObject panelConnecting;
    [Header("Error")]
    [SerializeField]
    private GameObject panelError;
    [SerializeField]
    private TMPro.TextMeshProUGUI textError;

    private API_CryptoFightClub Api_CryptoFightClub => Connection_Manager.Instance.Api_CryptoFightClub;




    #region DLL
    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();

    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);

    #endregion

    public void OnLogin()
    {

#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            panelConnecting.SetActive(true);
            Web3Connect();
            OnConnected();
        }
        catch (Exception e)
        {
            OnFailToSignIn(e.Message);
            Debug.LogException(e, this);
        }
#else

        if(!string.IsNullOrEmpty(_account))
            OnEnter(_account);

#endif

    }

    async private void OnConnected()
    {
        Debug.Log("OnConnected");

#if UNITY_WEBGL

        await new WaitForSeconds(0.75f);

        try
        {
            _account = ConnectAccount();
            while (_account == "")
            {
                await new WaitForSeconds(1.00f);
                _account = ConnectAccount();
            };

            // reset login message
            SetConnectAccount("");
            // load next scene
            Api_CryptoFightClub.OnSignIn(OnEnter, OnFailToSignIn);
        }
        catch (Exception e)
        {
            OnFailToSignIn(e.Message);
        }

#endif

    }


    private string ConvertIdMetaMask(string value)
    {
        //Ex value = account0x846b257a244141ecb5c65d7c8a122a72a5564c38

        if (value.Contains("account"))
        {
            return value.Remove(0, 7);
        }
        else
        {
            return value;
        }
    }

    public void OnGuest()
    {
        //panelTryOut.SetActive(true);

        //Versão com todos os personagens
        try
        {
            TextAsset targetFile = Resources.Load<TextAsset>("Json/AllAccounts");

            if (targetFile != null)
            {
                var json = "{ \"accounts\": " + targetFile.text + "}" ;
                Data_Manager.Instance.StartAccount(json, OnSuccessToSingIn, OnFailToSignIn);
            }

        }
        catch (Exception e)
        {
            OnFailToSignIn(e.Message);
        }
    }

    public void OnEnter(string contract)
    {
        try
        {
            gameplayView.instance.usingMeta = true;
            Data_Manager.Instance.contractId = contract;
            Debug.Log("contract -> " + contract);
            Data_Manager.Instance.accountId = ConvertIdMetaMask(_account);
            PlayerPrefs.SetString("account", Data_Manager.Instance.accountId);
            Debug.Log("account -> " + _account);
            //Tratar o ID do metaMask
            Api_CryptoFightClub.GetAccount(null, OnFailToSignIn);

        }
        catch (Exception e)
        {
            OnFailToSignIn(e.Message);
        }
    }

    public void OnSuccessToSingIn()
    {
        panelConnecting.SetActive(false);
        panelStart.SetActive(false);
        panelError.SetActive(false);
        panelSelection.SetActive(true);
        BGM_Manager.Instance.PlaySong();
       

    }

    public void OnFailToSignIn(string error)
    {
        Debug.Log(error);
        textError.text = error;

        panelConnecting.SetActive(false);
        panelSelection.SetActive(false);
        panelStart.SetActive(true);
        panelError.SetActive(true);
    }
}

