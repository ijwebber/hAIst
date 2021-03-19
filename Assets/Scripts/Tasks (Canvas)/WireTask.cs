using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WireTask : MonoBehaviour
{
    public bool complete;
    //public int[] input;

    public int wiresID = 0;

    public void OnEnable()
    {
        complete = false;
        //input = new int[4];
    }

    public void ButtonClick(int n)
    {
        Wires[] wiress = GameObject.FindObjectsOfType<Wires>();

        foreach (Wires wires in wiress)
        {
            if (wires.id == wiresID)
            {
                wires.GetComponent<PhotonView>().RPC("updateWires", RpcTarget.Others, wiresID, ( wires.wire == n ));
                wires.complete = true;
                wires.correct = ( wires.wire == n );

                complete = true;
            }
        }
    }
}
