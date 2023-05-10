using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CFC.Serializable;

public class gameplayView : MonoBehaviour
{
    public static gameplayView instance;

    public bool usingFreemint = false;

    public bool usingMeta = false;

    public (string, string) logedPlayer;

    public characterSelectionView csv;
    public CustomWebLogin cWL;
    public FireBaseWebGLAuth webGLAuth;
    public GameObject buttonsToEnableAftrLogin, buttonsToDisableAftrLogin;

    public bool logedin=false;
    public Account[] currentNFTs;
    public API_PlayfabMatchmaking apiPlayfab;
    public string TempSessionID;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
            return;
        }
        
        DontDestroyOnLoad(this);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += GetRefrences;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= GetRefrences;
    }

    public string GetLoggedPlayerString()
    {
        if (usingMeta)
            return Data_Manager.Instance.accountId; //PlayerPrefs.GetString("Account");
        else
            return logedPlayer.Item1 + "$$$" + logedPlayer.Item2;
    }


    public void RequestStartSession()
    {
        StartCoroutine(KeyMaker.instance.startSessionApi(Data_Manager.Instance.currentNftId));
    }

    public void RequestEndSession(int score,int kills)
    {
        StartCoroutine(KeyMaker.instance.endSessionApi(Data_Manager.Instance.currentNftId,score,kills));
    }


    void GetRefrences(Scene scene, LoadSceneMode mode)
    {
        if(scene.name=="Menu")
        {
            RefrenceHolder r= GameObject.FindGameObjectWithTag("RefHolder").GetComponent<RefrenceHolder>();
            if (csv == null)
                csv = r.CVS;
            if (cWL == null)
                cWL = r.CWL;
            if (webGLAuth == null)
                webGLAuth = r.Webgl;
            if (buttonsToDisableAftrLogin == null)
                buttonsToDisableAftrLogin = r.DAL; 
            if (buttonsToEnableAftrLogin == null)
                buttonsToEnableAftrLogin = r.EAL;

            if (logedin)
            {
                buttonsToDisableAftrLogin.SetActive(false);
                buttonsToEnableAftrLogin.SetActive(true);
                csv.gameObject.SetActive(true);
                csv.Display(currentNFTs);
            }
        }

       
    }

    #region LockerRoom

    public WearableReply wearableReply;

    public MintReply mintReply;

    public EquipWearableReply equipWearableReply;

    public string wearablesEqipped = "";

    [SerializeField]
    private LockerRoomAPI lockerRoomApi;

    public void GetEqippedWearables()
    {
        bool isFirst = true;

        wearablesEqipped = "";

        foreach (Wearable wearable in wearableReply.wearables)
        {
            if (wearable.is_equiped == "True" || wearable.is_equiped == "true")
            {
                if(isFirst)
                {
                    wearablesEqipped += lockerRoomApi.wearableDatabase.GetSlot(wearable.sku) + "_" + lockerRoomApi.wearableDatabase.GetSlug(wearable.sku);

                    isFirst = false;
                }
                else
                {
                    wearablesEqipped += "," + lockerRoomApi.wearableDatabase.GetSlot(wearable.sku) + "_" + lockerRoomApi.wearableDatabase.GetSlug(wearable.sku);
                }
                
            }
        }

        Debug.Log(wearablesEqipped);
    }

    #endregion
}
