using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalete", menuName = "ScriptableObjects/ColorPreset", order = 1)]
public class ColorPalette : ScriptableObject
{
    [SerializeField, ColorUsage(false,true)]
    private Color32 defaultRingColor, peakRingColor;
    [SerializeField]
    private Color32 defaultGridColor, peakGridColor;
    [SerializeField]
    private Color32 mainSkyColor, fadeSkyColor;

    public Color32 getDefaultRingColor()
    {
        return defaultRingColor;
    }
    public Color32 getPeakRingColor()
    {
        return peakRingColor;
    }
    public Color32 getDefaultGridColor()
    {
        return defaultGridColor;
    }
    public Color32 getPeakGridColor()
    {
        return peakGridColor;
    }
    public Color32 getMainSkyColor()
    {
        return mainSkyColor;
    }
    public Color32 getFadeSkyColor()
    {
        return fadeSkyColor;
    }
}