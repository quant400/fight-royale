using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public struct MintRequest
{
    public string id;
    public string slot;
}

public struct MintReply
{
    public bool status;
    public DataWearable data;
}
[System.Serializable]
public struct DataWearable
{
    public int id;
    public int sku;
    public int health;
    public bool is_equiped;
}

public struct GetWearable
{
    public string id;
}

[System.Serializable]
public struct Wearable
{
    public int id;

    public int sku;

    public string is_equiped;

    public int health;
}


public struct WearableReply
{
    public int num;

    public Wearable[] wearables;
}



public struct EquipWearable
{
    public string id;
}

public struct EquipWearableReply
{
    public bool status;
}


public class LockerRoomAPI : MonoBehaviour
{
    [SerializeField]
    private LockerRoomManager lockerRoomManager;

    private const string CSV_FILE_PATH = "CSV/WearableDatabase";

    public WearableDatabaseReader wearableDatabase;

    [SerializeField]
    private GameObject lockerRoomObject;
    [SerializeField]
    private Button playButton;

    // Start is called before the first frame update
    void Start()
    {
        //MintWearable("hassan.iqbal@quids.tech$$$0tuicf75vgosrhtbpywalketugg2", "SHORTS");

        //MintWearable("175", "SHORTS");

        //GetWearables(gameplayView.instance.currentNFTs[0].id);

        wearableDatabase = new WearableDatabaseReader();
        wearableDatabase.LoadData(CSV_FILE_PATH);
        MintWearable("175", "asd");
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void MintWearable(string assetId, string assetSlot)
    {
        //StartCoroutine(GetMint(assetId, assetSlot));

        StartCoroutine(GetMint(assetId, RandomSlug()));
    }

    public void GetWearables(string assetId, bool isLockerRoomManager)
    {
        StartCoroutine(GetWearablesRequest(assetId, isLockerRoomManager));
    }

    public void EquipWearables(string assetId)
    {
        StartCoroutine(EquipWearableRequest(assetId));
    }

    private string RandomSlug()
    {
        string slug = "";

        int temp = Random.Range(0, 6);
        switch (temp)
        {
            case 0:
                slug = "BELTS";
                break;
            case 1:
                slug = "GLOVES";
                break;
            case 2:
                slug = "SHOES";
                break;
            case 3:
                slug = "SHORTS";
                break;
            case 4:
                slug = "EXTRA";
                break;
            case 5:
                slug = "TRAINERS";
                break;
        }

        return slug.ToUpper();
    }


    IEnumerator GetMint(string assetId, string assetSlot)
    {
        string url = "";
        MintRequest mintRequest = new MintRequest();
        mintRequest.id = assetId;
        mintRequest.slot = assetSlot;
        Debug.Log(mintRequest.slot);
        if (KeyMaker.instance.buildType == BuildTypeGame.staging)
            url = "https://staging-api.cryptofightclub.io/game/sdk/wearable/mint";
        else if (KeyMaker.instance.buildType == BuildTypeGame.production)
            url = "https://staging-api.cryptofightclub.io/game/sdk/wearable/mint";

        string idJsonData = JsonUtility.ToJson(mintRequest);

        using (UnityWebRequest request = UnityWebRequest.Put(url, idJsonData))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(idJsonData);
            request.method = "PUT";
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.error == null)
            {
                string result = Encoding.UTF8.GetString(request.downloadHandler.data);
                MintReply r = JsonUtility.FromJson<MintReply>(request.downloadHandler.text);

                gameplayView.instance.mintReply = r;

                Debug.Log("MINT");

                Debug.Log("status: = " + r.status);


                Debug.Log("id: = " + r.data.id);
                Debug.Log("sku: = " + r.data.sku);
                Debug.Log("is_equiped: = " + r.data.is_equiped);
                Debug.Log("health: = " + r.data.health);

                Debug.Log("MINT END");
                /*
                if (KeyMaker.instance.buildType == BuildType.staging)
                    Debug.Log(request.downloadHandler.text);
                if (r.status == "false")
                    gameplayView.instance.juiceDisplay.SetJuiceBal("0");
                else
                    gameplayView.instance.juiceDisplay.SetJuiceBal(r.total);
                */
            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }


    IEnumerator GetWearablesRequest(string assetId, bool isLockerRoomManager)
    {
        string url = "";
        GetWearable getWearable = new GetWearable();
        getWearable.id = assetId;
        if (KeyMaker.instance.buildType == BuildTypeGame.staging)
            url = "https://staging-api.cryptofightclub.io/game/sdk/wearable/get";
        else if (KeyMaker.instance.buildType == BuildTypeGame.production)
            url = "https://staging-api.cryptofightclub.io/game/sdk/wearable/get";

        string idJsonData = JsonUtility.ToJson(getWearable);

        using (UnityWebRequest request = UnityWebRequest.Put(url, idJsonData))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(idJsonData);
            request.method = "PUT";
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.error == null)
            {
                string result = Encoding.UTF8.GetString(request.downloadHandler.data);
                WearableReply r = JsonUtility.FromJson<WearableReply>(request.downloadHandler.text);

                gameplayView.instance.wearableReply = r;
                lockerRoomManager.EnableLeftRight();
                /*
                Debug.Log("GET WEARABLES");
                Debug.Log("num: = " + r.num);

                for (int i = 0; i < r.wearables.Length; i++)
                {
                    Debug.Log("id: = " + r.wearables[i].id);
                    Debug.Log("sku: = " + r.wearables[i].sku);
                    Debug.Log("is_equiped: = " + r.wearables[i].is_equiped);
                    Debug.Log("health: = " + r.wearables[i].health);
                }
                Debug.Log("END GET WEARABLES");
                */
                if (isLockerRoomManager)
                {
                    lockerRoomManager.currentCharacter.wearablesData = r;

                    lockerRoomManager.GetWearablesModelArray();

                    lockerRoomManager.CaculateTotalAttrbutes();
                }

                gameplayView.instance.GetEqippedWearables();

            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }

    IEnumerator EquipWearableRequest(string assetId)
    {
        string url = "";
        EquipWearable equipWearable = new EquipWearable();
        equipWearable.id = assetId;
        if (KeyMaker.instance.buildType == BuildTypeGame.staging)
            url = "https://staging-api.cryptofightclub.io/game/sdk/wearable/equip";
        else if (KeyMaker.instance.buildType == BuildTypeGame.production)
            url = "https://staging-api.cryptofightclub.io/game/sdk/wearable/equip";

        string idJsonData = JsonUtility.ToJson(equipWearable);

        using (UnityWebRequest request = UnityWebRequest.Put(url, idJsonData))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(idJsonData);
            request.method = "PUT";
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.error == null)
            {
                string result = Encoding.UTF8.GetString(request.downloadHandler.data);
                EquipWearableReply r = JsonUtility.FromJson<EquipWearableReply>(request.downloadHandler.text);

                gameplayView.instance.equipWearableReply = r;

                Debug.Log("status: = " + r.status);
                /*
                if (KeyMaker.instance.buildType == BuildType.staging)
                    Debug.Log(request.downloadHandler.text);
                if (r.status == "false")
                    gameplayView.instance.juiceDisplay.SetJuiceBal("0");
                else
                    gameplayView.instance.juiceDisplay.SetJuiceBal(r.total);
                */
            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }

    public void BackToCharacterScreen()
    {
        lockerRoomObject.SetActive(false);

        playButton.interactable = false;
    }
}
