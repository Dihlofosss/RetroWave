using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalete", menuName = "ScriptableObjects/ColorPreset", order = 1)]
public class ColorPalette : ScriptableObject
{
    [SerializeField, ColorUsage(false ,true)]
    private Color defaultRingColor, peakRingColor;
    [SerializeField, ColorUsage(false, true)]
    private Color defaultGridColor, peakGridColor;
    [SerializeField, ColorUsage(false)]
    private Color mainSkyColor, fadeSkyColor;
    [SerializeField, ColorUsage(false, true)]
    private Color sunColor;

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

    public Color getSunColor()
    {
        return sunColor;
    }

    public void Apply(ColorPalette palette)
    {
        defaultRingColor = palette.getDefaultRingColor();
        defaultGridColor = palette.getDefaultGridColor();
        mainSkyColor = palette.getMainSkyColor();
        peakRingColor = palette.getPeakRingColor();
        peakGridColor = palette.getPeakGridColor();
        fadeSkyColor = palette.getFadeSkyColor();
        sunColor = palette.getSunColor();
    }
}