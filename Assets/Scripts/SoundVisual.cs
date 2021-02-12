using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class SoundVisual : MonoBehaviour
{
    public Gradient gradient;
    private Grid grid;
    private Mesh mesh;

    private void Awake() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }
    public void SetGrid(Grid grid) {
        this.grid = grid;
        UpdateSoundVis();
    }

    private void UpdateSoundVis() {
        CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);
        Color[] colors = new Color[grid.GetWidth()*grid.GetHeight()];

        // Debug.Log(grid.GetWidth());
        int i = 0;
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++)  {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1,1,0) * grid.GetCellSize();
                double gridValue = grid.GetValue(x,y);
                if (gridValue != 0) {
                    // Debug.Log(gridValue);
                    gridValue = 1/gridValue;
                } else {
                    gridValue = 1;
                }
                // colors[i] = new Vector4(1,0,1,(float)gridValue);
                // i++;
                Vector2 gridvalueUV = new Vector2((float)gridValue, 0);

                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, Quaternion.Euler(-90,0,0)*grid.GetWorldPosition(x,y), 0f, quadSize, gridvalueUV, gridvalueUV);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        // mesh.colors = colors;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs, out int[] triangles) {
        vertices = new Vector3[4*quadCount];
        uvs = new Vector2[4*quadCount];
        triangles = new int[6*quadCount];
    }
}
