﻿using UnityEngine;
using System.Collections.Generic;

public class GameCameraController : MonoBehaviour
{
    List<GameObject> Cameras;

    void Start() {
        Cameras = new List<GameObject>();

        foreach (Transform child in transform) {
            Cameras.Add(child.Find("Camera").gameObject);
        }
    }

    public bool DisableClosestCamera(Vector3 playerPos) {
        List<GameObject> enabledCameras = new List<GameObject>();

        foreach (GameObject camera in Cameras)
        {
            if (!camera.GetComponent<CameraProps>().disabled) {
                enabledCameras.Add(camera);
            }
        }

        if (enabledCameras.Count == 0) {
            return false;
        } else {
            GameObject closest = Cameras[0];
            float dist = Vector3.Distance(playerPos, Cameras[0].transform.position);
            
            foreach (GameObject camera in Cameras)
            {
                float newDist = Vector3.Distance(playerPos, camera.transform.position);

                if (newDist < dist) {
                    closest = camera;
                    dist = newDist;
                }
            }

            if (dist > 10) {
                return false;
            } else {
                closest.GetComponent<CameraProps>().disabled = true;
            }
        }
        
        return true;
    }
}