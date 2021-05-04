using UnityEngine;
using Photon.Pun;

public class IntentActions : MonoBehaviourPun
{    

    LaserController laserController;
    GameCameraController cameraController;

    void Start() {
        laserController = GameObject.FindObjectOfType<LaserController>();
        cameraController = GameObject.FindObjectOfType<GameCameraController>();
    }

    void Update() {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            Debug.Log("*** Switching Off");
            laserController.DisableNearestLaser(this.GetComponent<Transform>().position);
        }

        if (Input.GetKeyDown(KeyCode.N)) {
            Debug.Log("*** Switching On");
            EnableLasers();
        }

        if (Input.GetKeyDown(KeyCode.U)) {
            cameraController.DisableClosestCamera(transform.position);
        }

    }

    public void CarryOutIntent(string topIntent, float score) {
        if (score > 0.3) {
            switch (topIntent)
            {
                case "ShowMap":
                    ShowMap();
                    break;
                case "HideMap":
                    HideMap();
                    break;
                case "EnableLasers":
                    EnableLasers();
                    break;
                case "DisableLasers":
                    laserController.DisableNearestLaser(this.GetComponent<Transform>().position);
                    break;
                default:
                    Unsure();
                    break;
            }
        } else {
            Unsure();
        }
    }

    public void ShowMap() {
        Debug.Log("I need to show you the map!");
    }

    public void HideMap() {
        Debug.Log("I need to hide the map from you!");
    }

    public void EnableLasers() {
        Laser[] lasers = GameObject.FindObjectsOfType<Laser>();
        LaserDown[] lasersDown = GameObject.FindObjectsOfType<LaserDown>();
        
        foreach (Laser laser in lasers)
        {
            laser.GetComponent<PhotonView>().RPC("enableLaser", RpcTarget.All);
        }

        foreach (LaserDown laser in lasersDown)
        {
            laser.GetComponent<PhotonView>().RPC("enableLaser", RpcTarget.All);
        }
    }

    public void DisableLasers() {
        Laser[] lasers = GameObject.FindObjectsOfType<Laser>();
        LaserDown[] lasersDown = GameObject.FindObjectsOfType<LaserDown>();
        
        foreach (Laser laser in lasers)
        {
            laser.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
        }

        foreach (LaserDown laser in lasersDown)
        {
            laser.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
        }
    }

    public void Unsure() {
        Debug.Log("I am unsure as to what to do!");
    }
}
