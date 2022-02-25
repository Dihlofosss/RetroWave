using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalete", menuName = "ScriptableObjects/ColorPreset", order = 1)]
public class ColorPalette : ScriptableObject
{
    [SerializeField, ColorUsage(false ,true)]
    private Color defaultRingColor, peakRingColor;
    [SerializeField]
    private Color defaultGridColor, peakGridColor;
    [SerializeField]
    private Color mainSkyColor, fadeSkyColor;

    public Color getDefaultRingColor()
    {
        return defaultRingColor;
    }
    public Color getPeakRingColor()
    {
        return peakRingColor;
    }
    public Color getDefaultGridColor()
    {
        return defaultGridColor;
    }
    public Color getPeakGridColor()
    {
        return peakGridColor;
    }
    public Color getMainSkyColor()
    {
        return mainSkyColor;
    }
    public Color getFadeSkyColor()
    {
        return fadeSkyColor;
    }
}