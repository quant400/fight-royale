using CFC.Serializable;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
public enum BuildTypeGame
{
    staging,
    production
}
public struct ConnectObj
{
    public string address;
    public string r;
    public string g;
    public string b;
}

struct startObj
{
    public string id;
    public string address;
}
struct GameEndObj
{
    public string id;
    public string address;
    public string score;
    public string kills;
    public string r;
    public string g;
    public string b;
}
public struct ResponseObject
{
    public Account[] nfts;
    public string code;
}
public class KeyMaker : MonoBehaviour
{
    public static KeyMaker instance;

    String[] masterKeys = {
    "18fb4964-5b5e-48a5-9057-4e9ca70f730c",
    "407d0378-91a3-4756-99d4-8faf861004f2",
    "3ce01623-bbb8-43a5-922b-9512044eb094",
    "d85626f5-cb88-47db-865d-01d8303cea08",
    "5cd3c30a-a7c4-4d57-b5a3-fd21c95b31ec",
    "ff1de2eb-c3a8-4206-bd6e-d2e4abe46299",
    "1a6f79fb-f24c-41b2-b838-02ea97652de4",
    "7a145394-ad54-4317-826d-ce049a5f7ff3",
    "ccd5e5ef-110b-47a7-8acf-6c6e9fefd5fe",
    "f8cf0175-bc47-4602-8b3e-6438227201fa"
    };

    string currentAddress;
    int currentSequence;
    int currentGameEndSequence;
    string currentCode;
    ConnectObj currentConnectObj;
    GameEndObj currentEndObj;
    //connect variables 
    string connectR;
    string connectG;
    string connectB;


    // end variables
    string endR;
    string endG;
    string endB;

    int scoreUpdateTried = 0;

