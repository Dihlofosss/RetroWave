using UnityEngine;

public class ColorPaletteController : MonoBehaviour
{
    [SerializeField]
    private ColorPalette defaultPallete;
    [SerializeField]
    private ColorPalette newPallete;

    private void Awake()
    {
        defaultPallete.Apply(newPallete);
    }
}
