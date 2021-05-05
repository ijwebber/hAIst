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
            laserController.DisableNearestLaser(transform.position);
        }

        if (Input.GetKeyDown(KeyCode.N)) {
            Debug.Log("*** Switching On");
            EnableLaser();
        }

        if (Input.GetKeyDown(KeyCode.U)) {
            cameraController.DisableClosestCamera(transform.position);
        }

    }

    public void CarryOutIntent(string topIntent, float score) {
        if (score > 0.3) {
            switch (topIntent)
            {
                case "EnableCamera":
                    EnableCamera();
                    break;
                case "DisableCamera":
                    cameraController.DisableClosestCamera(transform.position);
                    break;
                case "EnableLaser":
                    EnableLaser();
                    break;
                case "DisableLaser":
                    laserController.DisableNearestLaser(transform.position);
                    break;
                default:
                    Unsure();
                    break;
            }
        } else {
            Unsure();
        }
    }

    public void EnableCamera()
    {
        Debug.Log("*** + Why would I do that!");
    }

    public void EnableLaser() {
        Debug.Log("*** + Why would I do that!");
    }

    public void Unsure() {
        Debug.Log("*** + I am unsure as to what to do!");
    }
}
