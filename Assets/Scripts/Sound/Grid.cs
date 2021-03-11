using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Node {
    public float pressure;
}

public class Grid {
    private int width;
    private int obstacleMask = (1 << 8) | (1 << 12);
    private int height;
    private float maxPressure = 120;
    private float deltaT, v;
    private float speedOfSound = 343/10; // this isn't actually the speed of sound, but it's the one that works best
    private int[,] gridArray;
    private float cellSize;
    public Vector3 offset = new Vector3(-100.333f,11.7f,-60.3f); // offset for bottom left grid tile (dont change z for some reason idk why)
    // private TextMesh[,] debugTextArray;
    private float[,] currentPressure, previousPressure, nextPressure, velocities;

    public float[,] getPressure() {
        return this.currentPressure;
    }
    public void setPressure(float[,] newPressure) {
        this.currentPressure = newPressure;
    }

    // update walls (will need to be implemented for moving walls or obstacles)
    public void updateWalls() {
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
                bool Collision = false;
                Vector3 point1 = new Vector3(i, 1, j) + offset;
                Vector3 point2 = new Vector3(i+1, 1, j) + offset;
                Vector3 point3 = new Vector3(i, 1, j+1) + offset;
                Vector3 point4 = new Vector3(i+1, 1, j+1) + offset;
                Color c1 = Color.green;
                Color c2 = Color.green;
                Color c3 = Color.green;
                if (Physics.Linecast(point2, point1, obstacleMask)) {
                    velocities[i,j] = 343;
                    c1 = Color.red;
                    Collision = true;
                }
                if (Physics.Linecast(point3, point1, obstacleMask)) {
                    c2 = Color.red;
                    velocities[i,j] = 343;
                    Collision = true;
                }
                Debug.DrawLine(point1, point2, c1,100);
                Debug.DrawLine(point1, point3, c2,100);
                Debug.DrawLine(point1, point4, c3,100);
                if (!Collision) {
                    velocities[i,j] = localVel;
                }
            }
        }
    }

    // initialise grid
    public Grid(int width, int height, float inCellSize) {
        this.width = width;
        this.height = height;
        cellSize = inCellSize;
        
        //Initialise arrays
        gridArray = new int[width,height];
        // debugTextArray = new TextMesh[width,height];

        deltaT = .02f;
        v = 343/10f;
        nextPressure     = new float[width,height];
        currentPressure  = new float[width,height];
        previousPressure = new float[width,height];
        velocities       = new float[width,height];

        updateWalls();
    }

    // calculate which nodes of the grid are inside walls
    List<GameObject> getWalls() {
        List<GameObject> walls = new List<GameObject>();
        GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[]; //will return an array of all GameObjects in the scene
        foreach(GameObject go in gos)
        {
            if(go.layer==12 || go.layer==8)
            {
                walls.Add(go);
            }
        } 
        return walls;
    }

    public float GetValue(int x, int y) {
        return this.currentPressure[x,y];
    }

    public float GetValue(Vector3 worldPosition) {
        getXY(worldPosition, out int x, out int y);
        return this.currentPressure[x,y];
    }

    //get world position of grid (doesn't work)
    public Vector3 GetWorldPosition(int x, int y) {
        Vector3 position = (new Vector3(x,0,y) * cellSize);
        position += offset;
        return position;
    }
    
    public void FixedUpdate() {
        // updateWalls();
    }

    // update every time step
    public void updateNodes() {
        for (int x = width-1; x >= 0; x--) {
            for (int y = height-1; y >= 0; y--) {
                float x_1 = 0;
                float x_2 = 0;
                float y_1 = 0;
                float y_2 = 0;
                // handle edge cases
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
                // god equation -- calculates next node based on the four neighbouring nodes and previous node info
                float nextValue = 2*currentPressure[x,y] - previousPressure[x,y] + deltaT*deltaT * velocities[x,y]*velocities[x,y] * ((x_1 - 2*currentPressure[x,y] + x_2) + (y_1 - 2*currentPressure[x,y] + y_2));
                // if the value is in a wall, don't propagate
                if (nextValue < 1 || velocities[x,y] != speedOfSound) {
                    nextPressure[x,y] = 0f;
                } else {
                    nextPressure[x,y] = nextValue;
                }
            }
        }
        // copy information for next timestep
        this.previousPressure = (float[,])currentPressure.Clone();
        this.currentPressure = (float[,])nextPressure.Clone();
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

    // insert value into grid
    public void SetValue(int x, int y, int value) {
        if (x >=0 && y >= 0 && x < width && y < height) {
            gridArray[x,y] = value;
            // debugTextArray[x,y].text = value.ToString();
            currentPressure[x,y] = value;
        }
    }

    public void getXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - offset).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - offset).z / cellSize);
    }
    public void SetValue(Vector3 worldPosition, int value) {
        int x, y;
        getXY(worldPosition, out x, out y);
        SetValue(x,y,value);
    }
}