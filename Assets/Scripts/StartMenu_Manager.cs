using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class StartMenu_Manager : MonoBehaviour
{
    [Header("UI")]

    [SerializeField] private GameObject panel_OnLoading;
    [SerializeField] private TMP_Text text_OnLoading;
    
    [SerializeField] private GameObject panel_OnFail;
    [SerializeField] private TMP_Text text_OnFail;

    [SerializeField] private GameObject panel_OnSucess;
    [SerializeField] private Button select_Button;

    [Header("Components")]
    [SerializeField] private Skin_Controller skinCharacter;
    [SerializeField] private TMP_Text text_Version;
    private const string _gameScene = "Game";
    
    void Awake()
    {
        SetUpUI();
    }

    void Start()
    {
        text_Version.text = "Version - " + Application.version;
        skinCharacter.SetUpSkin();
    }

    private void SetUpUI()
    {
        select_Button.onClick.AddListener(OnSelectClick);
    }

    void OnSelectClick()
    {
        select_Button.enabled = false;
        var NFT_ID = Data_Manager.Instance.currentNftId;

        Connection_Manager.Instance.GetPlayerPvpScore(NFT_ID,
            OnSuccessSessions,
            (msg) => 
            {
                OnFail("There was an error verifying your ticket ,please try again");
            }
        );
    }

    private void OnSuccessSessions(CFC.Serializable.PlayerPvpScore playerPvpScore)
    {
        select_Button.enabled = true;

        bool TryOutMode = !Data_Manager.Instance.isValidAccount();

        if (true) //TryOutMode || (playerPvpScore != null && playerPvpScore.dailySessionPlayed < 10))
            Connection_Manager.Instance.SearchAvailableLobby(
                () =>
                {
                    OnSetText("Looking for Room wait...");
                    OnLoading();
                },
                OnSuccess,
                () =>
                {
                    OnSetText("All rooms are full you will be in a queue until some room is freed...");
                });
        else
            OnFail("Your NFT has no PVP Session available");
    }

    void OnSetText(string text)
    {
        text_OnLoading.text = text;
        text_OnLoading.gameObject.SetActive(true);
    }

    void OnLoading()
    {
        panel_OnLoading.SetActive(true);
    }

    void OnSuccess()
    {
        panel_OnLoading.SetActive(false);
        panel_OnSucess.SetActive(true);
        StartCoroutine(LoadScene());
    }

    void OnFail(string msg = null)
    {
        
        if (!string.IsNullOrEmpty(msg))
        {
            text_OnFail.text = msg;    
        }

        select_Button.enabled = true;
        panel_OnSucess.SetActive(false);
        panel_OnLoading.SetActive(false);
        panel_OnFail.SetActive(true);
    }

  
    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1.25f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_gameScene);
        while (!asyncLoad.isDone)
        {
            yield return new WaitForSeconds(0.025f);
        }

    }
}
