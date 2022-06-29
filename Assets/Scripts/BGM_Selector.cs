using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BGM_Selector : MonoBehaviour
{
    [Header("BGM")]
    [SerializeField] private BGM_Manager _Bgm;

    [Header("UI")]
    [SerializeField] private Button _previousButton;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Button _nextButton;
  


    void Awake()
    {
        if(_Bgm == null)
        {
            _Bgm = BGM_Manager.Instance;
        }

        SetUpUI();
        SetText();
    }

    private void SetUpUI()
    {
        _nextButton.onClick.AddListener(OnNextButton);
        _previousButton.onClick.AddListener(OnPreviousButton);
    }


    private void OnNextButton()
    {
        _Bgm.Next();
        SetText();
    }

    private void OnPreviousButton()
    {
        _Bgm.Previous();
        SetText();
    }

    private void SetText()
    {
        _nameText.text = _Bgm.GetNameSong();
    }

}
