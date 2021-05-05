using UnityEngine;
using Photon.Pun;
using System;

public class LaserController : MonoBehaviour
{

    float maxDistanceToDisable = 20;

    public void DisableNearestLaser(Vector3 pos)
    {
        Laser closestLaser = null;
        Laser[] lasers = GameObject.FindObjectsOfType<Laser>();
        foreach (Laser laser in lasers)
        {
            float distance = Vector3.Distance(laser.GetComponent<Transform>().position, pos);

            if (!laser.disabled && distance < maxDistanceToDisable && (closestLaser == null || distance < Vector3.Distance(closestLaser.GetComponent<Transform>().position, pos)))
            {
                closestLaser = laser;
            }
        }

        LaserDown closestLaserDown = null;
        LaserDown[] laserDowns = GameObject.FindObjectsOfType<LaserDown>();
        foreach (LaserDown laserDown in laserDowns)
        {
            float distance = Vector3.Distance(laserDown.GetComponent<Transform>().position, pos);

            if (!laserDown.disabled && distance < maxDistanceToDisable && (closestLaserDown == null || distance < Vector3.Distance(closestLaserDown.GetComponent<Transform>().position, pos)))
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

    private double perpendicularDistance(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector2 startp = new Vector2(p1.x, p1.z);
        Vector2 endp = new Vector2(p2.x, p2.z);
        Vector2 p = new Vector2(p3.x, p3.z);

        double a = Vector2.Distance(startp, endp);
        double b = Vector2.Distance(startp, p);
        double c = Vector2.Distance(endp, p);

        double s = (a + b + c) / 2.0;

        double distance = 2.0 * Math.Sqrt(s * (s - a) * (s - b) * (s - c)) / a;

        return distance;
    }
}
