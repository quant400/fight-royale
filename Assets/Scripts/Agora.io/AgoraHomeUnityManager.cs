using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AgoraHomeUnityManager : MonoBehaviour
{

    public static AgoraHomeUnityManager Instance;

    private AgoraHome agoraHome = null;

    public GameObject panelGlobalVideo;
    public GameObject friendVideoPanel;
    public GameObject yourVideoPanel;
    
    public Text msgText;


    public void Awake()
    {
        Instance = this;
        if (agoraHome == null)
        {
            agoraHome = GameObject.Find("AgoraManager").GetComponent<AgoraHome>();
        }
    }

    public void OpenVideo()
    {
        agoraHome.onJoin(true, msgText.text+"Video");
        Leave();
    }

    public void Leave()
    {
        agoraHome.onLeaveButtonClicked();

        Debug.Log("Exit Scene ->" + this.gameObject.scene.name);
        SceneManager.UnloadSceneAsync(this.gameObject.scene);
    }
}
