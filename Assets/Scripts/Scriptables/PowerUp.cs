using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp [NewName]", menuName = "CFC/PowerUp")]
public class PowerUp : ScriptableObject
{
    public string Name;
    public AttributePower Attribute;
    public GameObject Effect_Prefab;
}

[Serializable]
public class AttributePower
{
    public CodeAttributePower Code;
    public string Description;
    public float Value;
    public float Duration; //  0 -> For instant power

}


public enum CodeAttributePower
{
    HEAL,
    ATK,
    DEF,
    SPEED
};
