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
            DisableLaser();
        }

        if (Input.GetKeyDown(KeyCode.N)) {
            EnableLaser();
        }

        if (Input.GetKeyDown(KeyCode.U)) {
            DisableCamera();
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
                    DisableCamera();
                    break;
                case "EnableLaser":
                    EnableLaser();
                    break;
                case "DisableLaser":
                    DisableLaser();
                    break;
                default:
                    Unsure();
                    break;
            }
        } else {
            Unsure();
        }
    }

    public void DisableCamera() {
        DisableCameraResult cameraResult = cameraController.DisableClosestCamera(transform.position);
        if (cameraResult == DisableCameraResult.NOT_FOUND) {
            Debug.Log("*** Can't find a camera that is enabled to disable!");
        } else if (cameraResult == DisableCameraResult.TOO_FAR) {
            Debug.Log("*** You need to get closer to the camera!");
        } else {
            Debug.Log("*** I've switched the camera off");
        }
    }

    public void EnableCamera()
    {
        Debug.Log("*** Why would I do that!");
    }

    public void DisableLaser() {
        LaserDisableResult laserResult = laserController.DisableNearestLaser(transform.position);
        if (laserResult == LaserDisableResult.NOT_FOUND) {
            Debug.Log("*** Can't find a laser that is enabled to disable!");
        } else if (laserResult == LaserDisableResult.TOO_FAR) {
            Debug.Log("*** You need to get closer to the laser!");
        } else {
            Debug.Log("*** I've switched the laser off");
        }
    }

    public void EnableLaser() {
        Debug.Log("*** Why would I do that!");
    }

    public void Unsure() {
        Debug.Log("*** + I am unsure as to what to do!");
    }
}
