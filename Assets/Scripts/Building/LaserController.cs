using UnityEngine;
using Photon.Pun;

public enum LaserDisableResult {
    SUCCESS = 0,
    NOT_FOUND = 1,
    TOO_FAR = 2
}

public class LaserController : MonoBehaviour
{

    float maxDistanceToDisable = 20;

    public LaserDisableResult DisableNearestLaser(Vector3 pos)
    {
        Laser closestLaser = null;
        Laser[] lasers = GameObject.FindObjectsOfType<Laser>();
        foreach (Laser laser in lasers)
        {
            float distance = Vector3.Distance(laser.GetComponent<Transform>().position, pos);

            if (!laser.disabled && (closestLaser == null || distance < Vector3.Distance(closestLaser.GetComponent<Transform>().position, pos)))
            {
                closestLaser = laser;
            }
        }

        LaserDown closestLaserDown = null;
        LaserDown[] laserDowns = GameObject.FindObjectsOfType<LaserDown>();
        foreach (LaserDown laserDown in laserDowns)
        {
            float distance = Vector3.Distance(laserDown.GetComponent<Transform>().position, pos);

            if (!laserDown.disabled && (closestLaserDown == null || distance < Vector3.Distance(closestLaserDown.GetComponent<Transform>().position, pos)))
            {
                closestLaserDown = laserDown;
            }
        }

        if (closestLaser != null && closestLaserDown != null)
        {
            if (Vector3.Distance(closestLaser.GetComponent<Transform>().position, pos) < Vector3.Distance(closestLaserDown.GetComponent<Transform>().position, pos))
            {
                if (Vector3.Distance(closestLaser.GetComponent<Transform>().position, pos) < maxDistanceToDisable) {
                    closestLaser.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
                    return LaserDisableResult.SUCCESS;
                } else {
                    return LaserDisableResult.TOO_FAR;
                }   
            }
            else
            {   
                if (Vector3.Distance(closestLaser.GetComponent<Transform>().position, pos) < maxDistanceToDisable) {
                    closestLaserDown.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
                    return LaserDisableResult.SUCCESS;
                } else {
                    return LaserDisableResult.TOO_FAR;
                }
            }
        }
        else if (closestLaser != null)
        {
            if (Vector3.Distance(closestLaser.GetComponent<Transform>().position, pos) < maxDistanceToDisable) {
                closestLaser.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
                return LaserDisableResult.SUCCESS;
            } else {
                return LaserDisableResult.TOO_FAR;
            } 
        }
        else if (closestLaserDown != null)
        {
            if (Vector3.Distance(closestLaserDown.GetComponent<Transform>().position, pos) < maxDistanceToDisable) {
                closestLaserDown.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
                return LaserDisableResult.SUCCESS;
            } else {
                return LaserDisableResult.TOO_FAR;
            }
        } else {
            return LaserDisableResult.NOT_FOUND;
        }
    }
}
