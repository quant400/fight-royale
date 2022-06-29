using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CFC.Chatt.Global
{
    public class Message_Component : MonoBehaviour
    {
        [SerializeField] private TMP_Text _textMessage;

        public void SetUp(string name, string message, Color color)
        {
            _textMessage.text = $"{name}: {message}";
            _textMessage.color = color;
        }
    }
}


