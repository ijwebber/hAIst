using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LaserController : MonoBehaviour
{

    void DisableNearestLaser(Vector3 pos)
    {
        Laser closest = null;
        Laser[] lasers = GameObject.FindObjectsOfType<Laser>();
        foreach (Laser laser in lasers)
        {
            if (closest == null || Vector3.Distance(laser.GetComponent<Transform>().position, pos) < Vector3.Distance(closest.GetComponent<Transform>().position, pos))
            {
                closest = laser;
            }
        }

        closest.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.Others);
        closest.GetComponent<LineRenderer>().enabled = false;
        if (closest.GetComponent<Laser>() != null)
        {
            closest.GetComponent<Laser>().disabled = true;
        }
        else
        {
            closest.GetComponent<LaserDown>().disabled = true;
        }
    }
}
