using UnityEngine;
using Photon.Pun;
using System;

public enum LaserDisableResult {
    SUCCESS = 0,
    NOT_FOUND = 1,
    TOO_FAR = 2,
    ERROR = 3
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
                if (distanceFromLine(closestLaser.GetComponent<Transform>().position, closestLaser.hit.point, pos) < maxDistanceToDisable)
                {
                    if (closestLaser.GetComponent<LaserButton>() == null && closestLaser.GetComponent<LaserKey>() == null)
                    {
                        closestLaser.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
                        return LaserDisableResult.SUCCESS;
                    }
                    else
                    {
                        return LaserDisableResult.ERROR;
                    }
                }
                else
                {
                    return LaserDisableResult.TOO_FAR;
                }
            }
            else
            {
                if (distanceFromLine(closestLaserDown.GetComponent<Transform>().position, closestLaserDown.hit.point, pos) < maxDistanceToDisable)
                {
                    if (closestLaserDown.GetComponent<LaserButton>() == null && closestLaserDown.GetComponent<LaserKey>() == null)
                    {
                        closestLaserDown.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
                        return LaserDisableResult.SUCCESS;
                    }
                    else
                    {
                        return LaserDisableResult.ERROR;
                    }
                }
                else
                {
                    return LaserDisableResult.TOO_FAR;
                }
            }
        }
        else if (closestLaser != null)
        {
            if (distanceFromLine(closestLaser.GetComponent<Transform>().position, closestLaser.hit.point, pos) < maxDistanceToDisable)
            {
                if (closestLaser.GetComponent<LaserButton>() == null && closestLaser.GetComponent<LaserKey>() == null)
                {
                    closestLaser.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
                    return LaserDisableResult.SUCCESS;
                }
                else
                {
                    return LaserDisableResult.ERROR;
                }
            }
            else
            {
                return LaserDisableResult.TOO_FAR;
            }
        }
        else if (closestLaserDown != null)
        {
            if (distanceFromLine(closestLaserDown.GetComponent<Transform>().position, closestLaserDown.hit.point, pos) < maxDistanceToDisable)
            {
                if (closestLaserDown.GetComponent<LaserButton>() == null && closestLaserDown.GetComponent<LaserKey>() == null)
                {
                    closestLaserDown.GetComponent<PhotonView>().RPC("disableLaser", RpcTarget.All);
                    return LaserDisableResult.SUCCESS;
                }
                else
                {
                    return LaserDisableResult.ERROR;
                }
            }
            else
            {
                return LaserDisableResult.TOO_FAR;
            }
        } else {
            return LaserDisableResult.NOT_FOUND;
        }
    }

    private double distanceFromLine(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector2 startp = new Vector2(p1.x, p1.z);
        Vector2 endp = new Vector2(p2.x, p2.z);
        Vector2 p = new Vector2(p3.x, p3.z);

        double a1 = Vector2.Angle(p2 - p1, p2 - p3);
        double a2 = Vector2.Angle(p1 - p2, p1 - p3);

        if (a1 < 90 && a2 < 90)
        {
            return perpendicularDistance(p1, p2, p3);
        }
        else if (a1 < 90)
        {
            return Vector3.Distance(p1, p3);
        }
        else
        {
            return Vector3.Distance(p2, p3);
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
