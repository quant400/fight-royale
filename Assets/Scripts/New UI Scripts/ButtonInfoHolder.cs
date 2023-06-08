using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonInfoHolder : MonoBehaviour
{
    /*
    [SerializeField]
    Sprite bg, selectedCharBG;
    */
    [SerializeField]
    Image background, selectedBackground;

    Image charPic;
    int bgIndex;
    string charName, charEdition;
    [SerializeField]
    Image display;
    Image displayBG;
    [SerializeField]
    Sprite defaultImg;
    [SerializeField]
    TMP_Text nameText, info;
    [SerializeField]
    characterSelectionView CSV;

    private Dictionary<string, Color32> fighterEditionColors;

    //[SerializeField]
    //ButtonInfoHolder currentSelected;
    private void Awake()
    {
        //bgIndex = Random.Range(0, bg.Length);
        //background = gameObject.GetComponent<Image>();
        charPic = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        displayBG=display.transform.parent.GetComponent<Image>();
        CSV = transform.GetComponentInParent<characterSelectionView>();
        AssignFighterColors();

        ResetSlot();
    }

    public void SetCurrent(Sprite img, int index)
    {
        //background.sprite = bg[index];
        //charPic.sprite = img;
        //charPic.color = new Color(225, 225, 225, 225);
    }
    public void SetChar(string name, string edition)
    {
        charName = name;

        charEdition = edition;

        if (charName == "null")
        {
            background.sprite = defaultImg;
            charPic.color = new Color(225, 225, 225, 0);
        }
        else
        {
            //bgIndex = Random.Range(0, bg.Length);
            //background.sprite = bg[CSV.GetavaliableColor()];
            if(charEdition == "free mint")
            {
                background.color = fighterEditionColors["Default"];
            }
            else
            {
                background.color = fighterEditionColors[charEdition];
            }
            


            if (gameplayView.instance.usingFreemint)

                charPic.sprite = Resources.Load(Path.Combine("DisplaySprites/FreeMint/HeadShots", name), typeof(Sprite)) as Sprite;

            else
                charPic.sprite = Resources.Load(Path.Combine("DisplaySprites/HeadShots", name), typeof(Sprite)) as Sprite;
            //charPic.sprite = Resources.Load(Path.Combine("DisplaySprites/HeadShots", name), typeof(Sprite)) as Sprite;
            charPic.color = new Color(225, 225, 225, 225);
        }
    }

    public void OnClick()
    {
        if(charName=="null")
        {
            Application.OpenURL("https://app.cryptofightclub.io/mint");
        }
        else
        {
            Debug.Log(charName);

            display.sprite = Resources.Load(Path.Combine("DisplaySprites/Display", charName), typeof(Sprite)) as Sprite;
            display.color = new Color(225, 225, 225, 225);

            if (charEdition == "free mint")
            {
                selectedBackground.color = fighterEditionColors["Default"];
            }
            else
            {
                selectedBackground.color = fighterEditionColors[charEdition];
            }

            //displayBG.sprite = selectedCharBG[int.Parse(background.sprite.name)];
            Character_Manager.Instance.ChangeCharacter(charName.Replace('-', ' '));

            UpdateInfo();
           
        }
       
    }

    private void ResetSlot()
    {
        charPic.sprite = defaultImg;
        charPic.color = new Color(225, 225, 225, 0);
    }


    void UpdateInfo()
    { 
        nameText.text = charName.ToUpper();
        //Invoke("UpdateSessionInfo", 1.5f);
       
    }

    /*void UpdateSessionInfo()
    {
        if (chickenGameModel.currentNFTSession<10)
            CSV.EnablePlay();
        info.text = "PLAYED " + chickenGameModel.currentNFTSession + " OUT OF 10 DAILY GAMES";
    }*/

    private void AssignFighterColors()
    {
        fighterEditionColors = new Dictionary<string, Color32>();

        fighterEditionColors.Add("Muay Thai", new Color32(0, 66, 145, 255));
        fighterEditionColors.Add("MMA", new Color32(0, 148, 83, 255));
        fighterEditionColors.Add("Boxing", new Color32(241, 31, 44, 255));
        fighterEditionColors.Add("Kung Fu", new Color32(205, 79, 8, 255));
        fighterEditionColors.Add("Krav Maga", new Color32(209, 212, 35, 255));
        fighterEditionColors.Add("Tae Kwon Do", new Color32(149, 31, 241, 255));
        fighterEditionColors.Add("Wrestling", new Color32(15, 207, 207, 255));
        fighterEditionColors.Add("Karate", new Color32(223, 56, 156, 255));
        fighterEditionColors.Add("Jiu Jitsu", new Color32(28, 134, 210, 255));
        fighterEditionColors.Add("Judo", new Color32(101, 180, 74, 255));
        fighterEditionColors.Add("free mint", new Color32(255, 255, 255, 255));
        fighterEditionColors.Add("Default", new Color32(134, 0, 255, 255));
    }
}
