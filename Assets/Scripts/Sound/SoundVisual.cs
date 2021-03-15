using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using CodeMonkey.Utils;

public class SoundVisual : MonoBehaviour
{
    public Gradient gradient;
    public Grid grid;
    private Mesh mesh;
    private int Dimension = 100;
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

    private Vector3[] GenerateVerts() {
        var verts = new Vector3[(Dimension + 1) * (Dimension + 1)];

        for (int x = 0; x <= 100; x++) {
            for(int z = 0; z <= 100; z++) {
                verts[index(x, z)] =  new Vector3(x, 0, z);
            }
        }
        return verts;
    }

    private int index(int x, int z) {
        return x * (Dimension + 1) + z;
    }

    private int[] GenerateTries() {
        var tries = new int[mesh.vertices.Length*6];

        for (int x = 0; x < Dimension; x++) {
            for (int z = 0; z < Dimension; z++) {
                tries[index(x,z) * 6 + 0] = index(x,z);
                tries[index(x,z) * 6 + 1] = index(x+1,z+1);
                tries[index(x,z) * 6 + 2] = index(x+1,z);
                tries[index(x,z) * 6 + 3] = index(x,z);
                tries[index(x,z) * 6 + 4] = index(x,z+1);
                tries[index(x,z) * 6 + 5] = index(x+1,z+1);
            }
        }

        return tries;
    }

    private void UpdateSoundVis() {
        grid.getXY(playerController.player.transform.position, out int playerX, out int playerY);
        int startX = Mathf.Max(1, playerX - 50);
        int startY = Mathf.Max(1, playerY - 50);
        int endX = Mathf.Min(grid.GetWidth(), playerX + 50);
        int endY = Mathf.Min(grid.GetHeight(), playerY + 50);
        if (playerX + 50 > grid.GetWidth()-1) {
            endX = grid.GetWidth()-1;
            startX = grid.GetWidth() - 100;
        }
        if (playerY + 50 > grid.GetHeight()-1) {
            endY = grid.GetHeight()-1;
            startY = grid.GetHeight() - 100;
        }
        // this.gameObject.transform.position = new Vector3(startX, this.gameObject.transform.position.y, startY);
        Debug.Log("!!!" + startX + " // " + endX);
        Debug.Log("!!!" + startY + " // " + endY);
        var verts = mesh.vertices;
        CreateEmptyMeshArrays(100 * 100, out Vector3[] vertices, out Vector2[] uv, out Color[] colors, out int[] triangles);
        for (int x = startX; x < endX; x++) {
            for (int y = startY; y < endY; y++)  {  
                double gridValue = grid.GetValue(x,y);
                if (gridValue > 240) {
                    Debug.Log(gridValue);
                }
                int index = ((x-startX) * 100 + (y-startY));
                Vector3 quadSize = new Vector3(1,1,0) * grid.GetCellSize();
                float a = 0f;
                Vector4 color = new Vector4(1, 1, 1, a);
                if (gridValue > 0) {
                    // normalise transparency value
                    gridValue = Math.Sqrt(gridValue)/10;
                    if (gridValue < .15f) {
                        a = .15f;
                    } else if(gridValue > .6f) {
                        a = .6f;
                    } else {
                        a = ((float)gridValue);
                    }
                    color = new Vector4(1, 1, 1, a);
                } else {
                    if (grid.getSpeed(x,y) == 343) {
                        // a = 1;
                        // color = new Vector4(1,0,0,a);
                    }
                }
                // set color


                float height0 = (float)Math.Sqrt(grid.GetAvgValue(x,y))/10;
                float height1 = (float)Math.Sqrt(grid.GetAvgValue(x+1,y))/10;
                float height2 = (float)Math.Sqrt(grid.GetAvgValue(x,y+1))/10;
                float height3 = (float)Math.Sqrt(grid.GetAvgValue(x+1,y+1))/10;
                float[] heights = {height0, height1, height2, height3};
                Color color0 = gradient.Evaluate((float)Math.Sqrt(grid.GetAvgValue(x,y))/3);
                Color color1 = gradient.Evaluate((float)Math.Sqrt(grid.GetAvgValue(x+1,y))/3);
                Color color2 = gradient.Evaluate((float)Math.Sqrt(grid.GetAvgValue(x,y+1))/3);
                Color color3 = gradient.Evaluate((float)Math.Sqrt(grid.GetAvgValue(x+1,y+1))/3);
                Color[] gradColors = {color0, color1, color2, color3};
                // set uv (deprecated, used to set colour from gradient)
                Vector2 gridvalueUV = new Vector2((float)gridValue, 0);
                // if (index < 10000) {
                MeshUtils.AddToMeshArrays(vertices, uv, colors, triangles, index, (grid.GetWorldPosition(x,y)), 0f, quadSize, gridvalueUV, gridvalueUV, gradColors, heights);
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
