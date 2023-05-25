using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private TMP_Text _healthValue;
    [SerializeField] private Image _healthBar;
    //[SerializeField] private Image _damageEffect;

    public void SetName(string name)
    {
        _playerName.text = name;
    }

    public void SetHealth(float health)
    {
        health = (int)Math.Round(health);
        _healthValue.text = health + "%";
        _healthBar.fillAmount = health / 100;
        //_damageEffect.color = new Vector4(_damageEffect.color.r, _damageEffect.color.g, _damageEffect.color.b,1-(health / 100));

        //for updating mini ui 
        IngameUIControler.instance.UpdatePlayerHealth(GetComponent<PlayerBehaviour>().GetComponent<PhotonView>(), health / 100);
    }

    public void SetHealthBar(Image newBar)
    {
        _healthBar = newBar;
    }
}
