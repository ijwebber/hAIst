using UnityEngine;
using Photon.Pun;

public class IntentActions : MonoBehaviourPun
{    

    LaserController laserController;
    GameCameraController cameraController;

    GameController gameController;

    void Start() {
        laserController = GameObject.FindObjectOfType<LaserController>();
        cameraController = GameObject.FindObjectOfType<GameCameraController>();
        gameController = GameObject.FindObjectOfType<GameController>();
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
            gameController.playerUpdates.updateDisplay("Can't find a camera that is enabled to disable!");
        } else if (cameraResult == DisableCameraResult.TOO_FAR) {
            gameController.playerUpdates.updateDisplay("You need to get closer to the camera!");
        } else {
            gameController.playerUpdates.updateDisplay("I've switched the camera off");
        }
    }

    public void EnableCamera()
    {
        gameController.playerUpdates.updateDisplay("Why would I do that!");
    }

    public void DisableLaser() {
        LaserDisableResult laserResult = laserController.DisableNearestLaser(transform.position);
        if (laserResult == LaserDisableResult.NOT_FOUND) {
            gameController.playerUpdates.updateDisplay("Can't find a laser that is enabled to disable!");
        } else if (laserResult == LaserDisableResult.TOO_FAR) {
            gameController.playerUpdates.updateDisplay("You need to get closer to the laser!");
        } else if (laserResult == LaserDisableResult.ERROR) {
            gameController.playerUpdates.updateDisplay("Can't turn that one off!");
        } else {
            gameController.playerUpdates.updateDisplay("I've switched the laser off");
        }
    }

    public void EnableLaser() {
        gameController.playerUpdates.updateDisplay("Why would I do that!");
    }

    public void Unsure() {
        gameController.playerUpdates.updateDisplay("I am unsure as to what to do!");
    }
}
