using System.Collections;
using System.Collections.Generic;
using CFC.Chatt.Global;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatGlobal_Manager : MonoBehaviour
{

    public static ChatGlobal_Manager Instance;
    
    [SerializeField]private Message_Component _prefabGlobalMessage;
    [SerializeField] private Transform _contentGlobalChat;

    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _buttonSend;

    private List<Message_Component> _messages = new List<Message_Component>();

    public PlayerBehaviour player;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        SetUpUI();
    }

    private void SetUpUI()
    {
        _inputField.onSubmit.AddListener((text)=>SendMessage());
        _buttonSend.onClick.AddListener(SendMessage);
    }
    
    private void SendMessage()
    {
        if(string.IsNullOrEmpty((_inputField.text)))return;

        var text = _inputField.text;
        text = text.Replace("\n", " ");
    
        player.SendMessage(player.netIdentity, player.pColor, text);

        _inputField.text = "";
    }
    
    public void CreateMessage(NetworkIdentity playerIdentity, Color32 color, string message)
    {
        /* Message_Component messageComponent = Instantiate(_prefabGlobalMessage, _contentGlobalChat);
         _messages.Add(messageComponent);
         messageComponent.SetUp(playerIdentity, message, color);*/
        IngameUIControler.instance.AddChat(playerIdentity, message);
    }
}
