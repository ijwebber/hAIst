using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Grid {
    private int width;
    private int height;
    private int[,] gridArray;
    private float cellSize;
    private Vector3 offset = new Vector3(-50f,9,0f);
    private TextMesh[,] debugTextArray;

    public Grid(int width, int height, float inCellSize, GameObject gridContainer) {
        this.width = width;
        this.height = height;
        cellSize = inCellSize;
        
        //Initialise arrays
        gridArray = new int[width,height];
        debugTextArray = new TextMesh[width,height];

        // populate grid
        for (int i = 0; i < gridArray.GetLength(0); i++) {
            for (int j = 0; j < gridArray.GetLength(1); j++) {
                debugTextArray[i,j] = UtilsClass.CreateWorldText(gridArray[i,j].ToString(), null, (offset + (Quaternion.Euler(90,0,0) * (new Vector3(i,j)*cellSize))) + new Vector3(cellSize, cellSize) *.5f, 5, Color.white, TextAnchor.MiddleCenter);
                Debug.DrawLine(offset + Quaternion.Euler(90,0,0) * (new Vector3(i,j)*cellSize), offset + Quaternion.Euler(90,0,0) * (new Vector3(i,j+1)*cellSize), Color.white, 100f);
                Debug.DrawLine(offset + Quaternion.Euler(90,0,0) * (new Vector3(i,j)*cellSize), offset + Quaternion.Euler(90,0,0) * (new Vector3(i+1,j)*cellSize), Color.white, 100f);
            }
            Debug.DrawLine(new Vector3(0,height)*cellSize, new Vector3(width, height), Color.white, 100f);
            Debug.DrawLine(new Vector3(width,0)*cellSize, new Vector3(width, height), Color.white, 100f);
        }
    }

    //get world position of grid (doesn't work)
    private Vector3 GetWorldPosition(int x, int y) {
        Vector3 position = new Vector3(x,y) * cellSize;
        Debug.Log(position.ToString());
        return position;
    }

    // set value of grid
    public void SetValue(int x, int y, int value) {
        if (x >=0 && y >= 0 && x < width && y < height) {
            gridArray[x,y] = value;
            debugTextArray[x,y].text = value.ToString();
        }
    }

    private void getXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - offset).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - offset).z / cellSize);
    }
    public void SetValue(Vector3 worldPosition, int value) {
        int x, y;
        getXY(worldPosition, out x, out y);
        SetValue(x,y,value);
    }
}