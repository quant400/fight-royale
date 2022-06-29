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

    void Awake()
    {
        loginButton.onClick.AddListener(OnLogin);
    }

    void OnLogin()
    {
        _auth.playerName = nameInputField.text;
        clientStartUp.OnLoginUserButtonClick();
    }
}
