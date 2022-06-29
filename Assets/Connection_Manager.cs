using System.Collections;
using System.Collections.Generic;
using CFC.Serializable.GetEntityToken;
using CFC.Serializable.ListMultiplayerServers;
using UnityEngine;

public class Connection_Manager : MonoBehaviour
{
    public static Connection_Manager Instance;
    public CustomWalletLogin WalletLogin;
    public PlayfabMatchmaking PlayfabMatchmaking;

    public string ServerIp => PlayfabMatchmaking.CurrentMultiplayerServerSummary.IPV4Address;
    public ushort ServerPort => (ushort)PlayfabMatchmaking.CurrentMultiplayerServerSummary.Ports[0].Num;
    public string BuildId => PlayfabMatchmaking.BuildId;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (Instance == null)
        {
            Instance = this;
        }

    }


    private void Start()
    {
        PlayfabMatchmaking.GetEntityToken(OnSuccessEntityToken, OnFail);
    }

    private void OnFail(string obj)
    {
        Debug.Log(obj);
    }

    private void OnSuccessEntityToken(string result)
    {
        PlayfabMatchmaking.ListMultiplayerServers(() => { Debug.Log("OnSuccess"); }, OnFail);
    }


}
