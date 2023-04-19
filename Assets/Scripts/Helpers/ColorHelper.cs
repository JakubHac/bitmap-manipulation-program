using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorHelper
{
    public static Color ChangeValue(Color color, float newValue)
    {
        Color.RGBToHSV(color, out float h, out float s, out _);
        return Color.HSVToRGB(h, s, newValue);
    }
}
