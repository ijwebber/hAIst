using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FlashlightSwitch : MonoBehaviourPun
{
    public Light flashlight;
    float initialIntensity;

    void Start() {
        initialIntensity = flashlight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            if (Input.GetKeyDown(KeyCode.F)) {
                bool isOn = flashlight.intensity == initialIntensity;

                if (isOn) {
                    flashlight.intensity = 0;
                } else {
                    flashlight.intensity = initialIntensity;
                }
            }  
        }        
    }
}
