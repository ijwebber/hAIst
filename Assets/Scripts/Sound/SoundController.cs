using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class SoundController : MonoBehaviourPun
{
    [SerializeField] public SoundVisual soundVis;
    public Grid grid;
    public GameObject player;
    public GameObject gridContainer;
    public int maxVolume;

#if UNITY_WEBGL && !UNITY_EDITOR
        void Awake()
        {
            // ask for permissions
            Microphone.Init();
            Microphone.QueryAudioInput();
            maxVolume = 0;
        }
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
            float[] volumes = Microphone.volumes;
#endif

    // on start
    void Start() {
        player = GameObject.Find("Timmy");
        grid = new Grid(110,60,1f);
    }

    //function done every frame
    void Update() {
        // update sound visualisation
        soundVis.SetGrid(grid);
        // send microphone volume if above threshold
#if UNITY_WEBGL && !UNITY_EDITOR
        Microphone.Update();
        if (Microphone.volumes[0]*240 > 2) {
            sendGrid(player.transform.position, Mathf.FloorToInt(Microphone.volumes[0]*240));
        }
#endif
        if (Input.GetKeyDown("j")) {
            sendGrid(player.transform.position, 240);
        }
        if (Input.GetKeyDown("k")) {
            sendGrid(player.transform.position, 60);
        }
        if (Input.GetKeyDown("l")) {
            sendGrid(player.transform.position, 30);
        }

        //flatten array
        // sendGrid();
        // this.photonView.RPC("newGrid", RpcTarget.Others, grid.getPressure(), grid.GetWidth(), grid.GetHeight());
    }

    void sendGrid(Vector3 playerPosition, int intensity) {
        // send new sound source to other clients
        this.photonView.RPC("updateGrid", RpcTarget.Others, playerPosition.x, playerPosition.y, playerPosition.z, intensity);
        // set value in local grid
        grid.SetValue(playerPosition, intensity);
    }

    [PunRPC]
    void updateGrid(float x, float y, float z, int intensity) {
        // receive new sound source and update local grid
        grid.SetValue(new Vector3(x,y,z), intensity);
    }

    // every time step (every 50th of a second)
    void FixedUpdate() {
        // call propagation step
        grid.updateNodes();
    }
}