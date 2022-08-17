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

    private void Start()
    {
        SetChar(Character_Manager.Instance.GetCurrentCharacter.Name.Replace(' ','-'));
    }
    public void SetChar(string cName)
    {
        character.sprite = Resources.Load(Path.Combine("DisplaySprites/HeadShots", cName), typeof(Sprite)) as Sprite;
        if (transform.GetSiblingIndex() == 0)
            Background.sprite = backGroundImages[4];
        else
            Background.sprite = backGroundImages[transform.GetSiblingIndex()-1];
        charName.text = cName.ToUpper();
        SetHealthColor();
    }

    void SetHealthColor()
    {
        int ind = transform.GetSiblingIndex();
        switch(ind)
        {
            case 0:
                HealthBar.color = Color.red;
                break;
            case 1:
                HealthBar.color = new Color(0.5882353f, 0.1333333f, 0.9450981f, 1);
                break;
            case 2:
                HealthBar.color = Color.yellow;
                break;
            case 3:
                HealthBar.color = Color.blue;
                break;

        }

    }
}
