using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CFC
{
    public class ChatBox_Manager : MonoBehaviour
    {
        public static ChatBox_Manager Instance;

        [Header("Components")]
        [SerializeField] private Transform _chatContent;
        [SerializeField] private Transform _contactsContent;
        
        [SerializeField] private Message_Component _prefabMessage;
        [SerializeField] private Contact_Component _prefabContact;
        
        [Header("UI")]
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Button _sendButton;

        private List<Chat> _chats = new List<Chat>();
        private List<Contact_Component> _contacts = new List<Contact_Component>();

        public enum ChatType
        {
            Writer,
            Receiver
        };
    
        void Awake()
        {
            
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            SetUpUI();
        }
        
        public void AddChat(string writerId, string receiverId)
        {
            _chats.Add(new Chat(
                    $"{writerId}:{receiverId}",
                    writerId,
                    receiverId
                ));

            UpdateContactList();
        }

        private void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                //string receiver_id = Multiplayer.NetworkManager.Instance.networkPlayers.Last().Value.id;
                //Multiplayer.NetworkManager.Instance.EmitOpenChatBox(receiver_id);
            }
        }

        #region UI

        private void SetUpUI()
        {
            _inputField.onSubmit.AddListener((text)=>OnMessageSend());
            _sendButton.onClick.AddListener(OnMessageSend);
            
            Hide();
        }
        
        private void UpdateContactList()
        {
            foreach (Transform child in _contactsContent.transform) {
                Destroy(child.gameObject);
            }
            
            foreach (var chat in _chats)
            {
                var contact = Instantiate(_prefabContact, _contactsContent);
                contact.SetUp(chat.receiverId, null, OpenChat);
            }
        }

        private void OpenChat(string receiverId)
        {
            //_nameText.text = Multiplayer.NetworkManager.Instance.networkPlayers[receiverId].name;
            Show();
        }

        public void Show()
        {
            SetVisibility(true);
        }
        
        public void Hide()
        {
            SetVisibility(false);
        }

        private void SetVisibility(bool isVisible)
        {
            transform.GetChild(0).gameObject.SetActive(isVisible);
        }

        private void OnMessageSend()
        {
            CreateMessage("MeuId", _inputField.text, ChatType.Writer);
        }
        
        private void OnMessageReceived(string playerId, string message)
        {
            CreateMessage(playerId, message, ChatType.Receiver);
        }
        
        public void CreateMessage(string playerId, string message, ChatType chatType)
        {
            Message_Component messageComponent = Instantiate(_prefabMessage, _chatContent);
            messageComponent.SetUp(GetPlayerNameById(playerId), message, chatType, GetColorById(playerId));
        }
        
        #endregion

        #region Utils

        private string GetPlayerNameById(string playerId)
        {
            //TODO: Programar essa função
            return "Test";
        }
    
        private Color GetColorById(string playerId)
        {
            //TODO: Programar essa função
            return Color.cyan;
        }
        
        #endregion
        
    }
}


