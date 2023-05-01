using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LockerRoomAPI : MonoBehaviour
{
    public struct MintRequest
    {
        public string id;
        public string slot;
    }

    public struct MintReply
    {
        public bool status;
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


    // Start is called before the first frame update
    void Start()
    {
        //MintWearable("hassan.iqbal@quids.tech$$$0tuicf75vgosrhtbpywalketugg2", "SHORTS");

        //MintWearable("175", "SHORTS");

        GetWearables(gameplayView.instance.currentNFTs[0].id);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MintWearable(string assetId, string assetSlot)
    {
        StartCoroutine(GetMint(assetId, assetSlot));
    }

    public void GetWearables(string assetId)
    {
        StartCoroutine(GetWearablesRequest(assetId));
    }

    public void EquipWearables(string assetId)
    {
        StartCoroutine(EquipWearableRequest(assetId));
    }

    IEnumerator GetMint(string assetId, string assetSlot)
    {
        string url = "";
        MintRequest mintRequest = new MintRequest();
        mintRequest.id = assetId;
        mintRequest.slot = assetSlot;
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


    IEnumerator GetWearablesRequest(string assetId)
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
            request.method = "GET";
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.error == null)
            {
                string result = Encoding.UTF8.GetString(request.downloadHandler.data);
                WearableReply r = JsonUtility.FromJson<WearableReply>(request.downloadHandler.text);

                Debug.Log("num: = " + r.num);

                for (int i = 0; i < r.wearables.Length; i++)
                {
                    Debug.Log("id: = " + r.wearables[i].id);
                    Debug.Log("name: = " + r.wearables[i].sku);
                    Debug.Log("is_equiped: = " + r.wearables[i].is_equiped);
                    Debug.Log("health: = " + r.wearables[i].health);
                }
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
}
