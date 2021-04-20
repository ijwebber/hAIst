﻿using UnityEngine;
using Photon.Pun;

public class LaserController : MonoBehaviour
{
    public void DisableNearestLaser(Vector3 pos)
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

        closest.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
    }
}