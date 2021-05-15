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
    private int Dimension = 75;
    public PlayerController playerController;
    private Vector3[] vertices;
    private Vector2[] uv;
    private Color[] colors;
    private int[] triangles;

    private void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateEmptyMeshArrays(Dimension * Dimension, out vertices, out uv, out colors, out triangles);
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
        // mesh.Clear();
        grid.getXY(playerController.player.transform.position, out int playerX, out int playerY);
        int startX = Mathf.Max(0, playerX - Dimension/2);
        int startY = Mathf.Max(0, playerY - Dimension/2);
        int endX = Mathf.Min(grid.GetWidth(), playerX + Dimension/2);
        int endY = Mathf.Min(grid.GetHeight(), playerY + Dimension/2);
        if (playerX + Dimension/2 > grid.GetWidth()-1) {
            endX = grid.GetWidth()-1;
            startX = grid.GetWidth() - Dimension;
        }
        if (playerY + Dimension/2 > grid.GetHeight()-1) {
            endY = grid.GetHeight()-1;
            startY = grid.GetHeight() - Dimension;
        }
        // this.gameObject.transform.position = new Vector3(startX, this.gameObject.transform.position.y, startY);
        for (int x = startX; x < endX; x++) {
            for (int y = startY; y < endY; y++)  {  
                // double gridValue = grid.GetValue(x,y);
                // if (grid.getVelocity(x,y) != 0) {
                int index = ((x-startX) * Dimension + (y-startY));
                Vector3 quadSize = new Vector3(1,1,0) * grid.GetCellSize();
                double gridValue = grid.GetAvgValue(x,y);
                double val2 = grid.GetAvgValue(x+1,y);
                double val3 = grid.GetAvgValue(x+1,y+1);
                double val4 = grid.GetAvgValue(x,y+1);
                float height0, height1, height2, height3;
                if (grid.getVelocity(x,y) > 0) {
                    height0 = process(gridValue, .15f, .6f);
                    height1 = process(val2, .15f, .6f);
                    height2 = process(val3, .15f, .6f);
                    height3 = process(val4, .15f, .6f);
                } else {
                    height0 = 0;
                    height1 = 0;
                    height2 = 0;
                    height3 = 0;
                }
                // float height0 = process(gridValue, .15f, .6f);
                // float height1 = process(val2, .15f, .6f);
                // float height2 = process(val3, .15f, .6f);
                // float height3 = process(val4, .15f, .6f);
                float[] heights = {height0, height1, height2, height3};
                Color color0 = gradient.Evaluate(process(gridValue,0,1));
                Color color1 = gradient.Evaluate(process(val2, 0, 1));
                Color color2 = gradient.Evaluate(process(val3, 0, 1));
                Color color3 = gradient.Evaluate(process(val4, 0, 1));
                Color[] gradColors = {color0, color1, color2, color3};
                // set uv (deprecated, used to set colour from gradient)
                Vector2 gridvalueUV = new Vector2((float)gridValue, 0);
                // if (index < 10000) {
                MeshUtils.AddToMeshArrays(vertices, uv, colors, triangles, index, (grid.GetWorldPosition(x,y)+new Vector3(.4f,0,.3f)), 0f, quadSize, gridvalueUV, gridvalueUV, gradColors, heights);
                // }
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

    private float process(double val, float min, float max) {
        if (val <= 0) {
            return 0;
        }
        return Mathf.Clamp((float)Math.Sqrt(val)/3,min,max);
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
