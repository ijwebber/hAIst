using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class SoundController : MonoBehaviour
{
    [SerializeField] public SoundVisual soundVis;
    public Grid grid;
    private PhotonView photonView;
    private Vector3 soundSource;
    public GameObject player;
    public GameObject gridContainer;
    public int maxVolume;

#if UNITY_WEBGL && !UNITY_EDITOR
        void Awake()
        {
            photonView = photonView.Get(this);
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
            photonView.RPC("updateGrid", RpcTarget.MasterClient, playerPosition, 240);
            //grid.SetValue(playerPosition, 240);
        }
        photonView.RPC("newGrid", RpcTarget.Others, grid);
    }

    [PunRPC]
    void updateGrid(Vector3 position, int intensity) {
        grid.SetValue(position, intensity);
        photonView.RPC("newGrid", RpcTarget.Others, grid);
        grid.updateNodes();
    }

    [PunRPC]
    void newGrid(Grid newGrid) {
        // photonView.RPC("newGrid", RpcTarget.Others, grid);
        grid = newGrid;
        grid.updateNodes();
    }
    void FixedUpdate() {
        // grid.updateNodes();
        // soundVis.SetGrid(grid);
        // if (soundSource != new Vector3(-1,-1,-1)) {
        //     grid.SetValue(soundSource, 240);
        // }
    }
}