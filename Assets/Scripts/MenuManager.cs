using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("UI MENU")]
    [SerializeField] private Button Button_LevelReset;

    [Header("UI TUTORIAL")]
    [SerializeField] private GameObject GO_Tutorial;

    [Header("UI K.O")]
    [SerializeField] private GameObject GO_KO;
    [SerializeField] Button Button_KO;

    [Header("UI Time")]
    [SerializeField] private GameObject GO_Time;
    [SerializeField] private TMP_Text Text_Time;

    public enum ThemeTime
    {
        Keep,
        Default,
        Critical,
        Alert,
    }

    void Awake()
    {
        if (Instance == null)
        {
            //DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Button_LevelReset.onClick.AddListener(OnReset);
        Button_KO.onClick.AddListener(OnReset);

    }

    public void Reset() => OnReset();
    public void ShowTutorial(bool show = true)
    {
        if (GO_Tutorial != null)
        {
            GO_Tutorial.SetActive(show);
            
            if(show)
                StartCoroutine(WaitLoading(() => { GO_Tutorial.SetActive(false); }, 6.00f));
        }
    }
    public void ShowKO(bool show = true)
    {
        GO_KO.SetActive(show);
    }
    private void OnReset()
    {
        ShowKO(false);
        CFCNetworkManager.singleton.StopClient();
        SceneManager.LoadScene("Game");
    }
    private void OpenLink(string url)
    {
        float startTime = Time.timeSinceLevelLoad;
        if (Time.timeSinceLevelLoad - startTime <= 1f)
        {
            Application.OpenURL(url);
        }
    }
    private IEnumerator WaitLoading(Action onFinish, float time = 0.10f)
    {
        yield return new WaitForSeconds(time);
        onFinish();
    }

    public void ShowTime(bool showTime)
    {
        GO_Time.SetActive(showTime);
    }

    public void CustomTime(string newText, ThemeTime themeTime)
    {
        switch (themeTime)
        {
            case ThemeTime.Keep:
                Text_Time.text = newText;
                break;
            case ThemeTime.Default:
                Text_Time.color = Color.white;
                Text_Time.fontSize = 86.0f;
                Text_Time.text = newText;
                break;
            case ThemeTime.Critical:
                Text_Time.color = Color.red;
                Text_Time.fontSize = 112.0f;
                Text_Time.text = newText;
                break;
            case ThemeTime.Alert:
                Text_Time.color = Color.yellow;
                Text_Time.fontSize = 52.0f;
                Text_Time.text = newText;
                break;

        }

    }



}



