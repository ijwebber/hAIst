using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Argon : MonoBehaviourPun
{
    [SerializeField] private List<GameObject> argonZones;
    public bool tripped = false;
    public void fillArgon() {
        if (!tripped) {
            this.photonView.RPC("argonFill", RpcTarget.All);
        }
    }

    [PunRPC]
    void argonFill() {
        tripped = true;
        foreach (var argon in argonZones)
        {
            argon.GetComponent<Animator>().SetTrigger("fillArgon");
        }
    }
}
