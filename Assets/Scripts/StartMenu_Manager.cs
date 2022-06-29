using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class StartMenu_Manager : MonoBehaviour
{
    [SerializeField] private Button select_Button;
    [SerializeField] private GameObject loading_GO;
    [SerializeField] private GameObject alert_GO;
    [SerializeField] private Skin_Controller skinCharacter;
    private  const string _gameScene = "Game";
    
    void Awake()
    {
        SetUpUI();
    }

    void Start()
    {
        skinCharacter.SetUpSkin();
    }

    private void SetUpUI()
    {
        
        select_Button.onClick.AddListener(OnSelectClick);
    }

    void OnSelectClick()
    {
        OpenGame();
    }

    void OpenGame()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        loading_GO.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_gameScene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        loading_GO.SetActive(false);
    }

}
