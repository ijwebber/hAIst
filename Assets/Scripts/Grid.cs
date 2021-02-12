using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Node {
    public float pressure;
}

public class Grid {
    private int width;
    private int height;
    private float maxPressure = 120;
    private float deltaT, v;
    private float speedOfSound = 343/10;
    private int[,] gridArray;
    private float cellSize;
    private Vector3 offset = new Vector3(-50.12f,11f,0.61f);
    // private TextMesh[,] debugTextArray;
    private double[,] currentPressure, previousPressure, nextPressure, velocities;

    public Grid(int width, int height, float inCellSize, GameObject gridContainer) {
        this.width = width;
        this.height = height;
        cellSize = inCellSize;
        
        //Initialise arrays
        gridArray = new int[width,height];
        // debugTextArray = new TextMesh[width,height];

        deltaT = .02f;
        v = 343/10f;
        nextPressure     = new double[width,height];
        currentPressure  = new double[width,height];
        previousPressure = new double[width,height];
        velocities       = new double[width,height];

        // populate grid
        for (int i = 0; i < gridArray.GetLength(0); i++) {
            for (int j = 0; j < gridArray.GetLength(1); j++) {
                velocities[i,j] = speedOfSound;
                // debugTextArray[i,j] = UtilsClass.CreateWorldText(gridArray[i,j].ToString(), null, (offset + (Quaternion.Euler(90,0,0) * (new Vector3(i,j)*cellSize))) + new Vector3(cellSize, cellSize) *.5f, 5, Color.white, TextAnchor.MiddleCenter);
                // Debug.DrawLine(offset + Quaternion.Euler(90,0,0) * (new Vector3(i,j)*cellSize), offset + Quaternion.Euler(90,0,0) * (new Vector3(i,j+1)*cellSize), Color.white, 100f);
                // Debug.DrawLine(offset + Quaternion.Euler(90,0,0) * (new Vector3(i,j)*cellSize), offset + Quaternion.Euler(90,0,0) * (new Vector3(i+1,j)*cellSize), Color.white, 100f);
            }
            // Debug.DrawLine(new Vector3(0,height)*cellSize, new Vector3(width, height), Color.white, 100f);
            // Debug.DrawLine(new Vector3(width,0)*cellSize, new Vector3(width, height), Color.white, 100f);
        }
        List<GameObject> walls = getWalls();
        foreach (GameObject w in walls)
        {
            Vector3 wallPos = new Vector3(w.transform.position.x, 0, w.transform.position.z) + offset;
        }
        for (int i = 0; i < gridArray.GetLength(0)-2; i++) {
            for (int j = 0; j < gridArray.GetLength(1) - 2; j++) {
                float localVel = speedOfSound;
                bool noCollision = false;
                Vector3 point1 = new Vector3(i, 3, j) + offset;
                Vector3 point2 = new Vector3(i+1, 3, j) + offset;
                Vector3 point3 = new Vector3(i, 3, j+1) + offset;
                Vector3 point4 = new Vector3(i+1, 3, j+1) + offset;
                if (Physics.Linecast(point1, point2, (1 << 8))) {
                    velocities[i,j] = 343;
                    velocities[i+1,j] = 343;
                    noCollision = true;
                }
                if (Physics.Linecast(point1, point3, (1 << 8))) {
                    velocities[i,j] = 343;
                    velocities[i,j+1] = 343;
                    noCollision = true;
                }
                if (Physics.Linecast(point1, point4, (1 << 8))) {
                    velocities[i,j] = 343;
                    velocities[i+1,j+1] = 343;
                    noCollision = true;
                }
                // if (Physics.Linecast(point2, point3, (1 << 8))) {
                //     velocities[i+1,j] = 343;
                //     velocities[i,j+1] = 343;
                //     noCollision = true;
                // }
                if (!noCollision) {
                    velocities[i,j] = localVel;
                }
            }
        }
    }

    List<GameObject> getWalls() {
        List<GameObject> walls = new List<GameObject>();
        GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[]; //will return an array of all GameObjects in the scene
        foreach(GameObject go in gos)
        {
            if(go.layer==8)
            {
                walls.Add(go);
            }
        } 
        return walls;
    }

    public double GetValue(int x, int y) {
        return this.currentPressure[x,y];
    }

    //get world position of grid (doesn't work)
    public Vector3 GetWorldPosition(int x, int y) {
        Vector3 position = (new Vector3(x,0,y) * cellSize);
        position += offset;
        return position;
    }

    // update every time step
    public void updateNodes() {
        for (int x = width-1; x >= 0; x--) {
            for (int y = height-1; y >= 0; y--) {
                double x_1 = 0;
                double x_2 = 0;
                double y_1 = 0;
                double y_2 = 0;
                if (x+1 < width) {
                    x_1 = currentPressure[x+1,y];
                }
                if (x != 0) {
                    x_2 = currentPressure[x-1,y];
                }
                if (y+1 < height) {
                    y_1 = currentPressure[x,y+1];
                }
                if (y != 0) {
                    y_2 = currentPressure[x,y-1];
                }
                double nextValue = 2*currentPressure[x,y] - previousPressure[x,y] + deltaT*deltaT * velocities[x,y]*velocities[x,y] * ((x_1 - 2*currentPressure[x,y] + x_2) + (y_1 - 2*currentPressure[x,y] + y_2));
                if (nextValue < 1 || velocities[x,y] != speedOfSound) {
                    nextPressure[x,y] = 0f;
                } else {
                    nextPressure[x,y] = nextValue;
                }
                // debugTextArray[x,y].text = ((int)(nextPressure[x,y])).ToString();
                float r = 1f;
                float a = 0;
                if (nextPressure[x,y] != 0) {
                    r = (float)((1/nextPressure[x,y]));
                    // Debug.Log(r);
                    a = 1;
                }
                // if (velocities[x,y] == speedOfSound) {
                    // debugTextArray[x,y].color = new Vector4(1,1,1,(1-r));
                // } else {
                //     debugTextArray[x,y].color = new Vector4(1,0,0,1);
                // }
            }
        }
        this.previousPressure = (double[,])currentPressure.Clone();
        this.currentPressure = (double[,])nextPressure.Clone();
    }

    public int GetWidth() {
        return this.width;
    }

    public int GetHeight() {
        return this.height;
    }

    public float GetCellSize() {
        return this.cellSize;
    }

    // set value of grid
    public void SetValue(int x, int y, int value) {
        if (x >=0 && y >= 0 && x < width && y < height) {
            gridArray[x,y] = value;
            // debugTextArray[x,y].text = value.ToString();
            currentPressure[x,y] = value;
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