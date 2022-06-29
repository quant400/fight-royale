using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CFC
{
    public class Contact_Component : MonoBehaviour
    {
        private string _player_id;
        [SerializeField]private Image _avatarImage;
        [SerializeField]private Button _contactButton;

        public void SetUp(string player_id, Sprite avatar, Action<string> onContactClick)
        {
            _avatarImage.sprite = avatar;
            _contactButton.onClick.AddListener(()=>onContactClick(player_id));
        }
    }
}

