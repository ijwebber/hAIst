using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
    {
        public GameObject player;
        public float cameraHeight = 12.0f;
        public float z_offset = -8.0f;
  
        void Update() {

            Vector3 pos = player.transform.position;
            pos.y += cameraHeight;
            pos.z += z_offset;
            transform.position = pos;
      }
  }