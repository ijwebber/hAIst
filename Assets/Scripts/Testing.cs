using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private Grid grid;
    public GameObject player;
    public GameObject gridContainer;
    void Start() {
        grid = new Grid(100,50,1f, gridContainer);
    }

    void Update() {
        if (Input.GetKeyDown("l")) {
            Vector3 playerPosition = player.transform.position;
            grid.SetValue(playerPosition, 56);
        }
    }
}