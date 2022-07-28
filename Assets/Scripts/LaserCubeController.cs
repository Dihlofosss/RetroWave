using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCubeController : MonoBehaviour
{
    public short size, steps;

    [Range(0, 1)]
    public float meshScale;

    public Mesh cube;

    private MeshFilter _mFilter;

    void Start()
    {
        _mFilter = new MeshFilter();
        PrepareBlock();
        
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
        CombineInstance[] combine = new CombineInstance[8];

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

        MeshFilter[] meshes = new MeshFilter[8];

        //_mFilter.mesh = cube;
        short j = 0;

        for (float y = -.5f; y < .5f; y += 1)
        {
            for (float z = -.5f; z < .5f; z += 1)
            {
                for (float x = -.5f; x < .5f; x += 1)
                {
                    Vector3 position = new Vector3(x, y, z);
                    _mFilter.mesh = cube;
                    _mFilter.transform.position = position;
                    combine[j].mesh = cube;
                    combine[j].transform = _mFilter.transform.localToWorldMatrix;
                    j++;
                }
            }
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
    }
}
