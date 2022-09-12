using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CFC.Chatt.Global
{
    public class Message_Component : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textMessage;

        public void SetUp(NetworkIdentity netIdentity, string message, Color color)
        {
            _textMessage.text = $"{netIdentity.GetComponent<PlayerBehaviour>().pName}: {message}";
            _textMessage.color = color;
        }
    }
}


