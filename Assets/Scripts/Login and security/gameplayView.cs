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
    
    public JuiceDisplayScript juiceDisplay;
    public bool logedin=false;
    public Account[] currentNFTs;
    public API_PlayfabMatchmaking apiPlayfab;
    public string TempSessionID;
    public string equipedWearables;
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
        //for testing
        //SetRandomWearables();
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

    public int bonusAtt;
    public int bonusDef;
    public int bonusTek;

    [SerializeField]
    private LockerRoomAPI lockerRoomApi;

    public void GetEqippedWearables()
    {
        bool isFirst = true;
        bonusAtt = 0;
        bonusDef = 0;
        bonusTek = 0;
        wearablesEqipped = "";

        if(wearableReply.num != 0)
        {
            foreach (Wearable wearable in wearableReply.wearables)
            {
                if (wearable.is_equiped == "True" || wearable.is_equiped == "true")
                {
                    if (isFirst)
                    {
                        if(lockerRoomApi.wearableDatabase.GetSlot(wearable.sku) != "Extras")
                        {
                            wearablesEqipped += lockerRoomApi.wearableDatabase.GetSlot(wearable.sku) + "_" + lockerRoomApi.wearableDatabase.GetSlug(wearable.sku);
                        }
                        else
                        {
                            wearablesEqipped += lockerRoomApi.wearableDatabase.GetType(wearable.sku) + "_" + lockerRoomApi.wearableDatabase.GetSlug(wearable.sku);
                        }

                        isFirst = false;
                    }
                    else
                    {
                        if (lockerRoomApi.wearableDatabase.GetSlot(wearable.sku) != "Extras")
                        {
                            wearablesEqipped += "," + lockerRoomApi.wearableDatabase.GetSlot(wearable.sku) + "_" + lockerRoomApi.wearableDatabase.GetSlug(wearable.sku);
                        }
                        else
                        {
                            wearablesEqipped += "," + lockerRoomApi.wearableDatabase.GetType(wearable.sku) + "_" + lockerRoomApi.wearableDatabase.GetSlug(wearable.sku);
                        }

                        
                    }
                    bonusAtt += lockerRoomApi.wearableDatabase.GetAtk(wearable.sku);
                    bonusDef += lockerRoomApi.wearableDatabase.GetDef(wearable.sku);
                    bonusTek += lockerRoomApi.wearableDatabase.GetTek(wearable.sku);
                }
            }
        }

        equipedWearables = wearablesEqipped;
        Debug.Log(wearablesEqipped);
    }

    #endregion

    #region testFucntion
    public void SetRandomWearables()
    { 
        equipedWearables="Shorts_" + GetQuality()+","+ "Gloves_mediocre";
        Debug.Log(equipedWearables);
    }

    string GetQuality()
    {
        string x1 = "";
        int temp = Random.Range(0, 9);
        switch (temp)
        {
            case 0:
                x1 = "average";
                break;
            case 1:
                x1 = "common";
                break;
            case 2:
                x1 = "epic";
                break;
            case 3:
                x1 = "exotic";
                break;
            case 4:
                x1 = "legendary";
                break;
            case 5:
                x1 = "mediocre";
                break;
            case 6:
                x1 = "ordinary";
                break;
            case 7:
                x1 = "rare";
                break;
            case 8:
                x1 = "super-rare";
                break;
            case 9:
                x1 = "world-champion";
                break;


        }
        return x1;
    }
    #endregion
}
