using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoorScript : MonoBehaviourPun
{
    public Animator anim;
    public SoundController soundController;
    // Start is called before the first frame update
    void Awake()
    {
        anim.ResetTrigger("OpenDoor");
        soundController = GameObject.FindObjectOfType<SoundController>();
    }

    public void OpenDoor() {
        anim.SetTrigger("OpenDoor");
    }

    public void CloseDoor() {
        anim.ResetTrigger("OpenDoor");
        anim.enabled = true;
    }
    
    public void updateWall() {
        if (PhotonNetwork.IsMasterClient) {
            this.photonView.RPC("updateWalls", RpcTarget.All);
        }
    }

    [PunRPC]
    void updateWalls(){
        soundController.grid.updateWalls();
    }

    void pauseAnimationEvent() {
        anim.enabled = false;
    }
}
