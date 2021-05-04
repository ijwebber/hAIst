using UnityEngine;
using Photon.Pun;

public class LaserController : MonoBehaviour
{
    public void DisableNearestLaser(Vector3 pos)
    {
        Laser closestLaser = null;
        Laser[] lasers = GameObject.FindObjectsOfType<Laser>();
        foreach (Laser laser in lasers)
        {
            if (closestLaser == null || 
                (!laser.disabled && Vector3.Distance(laser.GetComponent<Transform>().position, pos) < Vector3.Distance(closestLaser.GetComponent<Transform>().position, pos)))
            {
                closestLaser = laser;
            }
        }

        LaserDown closestLaserDown = null;
        LaserDown[] laserDowns = GameObject.FindObjectsOfType<LaserDown>();
        foreach (LaserDown laserDown in laserDowns)
        {
            if (closestLaserDown == null || 
                (!laserDown.disabled && Vector3.Distance(laserDown.GetComponent<Transform>().position, pos) < Vector3.Distance(closestLaserDown.GetComponent<Transform>().position, pos)))
            {
                closestLaserDown = laserDown;
            }
        }

        if (closestLaser != null && closestLaserDown != null)
        {
            if (Vector3.Distance(closestLaser.GetComponent<Transform>().position, pos) < Vector3.Distance(closestLaserDown.GetComponent<Transform>().position, pos))
            {
                closestLaser.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
            }
            else
            {
                closestLaserDown.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
            }
        }
        else if (closestLaser != null)
        {
            closestLaser.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
        }
        else if (closestLaserDown != null)
        {
            closestLaserDown.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
        }
    }
}
