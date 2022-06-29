using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif
using System.Collections;

/// <summary>
///    TestHome serves a game controller object for this application.
/// </summary>
public class AgoraHome : MonoBehaviour
{

    // Use this for initialization
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif

    public static AgoraHome Instance;
    public static AgoraHomeUnityVideo app = null;
    [SerializeField]
    private string PlaySceneName = "AgoraHomeUnityVideo";

    // PLEASE KEEP THIS App ID IN SAFE PLACE
    // Get your own App ID at https://dashboard.agora.io/
    [SerializeField]
    private string AppID = null;
    [SerializeField]
    private TMP_InputField InputChannel;

    private bool onVideo = true;

    private string ChannelName => InputChannel.text;


    void Awake()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
		permissionList.Add(Permission.Microphone);         
		permissionList.Add(Permission.Camera);               
#endif
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Update()
    {
        CheckPermissions();
    }
    
    /// <summary>
    ///   Checks for platform dependent permissions.
    /// </summary>
    private void CheckPermissions()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        foreach(string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {                 
				Permission.RequestUserPermission(permission);
			}
        }
#endif
    }


    public void onJoin(bool enableVideo)
    {
        onVideo = enableVideo;
        onJoinButtonClicked(enableVideo, ChannelName);
    }
    
    public void onJoin(bool enableVideo, string channelName)
    {
        onVideo = enableVideo;
        onJoinButtonClicked(enableVideo, channelName);
    }

    private void onJoinButtonClicked(bool enableVideo, string channelName, bool muted = false)
    {   
        // create app if nonexistent
        if (ReferenceEquals(app, null))
        {
            app = new AgoraHomeUnityVideo(); // create app
            app.loadEngine(AppID); // load engine
            Debug.Log("loadEngine");
        }

        // join channel and jump to next scene
        Debug.Log("AppID " + AppID);
        Debug.Log("ChannelName " + channelName);
      
        app.join(channelName, enableVideo, muted);

        SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
        SceneManager.LoadScene(PlaySceneName, LoadSceneMode.Additive);
    }

    public void onLeaveButtonClicked()
    {
        if (!ReferenceEquals(app, null))
        {
            try
            {
                app.leave(); // leave channel
                app.unloadEngine(); // delete engine
                app = null; // delete app
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
             
        }
        //Destroy(gameObject);
    }

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //mode = LoadSceneMode.Additive;

        if (scene.name == PlaySceneName)
        {
            if (!ReferenceEquals(app, null))
            {
                Debug.Log("OnLevelFinishedLoading");
                app.onSceneHelloVideoLoaded(onVideo); // call this after scene is loaded
            }
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }
    }

    void OnApplicationPause(bool paused)
    {
        if (!ReferenceEquals(app, null))
        {
            app.EnableVideo(paused);
        }
    }

    void OnApplicationQuit()
    {
        if (!ReferenceEquals(app, null))
        {
            app.unloadEngine();
        }
    }
}
