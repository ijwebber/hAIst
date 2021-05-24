using UnityEngine;
using Photon.Pun;

public class CameraProps : MonoBehaviourPun
{
    public bool disabled;
    private float disabledTime;
    public float disabledLength = 20f;

    // Checks whether to renable the camera or not.
    void Update() {
        if (disabled && Time.time - disabledTime > disabledLength) {
            disabled = false;
            this.GetComponent<CameraFOV>().cameraState = State.disabled;
        }
    }


    // Changes whether the camera is disabled or not for everyone.
    [PunRPC]
    public void setDisabled(bool value) {
        this.GetComponent<CameraFOV>().cameraState = State.disabled;
        disabled = value;
        disabledTime = Time.time;
    }
}
