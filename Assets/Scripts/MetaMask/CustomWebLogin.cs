using System;
using System.Runtime.InteropServices;
using UnityEngine;


public class CustomWebLogin : MonoBehaviour
{


    [Header("Start")]
    [SerializeField]
    private GameObject panelStart;
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

    private string _account;

    private API_CryptoFightClub Api_CryptoFightClub => Connection_Manager.Instance.Api_CryptoFightClub;


#if UNITY_WEBGL

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

    }

    async private void OnConnected()
    {
        Debug.Log("OnConnected");
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

    }

#endif
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
        //Versão com conta base
        //_account = "account0x846b257a244141ecb5c65d7c8a122a72a5564c38";
        //OnEnter("Guest");
        //return;

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

    private void OnEnter(string contract)
    {
        try
        {
            Data_Manager.Instance.contractId = contract;
            Debug.Log("contract -> " + contract);
            Data_Manager.Instance.accountId = ConvertIdMetaMask(_account);
            Debug.Log("account -> " + _account);

            //Tratar o ID do metaMask
            Api_CryptoFightClub.GetAccount((json) =>
            {
                Data_Manager.Instance.StartAccount(json, OnSuccessToSingIn, OnFailToSignIn);
            }
                , OnFailToSignIn);

        }
        catch (Exception e)
        {
            OnFailToSignIn(e.Message);
        }
    }

    private void OnSuccessToSingIn()
    {
        panelConnecting.SetActive(false);
        panelStart.SetActive(false);
        panelError.SetActive(false);
        panelSelection.SetActive(true);
        BGM_Manager.Instance.PlaySong();

    }

    private void OnFailToSignIn(string error)
    {
        Debug.Log(error);
        textError.text = error;

        panelConnecting.SetActive(false);
        panelSelection.SetActive(false);
        panelStart.SetActive(true);
        panelError.SetActive(true);
    }
}

