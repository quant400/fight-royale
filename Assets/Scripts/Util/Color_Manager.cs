using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Color_Manager : MonoBehaviour
{
    [SerializeField] private  ColorPalette _palette;
    public static ColorPalette pallete;

    void Awake()
    {
        pallete = _palette;
        DontDestroyOnLoad(gameObject);
    }
}
