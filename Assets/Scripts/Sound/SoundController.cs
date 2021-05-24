using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class SoundController : MonoBehaviourPun
{
    [SerializeField] public SoundVisual soundVis;
    [SerializeField] private DBControllerGame DB_Controller;
    public PlayerController playerController;
    public int multiplier = 240;
    public int threshold = 2;
    public Grid grid;
    public GuardController localSoundGrid;
    private bool soundEnabled = false;
    // public GuardMovement guardController;
    public GameObject gridContainer;
    public int maxVolume;
    float timeElapsed;
    
        void Awake()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            // ask for permissions
            Microphone.Init();
            Microphone.QueryAudioInput();
            //maxVolume = 0;
#endif
        }

#if UNITY_WEBGL && !UNITY_EDITOR
            float[] volumes = Microphone.volumes;
#endif

    // on start
    void Start() {
        grid = new Grid(202,122,1f);
        soundVis.initGrid(grid);
        this.playerController = GameObject.FindObjectOfType<PlayerController>();
        this.localSoundGrid = GameObject.FindObjectOfType<GuardController>();
        DB_Controller = GameObject.FindObjectOfType<DBControllerGame>();
        DB_Controller.getThresholds(PhotonNetwork.NickName);
        InvokeRepeating("timer", 0.01f, 0.02f);
        // StartCoroutine(timer());
        // guardController.setGrid(grid);
    }

    //function done every frame
    void Update() {
        // update sound visualisation
        soundVis.SetGrid();
#if UNITY_WEBGL && !UNITY_EDITOR
        Microphone.Update();
        // send microphone volume if above threshold
        // create new sound emission at location from microphone volume
        if (Microphone.volumes[0]*multiplier > threshold && !this.playerController.isDisabled) {
            sendGrid(playerController.player.transform.position, Mathf.FloorToInt(Microphone.volumes[0]*multiplier*(playerController.ninjaMultiplier)));
        }
#endif
        // testing inputs
        // if (Input.GetKeyDown("j") && !this.playerController.isDisabled) {
        //     sendGrid(playerController.player.transform.position, (int)(240));
        // }
        // if (Input.GetKeyDown("k") && !this.playerController.isDisabled) {
        //     sendGrid(playerController.player.transform.position, 60);
        // }
        // if (Input.GetKeyDown("l") && !this.playerController.isDisabled) {
        //     sendGrid(playerController.player.transform.position, 30);
        // }
    }

    public void sendGrid(Vector3 playerPosition, int intensity) {
        // send new sound source to other clients
        if (!playerController.isDisabled && soundEnabled) {
            localSoundGrid.setValue(playerPosition, intensity);
            this.photonView.RPC("updateGrid", RpcTarget.All, playerPosition.x, playerPosition.y, playerPosition.z, intensity);
        }
        // set value in local grid
        // grid.SetValue(playerPosition, intensity);
    }

    public void enableSound(bool val) {
        soundEnabled = val;
    }

    public bool getSoundValue()
    {
        return soundEnabled;
    }

    [PunRPC]
    void updateGrid(float x, float y, float z, int intensity) {
        // receive new sound source and update local grid
        grid.SetValue(new Vector3(x,y,z), intensity);
    }

    // every time step (every 50th of a second) update nodes
    private void timer() {
        grid.updateNodes();
    }
}