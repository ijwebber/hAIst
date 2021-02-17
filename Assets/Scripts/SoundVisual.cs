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
        CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out Color[] colors, out int[] triangles);

        // Debug.Log(grid.GetWidth());
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++)  {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1,1,0) * grid.GetCellSize();
                double gridValue = grid.GetValue(x,y);
                float a = 0;
                if (gridValue != 0) {
                    // Debug.Log(gridValue);
                    gridValue = 1/gridValue;
                    a = .2f;
                }
                Vector4 color = new Vector4(gradient.Evaluate((float)gridValue).r, gradient.Evaluate((float)gridValue).g, gradient.Evaluate((float)gridValue).b, a);
                Vector2 gridvalueUV = new Vector2((float)gridValue, 0);

                MeshUtils.AddToMeshArrays(vertices, uv, colors, triangles, index, Quaternion.Euler(-90,0,0)*grid.GetWorldPosition(x,y), 0f, quadSize, gridvalueUV, gridvalueUV, color);
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        // mesh.uv = uv;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs, out Color[] colors, out int[] triangles) {
        vertices = new Vector3[4*quadCount];
        uvs = new Vector2[4*quadCount];
        colors = new Color[4*quadCount];
        triangles = new int[6*quadCount];
    }
}
