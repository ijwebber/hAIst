using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class SoundVisual : MonoBehaviour
{
    public Gradient gradient;
    public Grid grid;
    private Mesh mesh;
    public PlayerController playerController;

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
        CreateEmptyMeshArrays(100 * 100, out Vector3[] vertices, out Vector2[] uv, out Color[] colors, out int[] triangles);
        grid.getXY(playerController.player.transform.position, out int playerX, out int playerY);
        int startX = Mathf.Max(0, playerX - 50);
        int startY = Mathf.Max(0, playerY - 50);
        int endX = Mathf.Min(grid.GetWidth(), playerX + 50);
        int endY = Mathf.Min(grid.GetHeight(), playerY + 50);
        if (playerX + 50 > grid.GetWidth()) {
            endX = grid.GetWidth();
            startX = grid.GetWidth() - 100;
        }
        if (playerY + 50 > grid.GetHeight()) {
            endY = grid.GetHeight();
            startY = grid.GetHeight() - 100;
        }
        // this.gameObject.transform.position = new Vector3(startX, this.gameObject.transform.position.y, startY);
        Debug.Log("!!!" + startX + " // " + endX);
        Debug.Log("!!!" + startY + " // " + endY);
        for (int x = startX; x < endX; x++) {
            for (int y = startY; y < endY; y++)  {  
                float gridValue = grid.GetValue(x,y);
                int index = ((x-startX) * 100 + (y-startY));
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
                // if (index < 10000) {
                    MeshUtils.AddToMeshArrays(vertices, uv, colors, triangles, index, Quaternion.Euler(-90,0,0)*grid.GetWorldPosition(x,y), 0f, quadSize, gridvalueUV, gridvalueUV, color);
                // }
            }
        }

        // update mesh
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.triangles = triangles;
        // mesh.uv = uv;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
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
