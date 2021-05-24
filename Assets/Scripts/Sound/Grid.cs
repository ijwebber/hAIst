using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Node {
    public float pressure;
}

public class Grid {
    private int width;
    private int obstacleMask = (1 << 8) | (1 << 20);
    private int argonMask = (1 << 21);
    public int[] averages = new int[500];
    private int height;
    private float maxPressure = 240;
    private double deltaT = 0.02;
    private double dt2;
    private double speedOfSound = 343/10; // this isn't actually the speed of sound, but it's the one that works best
    private double speedOfSoundArgon = 343/15; // this isn't actually the speed of sound, but it's the one that works best
    private int[,] gridArray;
    private float cellSize;
    public Vector3 offset = new Vector3(-100.5f,11.7f,-60f); // offset for bottom left grid tile (dont change z for some reason idk why)
    // private TextMesh[,] debugTextArray;
    public double[,] currentPressure, previousPressure, nextPressure, velocities;

    public double[,] getPressure() {
        return this.currentPressure;
    }
    public void setPressure(double[,] newPressure) {
        this.currentPressure = newPressure;
    }

    // update walls (implemented for moving walls or obstacles) Calculates which nodes are inside a wall
    public void updateWalls() {
        // populate grid
        for (int i = 0; i < gridArray.GetLength(0)-2; i++) {
            for (int j = 0; j < gridArray.GetLength(1) - 2; j++) {
                Vector3 point1 = new Vector3(i*cellSize, 1, j*cellSize) + offset;
                if (Physics.CheckSphere(point1, cellSize/4, obstacleMask)) {
                    velocities[i,j] = 0;
                } else if (Physics.CheckSphere(point1, cellSize/4, argonMask)){
                    velocities[i,j] = speedOfSoundArgon;
                } else {
                    velocities[i,j] = speedOfSound;
                }
            }
        }
    }

    // initialise grid
    public Grid(int width, int height, float inCellSize) {
        this.width = width;
        this.height = height;
        this.cellSize = inCellSize;

        dt2 = deltaT*deltaT;
        
        //Initialise arrays
        gridArray = new int[width,height];
        // debugTextArray = new TextMesh[width,height];

        nextPressure     = new double[width,height];
        currentPressure  = new double[width,height];
        previousPressure = new double[width,height];
        velocities       = new double[width,height];
        for (int i = 0; i < averages.Length; i++)
        {
            averages[i] = 0;
        }
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

    // return value at specified grid location
    public double GetValue(int x, int y) {
        try
        {
            return this.currentPressure[x,y];
        }
        catch (System.Exception)
        {
            Debug.Log("out of bounds");
            Debug.Log(x + " // " + y);
            Debug.Log(this.currentPressure.GetLength(0) + " // " + this.currentPressure.GetLength(1));
            
            
            throw;
        }
    }

    // return speed at specified grid location
    public double getSpeed(int x, int y) {
        return this.velocities[x,y];
    }

    // return value at world position (needs to calculate grid location)
    public double GetValue(Vector3 worldPosition) {
        getXY(worldPosition, out int x, out int y);
        return this.currentPressure[x,y];
    }
    // kernel for sound mesh
    public double GetAvgValue(int x, int y) {
        double value = 0;
        if (x-1 > 0) {
            value += 2*this.currentPressure[x-1,y];
            if (y-1 > 0) {
                value += this.currentPressure[x-1,y-1];
            }
            if (y+1 < gridArray.GetLength(1)) {
                value += this.currentPressure[x-1,y+1];
            }
        }
        if (x+1 < gridArray.GetLength(0)) {
            value += 2*this.currentPressure[x+1,y];
            if (y-1 > 0) {
                value += this.currentPressure[x+1,y-1];
            }
            if (y+1 < gridArray.GetLength(1)) {
                value += this.currentPressure[x+1,y+1];
            }
        }
        if (y-1 > 0) {
            value += 2*this.currentPressure[x,y-1];
        }
        if (y+1 < gridArray.GetLength(1)) {
            value += 2*this.currentPressure[x,y+1];
        }
        value += 6*this.currentPressure[x,y];
        value /= 9;
        return value;
    }

    //translate grid location into world location
    public Vector3 GetWorldPosition(int x, int y) {
        Vector3 position = (new Vector3(x,0,y) * cellSize);
        position += offset;
        return position;
    }

    public float getVelocity(int x, int y) {
        return (float)this.velocities[x,y];
    }

    // update every time step. The big propagatino function
    public void updateNodes() {
        for (int x = width-2; x >= 1; x--) {
            for (int y = height-2; y >= 1; y--) {
                if (velocities[x,y] > 0) {
                    // god equation -- calculates next node based on the four neighbouring nodes and previous node info
                    double v2 = velocities[x,y]*velocities[x,y];
                    double nextValue = 2*currentPressure[x,y] - previousPressure[x,y] + (dt2 * v2) *
                        ((currentPressure[x+1,y] - 2*currentPressure[x,y] + currentPressure[x-1,y])
                            + (currentPressure[x,y+1] - 2*currentPressure[x,y] + currentPressure[x,y-1]));
                    // if the value is in a wall, don't propagate. ALso cull below .1 dB
                    if (nextValue > .1f) {
                        nextPressure[x,y] = nextValue;
                    } else {
                        nextPressure[x,y] = 0;
                    }
                } else {
                    nextPressure[x,y] = 0;
                }
            }
        }
        // copy information for next timestep
        this.previousPressure = (double[,])currentPressure.Clone();
        this.currentPressure = (double[,])nextPressure.Clone();
    }

    // testing
    public void getAverages() {
        for (int i = 0; i < averages.Length; i++)
        {
            if (averages[i] > 0) {
                Debug.Log(i + " // " + averages[i]);
            }
        }
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
            gridArray[x,y] = value/5;
            // debugTextArray[x,y].text = value.ToString();
            currentPressure[x,y] = value/5;
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