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
    [SerializeField] private GameObject panel_OnSucess;
    [SerializeField] private GameObject panel_OnFail;
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
        Connection_Manager.Instance.SearchAvailableLobby(OnLoading, OnSuccess, OnFail);
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

    void OnFail()
    {
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
