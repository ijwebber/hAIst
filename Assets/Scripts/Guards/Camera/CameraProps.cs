using UnityEngine;
using Photon.Pun;

public class CameraProps : MonoBehaviourPun
{
    public bool disabled;
    private float disabledTime;
    public float disabledLength = 20f;

    void Update() {
        if (disabled && Time.time - disabledTime > disabledLength) {
            disabled = false;
        }
    }

    [PunRPC]
    public void setDisabled(bool value) {
        disabled = value;
        disabledTime = Time.time;
    }
}
