using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class SoundVisual : MonoBehaviour
{
    public Gradient gradient;
    public Grid grid;
    private Mesh mesh;

    private void Start() {
        Debug.Log("this is called");
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void initGrid(Grid grid) {
        this.grid = grid;
    }
    public void SetGrid() {
        UpdateSoundVis();
    }

    private void UpdateSoundVis() {
        CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out Color[] colors, out int[] triangles);

        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++)  {
                float gridValue = grid.GetValue(x,y);
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1,1,0) * grid.GetCellSize();
                float a = 0f;
                if (gridValue > 0) {
                    // normalise transparency value
                    gridValue = Mathf.Sqrt(gridValue)/10;
                    if (gridValue < .15f) {
                        a = .15f;
                    } else if(gridValue > .6f) {
                        a = .6f;
                    } else {
                        a = (gridValue);
                    }
                }
                // set color
                Vector4 color = new Vector4(1, 1, 1, a);

                // set uv (deprecated, used to set colour from gradient)
                Vector2 gridvalueUV = new Vector2((float)gridValue, 0);

                MeshUtils.AddToMeshArrays(vertices, uv, colors, triangles, index, Quaternion.Euler(-90,0,0)*grid.GetWorldPosition(x,y), 0f, quadSize, gridvalueUV, gridvalueUV, color);
            }
        }

        // update mesh
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangles;
        // mesh.uv = uv;
        mesh.RecalculateNormals();
    }


    public Grid getGrid() {
        Debug.Log("sending grid " + this.grid);
        return this.grid;
    }

    public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uvs, out Color[] colors, out int[] triangles) {
        vertices = new Vector3[4*quadCount];
        uvs = new Vector2[4*quadCount];
        colors = new Color[4*quadCount];
        triangles = new int[6*quadCount];
    }
}
