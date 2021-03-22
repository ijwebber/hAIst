using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LaserButton : MonoBehaviour
{
    public PressButton button;

    // Update is called once per frame
    void Update()
    {
        if (button.done)
        {
            this.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.Others);
            this.GetComponent<LineRenderer>().enabled = false;
            if (this.GetComponent<Laser>() != null)
            {
                this.GetComponent<Laser>().disabled = true;
            } else
            {
                this.GetComponent<LaserDown>().disabled = true;
            }
        }
    }
}
