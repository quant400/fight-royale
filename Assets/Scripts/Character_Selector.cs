﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Character_Selector : MonoBehaviour
{
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _previousButton;
    [SerializeField] private TMP_Text _nameText;
    
    private List<string> _charactersNames;

    void Start()
    {
        _charactersNames = Character_Manager.Instance.GetUnlockedCharacters.Select(auxCharacter => auxCharacter.Name).ToList();

        Character_Manager.Instance.ChangeCharacter(_charactersNames[0]);
        
        SetUpUI();
        UpdateUI();
    }

    private void SetUpUI()
    {
        _nextButton.onClick.AddListener(OnNextButton);
        _previousButton.onClick.AddListener(OnPreviousButton);
    }

    private void UpdateUI()
    {
        _nameText.text = _charactersNames[Character_Manager.Instance.GetUnlockedCurrentCharacterIndex];
    }

    private void OnNextButton()
    {
        int aux = Character_Manager.Instance.GetUnlockedCurrentCharacterIndex + 1;
        if (aux < _charactersNames.Count)
        {
            Character_Manager.Instance.ChangeCharacter(_charactersNames[aux]);
        }
        else
        {
            Character_Manager.Instance.ChangeCharacter(_charactersNames[0]);
        }

        UpdateUI();
        
    }

    private void OnPreviousButton()
    {
        int aux = Character_Manager.Instance.GetUnlockedCurrentCharacterIndex - 1;
        if (aux >= 0)
        {
            Character_Manager.Instance.ChangeCharacter(_charactersNames[aux]);
        }
        else
        {
            Character_Manager.Instance.ChangeCharacter(_charactersNames[_charactersNames.Count-1]);
        }
        
        UpdateUI();

    }
}
