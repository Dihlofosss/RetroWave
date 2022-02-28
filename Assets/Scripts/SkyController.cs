using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyController : MonoBehaviour
{
    [SerializeField]
    private ColorPalette colorPalette;

    private MeshRenderer mRenderer;
    private MaterialPropertyBlock mBlock;

    private void Awake()
    {
        mRenderer = GetComponent<MeshRenderer>();
        mBlock = new MaterialPropertyBlock();
        mRenderer.GetPropertyBlock(mBlock);

        mBlock.SetColor("_MainSkyColor", colorPalette.getMainSkyColor());
        mBlock.SetColor("_FadeSkyColor", colorPalette.getFadeSkyColor());

        mRenderer.SetPropertyBlock(mBlock);
    }
}
