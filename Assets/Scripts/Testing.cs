using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] public SoundVisual soundVis;
    private Grid grid;
    private Vector3 soundSource;
    public GameObject player;
    public GameObject gridContainer;
    void Start() {
        player = GameObject.Find("Timmy");
        grid = new Grid(110,60,1f, gridContainer);
        soundSource = new Vector3(-1,-1,-1);
        // Debug.Log(GameObject.Find("Timmy"));

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
            soundSource = playerPosition;
            grid.SetValue(playerPosition, 240);
        }
    }

    void FixedUpdate() {
        grid.updateNodes();
        soundVis.SetGrid(grid);
        // if (soundSource != new Vector3(-1,-1,-1)) {
        //     grid.SetValue(soundSource, 240);
        // }
    }
}