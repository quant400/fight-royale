
using UnityEngine;
//using DG.Tweening;
using UnityEngine.UI;
using System;
using System.IO;
using CFC.Serializable;
using UnityEngine.Networking;
using System.Collections;

public class characterSelectionView : MonoBehaviour
{
    public int currentCharacter;
    [SerializeField]
    Button rightButton, leftButton , select, backButton;
    Account[] myNFT;
    Account[] characterNFTMap;
    [SerializeField]
    Transform[] charButtons;
    bool[] avaliableColors = new bool[] { true, true, true, true, true };
    int currentStartIndex;
    bool skipping;
    UnityEngine.Object[] info;

    public static UnityWebRequest temp;

    public GameObject noNFTCanvas;
    

    [SerializeField]
    GameObject backButtonDestScreen;
    public void Start()
    {
        backButton.onClick.AddListener(BackButtonOnClick);
        //charButtons[0].GetComponent<ButtonInfoHolder>().SetChar("a-rod");
        //StartCoroutine(GetRequest("https://api.cryptofightclub.io/game/sdk/0xbecd7b5cfab483d65662769ad4fecf05be4d4d05"));
    }

    
    #region login functions
    public void SetUpCharactersLogin()
    {
        string acc = Data_Manager.Instance.accountId;
        skipping = false;
        if (acc != "")
        {
            StartCoroutine(KeyMaker.instance.GetRequest());

        }
        else
            Invoke("SetUpCharactersLogin", 1f);
    }

    public void Display(Account[] NFTData)
    {
        Debug.Log(4);
        Account[] used=NFTData;
        if (used.Length == 0 && !gameplayView.instance.usingMeta)
        {
            gameplayView.instance.usingFreemint = true;
            FreeMint();
        }
        else if (used.Length == 0 && gameplayView.instance.usingMeta)
        {
            Debug.Log("NoNft");
        }
        else
        {
            noNFTCanvas.SetActive(false);
            SetData(used);
        }

    }
    void DisplayChar(int startingindex)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i + startingindex >= myNFT.Length)
            {
                charButtons[i].GetComponent<ButtonInfoHolder>().SetChar("null");
            }

            else
            {
                string charName = NameToSlugConvert(myNFT[i + startingindex].name);
                charButtons[i].GetComponent<ButtonInfoHolder>().SetChar(charName);
                characterNFTMap[i + startingindex] = myNFT[i + startingindex];
            }
        }
        ResetAvalaibleColors();
    }
    #endregion login functions

    #region guest functions
    public void SetUpCharactersGuest()
    {
        skipping = true;
        info = Resources.LoadAll("DisplaySprites/HeadShots", typeof(Sprite));
        characterNFTMap = new Account[info.Length];
        SkipDisplayChars(0);
    }
    public void SkipDisplayChars(int startingindex)
    {
        for (int i = 0; i < 4; i++)
        {

            if (i + startingindex >= info.Length)
                charButtons[i].GetComponent<ButtonInfoHolder>().SetChar("null");
            else
            {
                string name = info[i + startingindex].name;
                charButtons[i].GetComponent<ButtonInfoHolder>().SetChar(name);
                characterNFTMap[i + startingindex] = new Account { id = "175", name = name };
            }
        }
        ResetAvalaibleColors();
    }
    #endregion guest functions


    public void MoveRight()
    {
        currentStartIndex += 4;
        if (skipping)
        {
            if (currentStartIndex + 4 > info.Length - 1)
                rightButton.gameObject.SetActive(false);
            else
                rightButton.gameObject.SetActive(true);
            if (currentStartIndex > 0)
                leftButton.gameObject.SetActive(true);
            SkipDisplayChars(currentStartIndex);
        }
        else
        {
            if (currentStartIndex + 4 > myNFT.Length - 1)
                rightButton.gameObject.SetActive(false);
            else
                rightButton.gameObject.SetActive(true);
            if (currentStartIndex > 0)
                leftButton.gameObject.SetActive(true);
            DisplayChar(currentStartIndex);
        }


    }

    public void MoveLeft()
    {

        currentStartIndex -= 4;
        if (skipping)
        {
            if (currentStartIndex - 4 < 0)
                leftButton.gameObject.SetActive(false);
            else
                leftButton.gameObject.SetActive(true);
            if (currentStartIndex < info.Length)
                rightButton.gameObject.SetActive(true);
            SkipDisplayChars(currentStartIndex);
        }
        else
        {
            if (currentStartIndex - 4 < 0)
                leftButton.gameObject.SetActive(false);
            else
                leftButton.gameObject.SetActive(true);
            if (currentStartIndex < myNFT.Length)
                rightButton.gameObject.SetActive(true);
            DisplayChar(currentStartIndex);
        }
    }

    public void FreeMint()
    {
        info = Resources.LoadAll("DisplaySprites/FreeMint/HeadShots", typeof(Sprite));
        characterNFTMap = new Account[info.Length];
        FreeMintDisplayChars(0);
       
    }

    void FreeMintDisplayChars(int startingindex)
    {
        for (int i = 0; i < 4; i++)
        {

            if (i + startingindex >= info.Length)
                charButtons[i].GetComponent<ButtonInfoHolder>().SetChar("null");
            else
            {
                string name = info[i + startingindex].name;
                charButtons[i].GetComponent<ButtonInfoHolder>().SetChar(name);
                characterNFTMap[i + startingindex] = new Account { id = "00000", name = name };
            }
        }
        ResetAvalaibleColors();
    }

    void GetCharacters()
    {
        string data = "{\"Items\":" + temp.downloadHandler.text + "}";
        Account[] NFTData = JsonHelper.FromJson<Account>(data);
        SetData(NFTData);
    }

    internal void SetData(Account[] nFTData)
    {
        myNFT = nFTData;
        characterNFTMap = new Account[myNFT.Length];
        DisplayChar(0);

    }
    





    string NameToSlugConvert(string name)
    {
        string slug;
        slug = name.ToLower().Replace(".", "").Replace("'", "").Replace(" ", "-");
        return slug;

    }
    private void ResetAvalaibleColors()
    {
        for (int i = 0; i < avaliableColors.Length; i++)
        {
            avaliableColors[i] = true;
        }
    }

    public int GetavaliableColor()
    {
        int c = UnityEngine.Random.Range(0, avaliableColors.Length);
        if (avaliableColors[c] == true)
        {
            avaliableColors[c] = false;
            return c;
        }
        else
            return GetavaliableColor();
    }

    public void BackButtonOnClick()
    {
        backButtonDestScreen.SetActive(true);
        gameObject.SetActive(false);
    }





   
}

