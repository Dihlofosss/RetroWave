using static Unity.Mathematics.math;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Mesh), typeof(MeshFilter))]
public class MeshGen : MonoBehaviour
{
    private Mesh mesh;

    private Texture2D texture;
    private List<Vector3> verts = new List<Vector3>();
    private List<Color> colors = new List<Color>();
    private int[] tris;
    private Vector2[] uv;
    private float[] sound = new float[64];
    private bool isPaused = true;
    private float _pauseScale = 0;

    [SerializeField]
    private ColorPalette palette;
    [SerializeField]
    private int width, length;
    [SerializeField]
    private bool useTexture;
    private float counter;

    private void Awake()
    {
        mesh = new Mesh();
        mesh.name = "Procedural Grid";
        GetComponent<MeshFilter>().mesh = mesh;

        //verts = new Vector3[(width + 1) * (length + 1)];
        tris = new int[4 * width * length * 6];
        uv = new Vector2[(width * 2 + 1) * (length * 2 + 1)];
        texture = new Texture2D(128, 128, TextureFormat.R8, true);
        texture.anisoLevel = 8;
    }

    void Start()
    {
        for(short i = 0, z = (short)-length; z <= length; z++)
        {
            for(short x = (short)-width; x <= width; x++, i++)
            {
                verts.Add(new Vector3(x, 0, z));
                uv[i] = new Vector2(x, z);
                colors.Add(palette.getDefaultGridColor());
            }
        }
        mesh.vertices = verts.ToArray();
        mesh.uv = uv;
        mesh.colors = colors.ToArray();
        
        rebuildTris();
        if(useTexture)
            generateGridTexture();
    }

    void Update()
    {
        if (isPaused)
            return;

        counter += Time.deltaTime * _pauseScale;
        //counter *= _pauseScale;
        if(counter >= 1)
        {
            counter = frac(counter);
            rebuildVerts(counter);
        }
        moveGrid();
        rebuildTris();
    }

    private void rebuildVerts(float shift)
    {
        float height;
        AudioListener.GetSpectrumData(sound, 0, FFTWindow.Rectangular);
        //remove last poly row
        for(int i = verts.Count; i <= verts.Count - (2 * width); i--)
        {
            verts.RemoveAt(i - 1);
            colors.RemoveAt(i - 1);
        }
        //add vert for 1st poly row
        for(int i = width; i >= -width; i--)
        {
            height = Mathf.Abs(i) - width / 4;
            height = 1 - Mathf.Pow(1 - sound[Mathf.Abs(Mathf.Abs((int)height) - (width / 2))], 2);
            height *= 3f;

            if (i < 2 && i > -2)
                height *= 0.1f;
            verts.Insert(0, new Vector3(i, height , shift - length));
            
            colors.Insert(0, Color.Lerp(palette.getDefaultGridColor(), palette.getPe(), height / 3f));
        }
        verts.TrimExcess();
        mesh.vertices = verts.ToArray();
        mesh.colors = colors.ToArray();
    }

    private void moveGrid()
    {
        for(short i = 0; i < verts.Count; i++)
        {
            //verts[i].z -= Time.deltaTime;
            verts[i] = new Vector3(verts[i].x, verts[i].y, verts[i].z + Time.deltaTime * _pauseScale);
        }
        mesh.vertices = verts.ToArray();
    }

    private void rebuildTris()
    {
        for (short ti = 0, vi = 0, y = 0; y < length * 2; y++, vi++)
        {
            for (short x = 0; x < width * 2; x++, ti += 6, vi++)
            {
                tris[ti] = vi;
                tris[ti + 3] = tris[ti + 2] = vi + 1;
                tris[ti + 4] = tris[ti + 1] = vi + width * 2 + 1;
                tris[ti + 5] = vi + width * 2 + 2;
            }
        }
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        //mesh.no
        mesh.RecalculateBounds();
    }

    private void generateGridTexture()
    {
        float value = (float) 1 / texture.width;
        float modW;
        float modH;
        for(short height = 0; height < texture.height; height++)
        {
            modH = abs(((float)height * 2) - texture.height) * value;
            modH = pow(modH, 10);
            modH = 1 - modH;
            for (short width = 0; width < texture.width; width++)
            {
                modW = abs(((float)width * 2) - texture.width) * value;
                modW = pow(modW, 10);
                modW = 1 - modW;
                
                texture.SetPixel(height, width, floatToColor(smoothstep(0.65f, 0.8f, 1 - (modH * modW))));
            }
        }
        texture.Apply();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetTexture("_MainTex", texture);
        propertyBlock.SetFloat("_UseTexture", 1);
        meshRenderer.SetPropertyBlock(propertyBlock);
        //GetComponent<MeshRenderer>().material.SetFloat("_UseTexture", 1);
        //GetComponent<MeshRenderer>().material.mainTexture = texture;
    }

    private Color floatToColor(float value)
    {
        return new Color(value, value, value);
    }

    IEnumerator Pause()
    {
        while(_pauseScale > 0)
        {
            _pauseScale -= Time.deltaTime * 3f;
            yield return null;
        }
        _pauseScale = 0;
        isPaused = true;
    }

    IEnumerator UnPause()
    {
        isPaused = false;
        while (_pauseScale < 1)
        {
            _pauseScale += Time.deltaTime * 3f;
            yield return null;
        }
        _pauseScale = 1;
    }

    public void PauseToggle()
    {
        if (isPaused)
            StartCoroutine(UnPause());
        else
            StartCoroutine(Pause());
    }
}