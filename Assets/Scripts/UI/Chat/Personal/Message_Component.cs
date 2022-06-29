using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CFC
{
    public class Message_Component : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField]private Image _avatarContainer;
        [SerializeField]private TMP_InputField _inputField;
        [SerializeField]private Image _backGroundImage;

        public void SetUp(string name, string message, ChatBox_Manager.ChatType chatType, Color color)
        {
            _inputField.text = $"{name}/n{message}";
            _backGroundImage.color = color;

            switch (chatType)
            {
                case ChatBox_Manager.ChatType.Writer:
                    _inputField.transform.SetAsFirstSibling();
                    break;
            
                case ChatBox_Manager.ChatType.Receiver:
                    _inputField.transform.SetAsLastSibling();
                    break;
            }
        }
    }
}


