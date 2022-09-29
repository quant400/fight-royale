using System;
using System.Collections;
using System.Linq;
using Cinemachine;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("UI MENU")]
    [SerializeField] private Button Button_LevelReset;
    [SerializeField] private Button Button_LevelResetServer;

    [Header("UI TUTORIAL")]
    [SerializeField] private GameObject GO_Tutorial;

    [Header("UI K.O")]
    [SerializeField] private GameObject GO_KO;
    [SerializeField] Button Button_KO;
    [SerializeField] Button Button_CamViewer;
    
    [Header("UI WINNER")]
    [SerializeField] private GameObject GO_Winner;
    [SerializeField] Button Button_Winner;

    [Header("UI TIME")]
    [SerializeField] private GameObject GO_Time;
    [SerializeField] private TMP_Text Text_Time;
    [SerializeField] private TimerManager _timerManager;

    [Header("UI LEADERBOARD")]

    [SerializeField] private Leaderboard_Manager Daily;
    [SerializeField] private Leaderboard_Manager AllTime;


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
        Button_LevelResetServer.onClick.AddListener(OnResetServer);
        Button_KO.onClick.AddListener(OnQuit);
        Button_Winner.onClick.AddListener(OnQuit);
        Button_CamViewer.onClick.AddListener(OnChangeViewer);

    }
    public void ShowLeaderboardsAllTime()
    {
        try
        {

            Connection_Manager.Instance.GetLeaderboardAllTime(()=>
            {
                AllTime.OnCreate(Data_Manager.Instance.leaderboard_AllTime.scoreboards);
            },(erro)=> 
            {
                AllTime.transform.parent.gameObject.SetActive(false);
                Debug.Log(erro); 
            }
            );  
        }
        catch (Exception e)
        {
            Debug.Log("ShowLeaderboardsAllTime -> " +  e);
        }
            
    }

    public void ShowLeaderboardsDaily()
    {
        try
        {
            Connection_Manager.Instance.GetLeaderboardDaily(() =>
            {
                Daily.OnCreate(Data_Manager.Instance.leaderboard_Daily.scoreboards);
            }, (erro) =>
            {
                Daily.transform.parent.gameObject.SetActive(false);
                Debug.Log(erro);
            }
            );
        }
        catch (Exception e)
        {
            Debug.Log("ShowLeaderboardsDaily  -> " + e);
        }

    }

    public void Reset() => OnReset();
    public void ShowTutorial(bool show = true)
    {
        GameManager.Instance.timerBehavior.gameObject.SetActive(true);
        if (GO_Tutorial != null)
        {
            GO_Tutorial.SetActive(show);
            
            if(show)
                StartCoroutine(WaitLoading(() => { GO_Tutorial.SetActive(false); }, 6.00f));
        }
    }
    public void ShowKO(bool show = true)
    {
        
        currentCam = 0;
        GO_KO.SetActive(show);
    }
    public void ShowWinner(bool show = true)
    {
        GO_Winner.SetActive(show);
    }


    private int currentCam;
    private void OnChangeViewer()
    {
        try
        {
            var listCamTarget = GameObject.FindGameObjectsWithTag("CinemachineTarget").ToList();

            if(listCamTarget != null && listCamTarget.Count > 0)
            {
                if(currentCam >= listCamTarget.Count)
                {
                    currentCam = 0;
                }

                Camera_Manager.Instance.followCam.Follow = listCamTarget[currentCam].transform;
                currentCam++;
            }
        }
        catch (Exception e)
        {
            currentCam = 0;
            Debug.Log("OnChangeViewer " + e.Message);
        }
    }
    private void OnReset()
    {
        ShowKO(false);
        CFCNetworkManager.singleton.StopClient();
        SceneManager.LoadScene("Game");
    }
    private void OnQuit()
    {
        ShowWinner(false);
        CFCNetworkManager.singleton.StopClient();
        SceneManager.LoadScene("Menu");
    }
    
    private void OnResetServer()
    {
        var localPlayer = FindObjectsOfType<PlayerBehaviour>().First(aux=> aux.isLocalPlayer);
        localPlayer.ResetServer();
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



