using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class PlayerDisplayScript : MonoBehaviour
{
    [SerializeField]
    Image character;
    [SerializeField]
    Image Background;
    [SerializeField]
    Image HealthBar;
    [SerializeField]
    TMP_Text charName;
    [SerializeField]
    Sprite[] backGroundImages;
    [SerializeField]
    Sprite[] nameDisplayBackgrounds;

    private void Start()
    {
        //SetChar(Character_Manager.Instance.GetCurrentCharacter.Name.Replace(' ','-'));
    }
    public void SetChar(string cName, GameObject chtr)
    {
        character.sprite = Resources.Load(Path.Combine("DisplaySprites/HeadShots", cName), typeof(Sprite)) as Sprite;
        int ind = IngameUIControler.instance.GetPlayerNumber();
        Background.sprite = backGroundImages[ind];
        SetHealthColor(ind);
        charName.text = cName.ToUpper();
        SetHealthColor(ind);
        SetNameDisplayColor(chtr,ind);
    }

    public void SetLocalChar(string cName)
    {
        character.sprite = Resources.Load(Path.Combine("DisplaySprites/HeadShots", cName), typeof(Sprite)) as Sprite;
        Background.sprite = backGroundImages[4];
        charName.text = cName.ToUpper();
        SetHealthColor(5);
        Invoke("CheckIndex",1f);
    }
    void SetHealthColor(int ind)
    {
        switch(ind)
        {
            case 5:
                HealthBar.color = Color.red;
                break;
            case 0:
                HealthBar.color = new Color(0.5882353f, 0.1333333f, 0.9450981f, 1);
                break;
            case 1:
                HealthBar.color = new Color(0f, 0.5803922f, 0.3254902f, 1);
                break;
            case 2:
                HealthBar.color = new Color(0.8196079f, 0.8313726f, 0.1372549f, 1);
                break;
            case 3:
                HealthBar.color = new Color(0.1098039f, 0.5254902f, 0.8235295f, 1);
                break;
            

        }

    }
    public void SetNameDisplayColor(GameObject character,int ind)
    {
        character.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = nameDisplayBackgrounds[ind];
    }
    void CheckIndex()
    {
        if(transform.GetSiblingIndex()!=0)
        {
            transform.SetAsFirstSibling();
        }
    }
}
