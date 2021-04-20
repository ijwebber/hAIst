using UnityEngine;
using Photon.Pun;

public class IntentActions : MonoBehaviourPun
{    

    LaserController laserController;

    void Start() {
        laserController = GameObject.FindObjectOfType<LaserController>();
    }

    void Update() {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (Input.GetKey(KeyCode.B)) {
            Debug.Log("*** Switching Off");
            DisableLasers();
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
                    DisableLasers();
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
        Debug.Log("I have switched on the lasers!");
    }

    public void DisableLasers() {
        laserController.DisableNearestLaser(transform.position);
    }

    public void Unsure() {
        Debug.Log("I am unsure as to what to do!");
    }
}
