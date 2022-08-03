
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
    Camera cam;
    [SerializeField]
    Transform[] characters;
    [SerializeField]
    Transform characterList;
   
    bool selected;
    [SerializeField]
    float sideCharZdisp;
    [SerializeField]
    Button rightButton, leftButton , select, backButton;
    Account[] myNFT;
    [SerializeField]
    RuntimeAnimatorController controller;
    [SerializeField]
    GameObject buttonsToEnable, ButtonToDisable;
    //[SerializeField]
    Account[] characterNFTMap;

    //for new screen
    [SerializeField]
    Transform[] charButtons;
    bool[] avaliableColors = new bool[] { true, true, true, true, true };
    int currentStartIndex;
    //for skip
    bool skipping;
    UnityEngine.Object[] info;

    public static UnityWebRequest temp;

    public void Start()
    {
       
        StartCoroutine(GetRequest("https://api.cryptofightclub.io/game/sdk/0xbecd7b5cfab483d65662769ad4fecf05be4d4d05"));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    //Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    temp = webRequest;
                    GetCharacters();
                    break;
            }
        }
    }

    
   
    void GetCharacters()
    {
        string data = "{\"Items\":" + temp.downloadHandler.text + "}";
        Account[] NFTData = JsonHelper.FromJson<Account>(data);
        SetData(NFTData);
    }

    internal void SetData(Account[] nFTData)
    {
        //will be respossible for setting up characters according to nft
        myNFT = nFTData;
        characterNFTMap = new Account[myNFT.Length];
        DisplayChar(0);

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
                //characterNFTMap[i + startingindex] = myNFT[i + startingindex];
            }
        }
        ResetAvalaibleColors();
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
    /*public void UpdateSelected(int selected)
    {
        currentCharacter = currentStartIndex + selected;
        gameplayView.instance.chosenNFT = characterNFTMap[currentCharacter];
        gameplayView.instance.GetScores();

    }*/

    /*void setPlayButtonDependtoSessions(int sessions)
   {
       if (sessions >= 10)
       {
           select.interactable = false;

       }
       else
       {
           select.interactable = true;
       }
   }


   private void ResetAvalaibleColors()
   {
       for(int i=0;i<avaliableColors.Length;i++)
       {
           avaliableColors[i] = true;
       }
   }
   public void MoveRight()
   {

       currentStartIndex += 4;
       if (skipping) 
       {
           if (currentStartIndex+4 >info.Length-1)
               rightButton.gameObject.SetActive(false);
           else
               rightButton.gameObject.SetActive(true);
           if(currentStartIndex>0)
               leftButton.gameObject.SetActive(true);
           SkipDisplayChars(currentStartIndex);
       }
       else
       {
           if (currentStartIndex+4 > myNFT.Length-1)
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

   public void EnablePlay()
   {
       select.interactable = true;
   }
   public void DisablePlay()
   {
       select.interactable = false;
   }

   public void Selected()
   {
      // GameRoom.room.ChooseAvatar("PlayerAvatar" + currentCharacter);
       selected = true;
       characters[currentCharacter].GetComponent<Animator>().SetBool("Selected", true);

   }


   public void UndoFinalSelect()
   {
       rightButton.interactable = true;
       leftButton.interactable = true;
   }

   //added for single player
   public void FinalSelectSinglePlayer()
   {
       gameplayView.instance.chosenNFT = characterNFTMap[currentCharacter];
       selected = true;
       //characters[currentCharacter].GetComponent<Animator>().SetBool("Selected", true);


   }

   // foire new character selection
   public void UpdateSelected(int selected)
   {
       currentCharacter = currentStartIndex+selected;
       gameplayView.instance.chosenNFT = characterNFTMap[currentCharacter];
       gameplayView.instance.GetScores();

   }


   internal void SetData(NFTInfo[] nFTData)
   {
       //will be respossible for setting up characters according to nft
       myNFT = nFTData;
       SetUpCharacters();

   }


   private void SetUpCharacters()
   {
       if (chickenGameModel.charactersSetted == false)
       {



          skipping = false;
          characters = new Transform[myNFT.Length];
          characterNFTMap = new NFTInfo[myNFT.Length];


       }

       Done();


   }


   //skip for new screen
   public void Skip()
   {
       if (!gameplayView.instance.isTryout)
       {
           skipping = true;
           info = Resources.LoadAll("SinglePlayerPrefabs/DisplaySprites/HeadShots", typeof(Sprite));
           characterNFTMap = new NFTInfo[info.Length];
           SkipDisplayChars(0);
           Done();
       }
       else
       {
           gameplayView.instance.chosenNFT = new NFTInfo { name = "a-rod", id = 175 };
           selected = true;
       }
   }

   void SkipDisplayChars(int startingindex)
   {
       for (int i = 0; i < 4; i++)
       { 

           if (i+startingindex>=info.Length)
               charButtons[i].GetComponent<ButtonInfoHolder>().SetChar("null");
           else
           {
               string name = info[i + startingindex].name;
               charButtons[i].GetComponent<ButtonInfoHolder>().SetChar(name);
               characterNFTMap[i + startingindex] = new NFTInfo { id = 175, name = name };
           }
       }
       ResetAvalaibleColors();
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
               string charName = NameToSlugConvert(myNFT[i+startingindex].name);
               charButtons[i].GetComponent<ButtonInfoHolder>().SetChar(charName);
               characterNFTMap[i+startingindex] = myNFT[i+startingindex];
           }
       }
       ResetAvalaibleColors();
       //chickenGameModel.charactersSetted = true;
   }
   private void Done()
   {
       buttonsToEnable.SetActive(true);
       ButtonToDisable.SetActive(false);
   }

   void BackButton()
   {
       Debug.Log("reached");
       //chickenGameModel.gameCurrentStep.Value = chickenGameModel.GameSteps.Onlogged;
   }

   string NameToSlugConvert(string name)
   {
       string slug;
       slug = name.ToLower().Replace(".", "").Replace("'", "").Replace(" ", "-");
       return slug;

   }*/
}

