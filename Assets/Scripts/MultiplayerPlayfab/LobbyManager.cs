using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private ClientStartUp clientStartUp;
    [SerializeField] private CFCAuth _auth;

    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button loginButton;
    
    [SerializeField] private Configuration config;

    void Start()
    {
        loginButton.onClick.AddListener(() => OnLogin(nameInputField.text));
        if (config.buildType != BuildType.REMOTE_SERVER && config.buildType != BuildType.LOCAL_SERVER)
                OnLogin(Character_Manager.Instance.GetCurrentCharacter.Name);
    }

    void OnLogin(string name)
    {
        _auth.playerName = name;
        _auth.playerId = Data_Manager.Instance.currentNftId;
        _auth.walletNfts = //Data_Manager.Instance.GetWalletNfts();//
                           Data_Manager.Instance.isValidAccount()? Data_Manager.Instance.GetWalletNfts() : "Demo";
        clientStartUp.OnLoginUserButtonClick();
    }
}
