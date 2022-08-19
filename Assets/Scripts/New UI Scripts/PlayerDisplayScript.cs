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
    public void SetChar(string cName ,GameObject chtr)
    {
        character.sprite = Resources.Load(Path.Combine("DisplaySprites/HeadShots", cName), typeof(Sprite)) as Sprite;
        Debug.Log((transform.GetSiblingIndex(), backGroundImages[transform.GetSiblingIndex()].name));
        Background.sprite = backGroundImages[transform.GetSiblingIndex()];
        charName.text = cName.ToUpper();
        SetHealthColor(transform.GetSiblingIndex());
        SetNameDisplayColor(chtr);
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
                HealthBar.color = Color.green;
                break;
            case 2:
                HealthBar.color = Color.yellow;
                break;
            case 3:
                HealthBar.color = Color.blue;
                break;

        }

    }
    public void SetNameDisplayColor(GameObject character)
    {
        character.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = nameDisplayBackgrounds[transform.GetSiblingIndex()];
    }
    void CheckIndex()
    {
        if(transform.GetSiblingIndex()!=0)
        {
            transform.SetAsFirstSibling();
        }
    }
}
