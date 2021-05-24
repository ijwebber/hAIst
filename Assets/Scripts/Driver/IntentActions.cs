using UnityEngine;
using Photon.Pun;

public class IntentActions : MonoBehaviourPun
{    

    LaserController laserController;
    GameCameraController cameraController;

    GameController gameController;

    void Start() {
        // Find all of the controllers
        laserController = GameObject.FindObjectOfType<LaserController>();
        cameraController = GameObject.FindObjectOfType<GameCameraController>();
        gameController = GameObject.FindObjectOfType<GameController>();
    }

    void Update() {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        
        // Testing controls
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

    // Receives intent and score from JavaScript and carries out goal
    public void CarryOutIntent(string topIntent, float score) {

        // If the confidence is higher than 0.3, carry out the intent.
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
            gameController.playerUpdates.updateDisplay("Driver: I can't find a camera that is enabled to disable!");
        } else if (cameraResult == DisableCameraResult.TOO_FAR) {
            gameController.playerUpdates.updateDisplay("Driver: You need to get closer to the camera!");
        } else {
            gameController.playerUpdates.updateDisplay("Driver: I've switched the camera off");
        }
    }

    public void EnableCamera()
    {
        gameController.playerUpdates.updateDisplay("Driver: Why would I turn on a camera! Stop wasting time!");
    }

    public void DisableLaser() {
        LaserDisableResult laserResult = laserController.DisableNearestLaser(transform.position);
        if (laserResult == LaserDisableResult.NOT_FOUND) {
            gameController.playerUpdates.updateDisplay("Driver: Can't find a laser that is enabled to disable!");
        } else if (laserResult == LaserDisableResult.TOO_FAR) {
            gameController.playerUpdates.updateDisplay("Driver: You need to get closer to the laser!");
        } else if (laserResult == LaserDisableResult.ERROR) {
            gameController.playerUpdates.updateDisplay("Driver: I can't switch that laser off, look around for another way!");
        } else {
            gameController.playerUpdates.updateDisplay("Driver: I've switched the laser off");
        }
    }

    public void EnableLaser() {
        gameController.playerUpdates.updateDisplay("Driver: Why would I switch the laser back on!");
    }

    public void Unsure() {
        gameController.playerUpdates.updateDisplay("Driver: I do not know what you want me to do!");
    }
}
