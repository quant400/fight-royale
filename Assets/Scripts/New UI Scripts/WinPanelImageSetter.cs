using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WinPanelImageSetter : MonoBehaviour
{
    [SerializeField]
    Image img;
    Sprite spr;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        Debug.Log(2);
        SetSprite();
    }
    void SetSprite()
    {
        Debug.Log(3);
        string charName = Character_Manager.Instance.GetCurrentCharacter.Name;
        Debug.Log(charName.Replace(' ', '-'));
        img.sprite = Resources.Load(Path.Combine("DisplaySprites/Display", charName.Replace(' ','-')), typeof(Sprite)) as Sprite;
    }
}