    public BuildTypeGame buildType;
    public string game;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        DontDestroyOnLoad(this);

        

    }
    public void SetCode(string code)
    {
        currentCode = code;
    }

    #region CodesGenerators
    public string GetAuthString()
    {
        string auth = gameplayView.instance.GetLoggedPlayerString() + ":" + currentCode;
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(auth));
    }
    public string GetXSeqConnect(string addr, int seq)
    {
        currentAddress = addr;
        currentSequence = seq;

        string tmst = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

        connectR = SHA256Cal(tmst + masterKeys[4] + currentAddress);
        connectG = SHA256Cal(connectR + currentSequence.ToString() + masterKeys[9] + currentAddress);
        connectB = SHA256Cal(connectG + connectR + masterKeys[3] + currentAddress);

        string xSeq = SHA256Cal(connectB + connectR + currentAddress + masterKeys[6]);

        currentConnectObj = new ConnectObj();
        currentConnectObj.address = currentAddress;
        currentConnectObj.r = connectR;
        currentConnectObj.g = connectG;
        currentConnectObj.b = connectB;

        return xSeq;
    }

    public string GetGameEndKey(int score, string nftID, int seq)
    {
        string tmst = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        currentGameEndSequence = seq;

        endR = SHA256Cal(currentCode + tmst + masterKeys[0] + score.ToString());
        endG = SHA256Cal(endR + masterKeys[1] + seq + score.ToString());
        endB = SHA256Cal(endG + nftID.ToString() + masterKeys[2] + currentAddress + score.ToString());

        int masterKeyNum = seq + 2;

        string xSeq = SHA256Cal(endB + score.ToString() + masterKeys[masterKeyNum]);

        currentEndObj = new GameEndObj();
        currentEndObj.id = nftID;
        currentEndObj.address = currentAddress;
        currentEndObj.score = score.ToString();
        currentEndObj.r = endR;
        currentEndObj.g = endG;
        currentEndObj.b = endB;

        return xSeq;
    }


    string SHA256Cal(string data)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

            // Convert byte array to a string   
            string encodedText = Convert.ToBase64String(bytes);
            return encodedText.ToString();
            /* StringBuilder builder = new StringBuilder();
             for (int i = 0; i < bytes.Length; i++)
             {
                 builder.Append(bytes[i].ToString("x2"));
             }
             return builder.ToString();*/
        }

    }
    #endregion CodesGenerators


    #region Requests
    public IEnumerator GetRequest()
    {
        int sequence = UnityEngine.Random.Range(1, 8);
        Debug.Log(gameplayView.instance.GetLoggedPlayerString());
        string xseq = GetXSeqConnect(gameplayView.instance.GetLoggedPlayerString(), sequence);
        string uri="";
        if (buildType == BuildTypeGame.staging)
            uri = "https://staging-api.cryptofightclub.io/game/sdk/connect";
        else if (buildType == BuildTypeGame.production)
            uri = "https://api.cryptofightclub.io/game/sdk/connect";
        gameplayView.instance.webGLAuth.MetaSignin();
        using (UnityWebRequest webRequest = UnityWebRequest.Put(uri, JsonUtility.ToJson(currentConnectObj)))
        {
            webRequest.SetRequestHeader("sequence", sequence.ToString());
            webRequest.SetRequestHeader("timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
            webRequest.SetRequestHeader("xsequence", xseq);
            webRequest.SetRequestHeader("Content-Type", "application/json");

            //webRequest.uploadHandler = new UploadHandlerRaw((System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(currenConnectObj))));
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.Log("no connection");
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    ResponseObject temp = JsonUtility.FromJson<ResponseObject>(webRequest.downloadHandler.text);
                    SetCode(temp.code);
                    Debug.Log(webRequest.downloadHandler.text);
                    if (gameplayView.instance.usingMeta)
                        gameplayView.instance.GetLoggedPlayerString();
                    string json = "{ \"accounts\": " + JsonHelper.ToJson(temp.nfts).Replace("{\"Items\":", "");
                    gameplayView.instance.buttonsToDisableAftrLogin.SetActive(false);
                    gameplayView.instance.buttonsToEnableAftrLogin.SetActive(true);
                    Data_Manager.Instance.StartAccount(json, gameplayView.instance.cWL.OnSuccessToSingIn, gameplayView.instance.cWL.OnFailToSignIn);
                    gameplayView.instance.csv.gameObject.SetActive(true);
                    gameplayView.instance.csv.Display(temp.nfts);

                    break;
            }
        }
    }

    public IEnumerator GetOtherNft()
    {
        string uri = "https://api.cryptofightclub.io/game/sdk/koakuma/" + PlayerPrefs.GetString("Account");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.Log("no connection");
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    string data = "{\"Items\":" + webRequest.downloadHandler.text + "}";
                    Account[] Data = JsonHelper.FromJson<Account>(data);
                    if (Data.Length > 0)
                    {
                        //gameplayView.instance.hasOtherChainNft = true;
                    }
                    break;
            }
        }
    }
    public IEnumerator startSessionApi(string assetId)
    {
        leaderboardModel.userGetDataModel idData = new leaderboardModel.userGetDataModel();
        startObj strt = new startObj();
        strt.id = assetId;
        strt.address = gameplayView.instance.GetLoggedPlayerString();
        string uri="";
        if (buildType == BuildTypeGame.staging)
            uri = "https://staging-api.cryptofightclub.io/game/sdk/" + game + "/start-session";
        else if (buildType == BuildTypeGame.production)
            uri = "https://api.cryptofightclub.io/game/sdk/" + game + "/start-session";
        using (UnityWebRequest request = UnityWebRequest.Put(uri, JsonUtility.ToJson(strt)))
        {
            //request.method = UnityWebRequest.kHttpVerbPOST;
            request.downloadHandler = new DownloadHandlerBuffer();
            //request.uploadHandler = new UploadHandlerRaw(string.IsNullOrEmpty(idJsonData) ? null : Encoding.UTF8.GetBytes(idJsonData));
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Basic " + GetAuthString());
            yield return request.SendWebRequest();
            if (request.error == null)
            {
                // getDataFromRestApi(assetId);
                scoreUpdateTried = 0;
                Debug.Log("all is good in server" + Encoding.UTF8.GetString(request.downloadHandler.data));

            }
            else
            {
                Debug.Log(request.error);
                Debug.Log("error in server");
            }


        }



    }

    public IEnumerator endSessionApi(string id, int scoreAdded)
    {
        leaderboardModel.userPostedData postedData = new leaderboardModel.userPostedData();
        int sequence = UnityEngine.Random.Range(1, 8);
        string xseq = GetGameEndKey(scoreAdded, id, sequence);
        string uri = "";
        if (buildType == BuildTypeGame.staging)
            uri = "https://staging-api.cryptofightclub.io/game/sdk/" + game + "/end-session";
        else if (buildType == BuildTypeGame.production)
            uri = "https://api.cryptofightclub.io/game/sdk/" + game + "/end-session";
        using (UnityWebRequest request = UnityWebRequest.Put(uri, JsonUtility.ToJson(currentEndObj)))
        {
            request.timeout = 5;
            //byte[] bodyRaw = Encoding.UTF8.GetBytes(idJsonData);
            //request.method = "POST";
            request.SetRequestHeader("sequence", currentGameEndSequence.ToString());
            request.SetRequestHeader("timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
            request.SetRequestHeader("xsequence", xseq);
            request.SetRequestHeader("Authorization", "Basic " + GetAuthString());
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.error == null)
            {

                Debug.Log("posted Score in function");
                //Debug.Log(idJsonData);
                //Enable try again button once server responds with new score update.
               /* gameplayView.instance.gameObject.GetComponent<uiView>().SetTryAgain(true);
                if (gameplayView.instance.usingFreemint)
                    DatabaseManagerRestApi._instance.getDataFromRestApi(gameplayView.instance.GetLoggedPlayerString());
                else
                    DatabaseManagerRestApi._instance.getDataFromRestApi(gameplayView.instance.chosenNFT.id.ToString());*/


            }
            else
            {
                //if server responded with an error and resend score 
                /*if (gameplayView.instance.GetSessions() <= 10 && scoreUpdateTried < 5)
                {
                    scoreUpdateTried++;
                    gameplayView.instance.transform.GetComponentInChildren<gameEndView>().Invoke("setScoreAtStart", 6);
                }
                Debug.Log(request.error);*/
            }


        }
    }

    //to make skip option 
    public IEnumerator GetRequestSkip()
    {
        int sequence = UnityEngine.Random.Range(1, 8);
        string xseq = GetXSeqConnect(gameplayView.instance.GetLoggedPlayerString(), sequence);
        string uri = "";
        if (buildType == BuildTypeGame.staging)
            uri = "https://staging-api.cryptofightclub.io/game/sdk/connect";
        else if (buildType == BuildTypeGame.production)
            uri = "https://api.cryptofightclub.io/game/sdk/connect";
        using (UnityWebRequest webRequest = UnityWebRequest.Put(uri, JsonUtility.ToJson(currentConnectObj)))
        {
            webRequest.SetRequestHeader("sequence", sequence.ToString());
            webRequest.SetRequestHeader("timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
            webRequest.SetRequestHeader("xsequence", xseq);
            webRequest.SetRequestHeader("Content-Type", "application/json");

            //webRequest.uploadHandler = new UploadHandlerRaw((System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(currenConnectObj))));
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.Log("no connection");
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    ResponseObject temp = JsonUtility.FromJson<ResponseObject>(webRequest.downloadHandler.text);
                    SetCode(temp.code);
                    break;
            }
        }
    }


    #endregion Requests
}
