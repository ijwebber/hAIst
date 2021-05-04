using UnityEngine;
using Photon.Pun;

public class CameraProps : MonoBehaviourPun
{
    public bool disabled;

    [PunRPC]
    public void setDisabled(bool value) {
        disabled = value;
    }
}
