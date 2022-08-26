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

    public void SetName(string name)
    {
        _playerName.text = name;
    }

    public void SetHealth(float health)
    {
        health = (int)Math.Round(health);
        _healthValue.text = health + "%";
        _healthBar.fillAmount = health / 100;

        //for updating mini ui 
        IngameUIControler.instance.UpdatePlayerHealth(GetComponent<PlayerBehaviour>().netIdentity, health);
    }

    public void SetHealthBar(Image newBar)
    {
        _healthBar = newBar;
    }
}
