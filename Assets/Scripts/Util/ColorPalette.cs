using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "CFC/New Color Palette")]
public class ColorPalette : ScriptableObject
{
    public Color primaryColor;
    public Color secondaryColor;

    public List<Color> playerColors;

    public Color RandomPlayerColor()
    {
        return playerColors[Random.Range(0, playerColors.Count)];
    }
}
