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
        if (Input.GetKeyDown("k")) {
            Vector3 playerPosition = player.transform.position;
            grid.SetValue(playerPosition, 60);
        }
        if (Input.GetKeyDown("l")) {
            Vector3 playerPosition = player.transform.position;
            grid.SetValue(playerPosition, 30);
        }
        if (Input.GetKeyDown("j")) {
            Vector3 playerPosition = player.transform.position;
            grid.SetValue(playerPosition, 120);
        }
    }

    void FixedUpdate() {
        grid.updateNodes();
    }
}