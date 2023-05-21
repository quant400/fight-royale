using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class StartMenu_Manager : MonoBehaviourPunCallbacks
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

    private int RoomSize = 4; //CFCNetworkManager.Instance.maxConnections;

    void Awake()
    {
        SetUpUI();
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
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

        PhotonNetwork.JoinRandomRoom();

        //if (true) //TryOutMode || (playerPvpScore != null && playerPvpScore.dailySessionPlayed < 10))
        //    Connection_Manager.Instance.SearchAvailableLobby(
        //        () =>
        //        {
        //            OnSetText("Loading... Please wait...".ToUpper());
        //            OnLoading();
        //        },
        //        OnSuccess,
        //        () =>
        //        {
        //            OnSetText("All rooms are full you will be in a queue until some room is freed...".ToUpper());
        //        });
        //else
        //    OnFail("Your NFT has no PVP Session available");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined");
        base.OnJoinedRoom();
        PhotonNetwork.AutomaticallySyncScene = true;
        OnSetText("Loading... Please wait...".ToUpper());
        OnLoading();
        OnSuccess();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room");
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("Creating room");
        int randomRoomNumber = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)RoomSize };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps);
        Debug.Log(randomRoomNumber);
        PlayerPrefs.SetInt("room", randomRoomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room.. trying again");
        CreateRoom();
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
