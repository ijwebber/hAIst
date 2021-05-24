using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Wires : MonoBehaviour
{
    public int id;
    public bool complete;

    public bool correct;

    //public int[] sequence;
    public int wire;

    // Start is called before the first frame update
    void Start()
    {
        complete = false;
        correct = false;
        if (PhotonNetwork.IsMasterClient)
        {
            wire = Random.Range(0, 4);
            this.gameObject.GetComponent<PhotonView>().RPC("SendWire", RpcTarget.Others, id, wire);
        }
    }

    // Completes wire game for everyone
    [PunRPC]
    void updateWires(int id, bool correct)
    {
        if (id == this.id)
        {
            this.complete = true;
            this.correct = correct;
        }
    }

    // Sends correct wire to everyone
    [PunRPC]
    void SendWire(int queryId, int wire)
    {
        if (queryId == id)
        {
            this.wire = wire;
        }
    }
}
