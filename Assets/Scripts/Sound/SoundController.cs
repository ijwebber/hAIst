using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class SoundController : MonoBehaviourPun
{
    [SerializeField] public SoundVisual soundVis;
    public Grid grid;
    private Vector3 soundSource;
    public GameObject player;
    public GameObject gridContainer;
    public int maxVolume;

#if UNITY_WEBGL && !UNITY_EDITOR
        void Awake()
        {
            Microphone.Init();
            Microphone.QueryAudioInput();
            maxVolume = 0;
        }
#endif

// #if UNITY_WEBGL && !UNITY_EDITOR
        // void Update()
        // {
            // Testing.incomingVolume(maxVolume);
        // }
// #endif

#if UNITY_WEBGL && !UNITY_EDITOR
            float[] volumes = Microphone.volumes;
#endif

    void Start() {
        player = GameObject.Find("Timmy");
        #if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log(player);
        #endif
        grid = new Grid(110,60,1f);
        soundSource = new Vector3(-1,-1,-1);
        // Debug.Log(GameObject.Find("Timmy"));

    }

    void Update() {
        // grid.updateWalls();
#if UNITY_WEBGL && !UNITY_EDITOR
        Microphone.Update();
        grid.SetValue(player.transform.position, Mathf.FloorToInt(Microphone.volumes[0]*240));
#endif
        if (Input.GetKeyDown("k")) {
            Vector3 playerPosition = player.transform.position;
            //grid.SetValue(playerPosition, 60);
        }
        if (Input.GetKeyDown("l")) {
            Vector3 playerPosition = player.transform.position;
            grid.SetValue(playerPosition, 30);
        }
        if (Input.GetKeyDown("j")) {
            Vector3 playerPosition = player.transform.position;
        #if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log("!!!pressed j");
        #endif
            soundSource = playerPosition;
            this.photonView.RPC("updateGrid", RpcTarget.MasterClient, playerPosition.x, playerPosition.y, playerPosition.z, 240);
            //grid.SetValue(playerPosition, 240);
        }

        //flatten array
        // sendGrid();
        // this.photonView.RPC("newGrid", RpcTarget.Others, grid.getPressure(), grid.GetWidth(), grid.GetHeight());
    }

    void sendGrid() {
        double[] flatPressure = new double[grid.GetWidth() * grid.GetHeight()];
        double[,] pressure = grid.getPressure();
        int i = 0;
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++) {
                flatPressure[i] = pressure[x,y];
                i++;
            }
        }
        this.photonView.RPC("newGrid", RpcTarget.Others, flatPressure, grid.GetWidth(), grid.GetHeight());
    }

    [PunRPC]
    void updateGrid(float x, float y, float z, int intensity) {
        grid.SetValue(new Vector3(x,y,z), intensity);
        // sendGrid();
    }

    [PunRPC]
    void newGrid(double[] newGrid, int width, int height) {
        // photonView.RPC("newGrid", RpcTarget.Others, grid);
        double[,] pressure = new double[width,height];
        int i = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++) {
                pressure[x,y] = newGrid[i];
                i++;
            }
        }
        //serialize now
        grid.setPressure(pressure);
        // grid.updateNodes();
    }
    void FixedUpdate() {
        // if (PhotonNetwork.IsMasterClient) {
        grid.updateNodes();
        // }
        soundVis.SetGrid(grid);
        // if (soundSource != new Vector3(-1,-1,-1)) {
        //     grid.SetValue(soundSource, 240);
        // }
    }
}