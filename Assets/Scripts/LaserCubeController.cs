using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCubeController : MonoBehaviour
{
    public short size, steps, meshScale;
    public Mesh cube;

    private MeshFilter _mFilter;

    void Start()
    {
        _mFilter.mesh = cube;
        PrepareBlock();
        for(short x = -1; x < 1; x += 2)
        {
            for(short y = -1; y < 1; y += 2)
            {
                for(short z = -1; z < 1; z += 2)
                {
                    Vector3 position = new Vector3(x, y, z);
                    position.Scale(new Vector3(0.5f, 0.5f, 0.5f));
                    _mFilter.transform.position = position;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CubeGen()
    {

    }

    private void PrepareBlock()
    {
        Vector2[] uvs = cube.uv;
        for (short i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(0.01f, 0.01f);
        }
        cube.uv = uvs;

        Vector3[] verts = cube.vertices;
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i].Scale(new Vector3(meshScale, 0.7f, meshScale));
        }
        cube.vertices = verts;
    }
}
