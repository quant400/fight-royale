using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "CFC/NFT/Character")]
public class Character : ScriptableObject
{
    public string Name;
    public GameObject Asset;
    public Texture Texture;

    public bool isUnlocked;
}