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
            // synchronise argon animation among clients
            this.photonView.RPC("argonFill", RpcTarget.All);
        }
    }

    [PunRPC]
    void argonFill() {
        tripped = true;
        // animate argon visibility
        foreach (var argon in argonZones)
        {
            argon.GetComponent<Animator>().SetTrigger("fillArgon");
        }
    }
}
