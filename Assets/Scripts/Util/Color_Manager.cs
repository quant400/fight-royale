using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Color_Manager : MonoBehaviour
{
    public ColorPalette pallete;
    public static Color_Manager Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
