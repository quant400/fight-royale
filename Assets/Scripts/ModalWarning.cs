using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalWarning : MonoBehaviour
{
    public static ModalWarning Instance;

    [Header("Components")]
    [SerializeField] private TMP_Text textTitle;
    [SerializeField] private TMP_Text textDescription;
    [SerializeField] private Button buttonConfirm;
    [SerializeField] private Button buttonBGPanel;
    [SerializeField] private GameObject panel;

    void Awake()
    {
        if (Instance == null) Instance = this;

        panel.SetActive(false);

        buttonBGPanel.onClick.AddListener(Close);
    }

    public void Show(string description, UnityAction confirmAction = null)
    {
        textDescription.text = description;
        buttonConfirm.onClick.AddListener(confirmAction ?? Close);
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
    }


}
