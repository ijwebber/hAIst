using UnityEngine;
using Photon.Pun;

public class FlashlightSwitch : MonoBehaviourPun
{
    public Light flashlight;
    float initialIntensity;
    public Light flashlight2;
    float initialIntensity2;

    void Start() {
        initialIntensity = flashlight.intensity;
        initialIntensity2 = flashlight2.intensity;
    }

    void Update()
    {
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            if (Input.GetKeyDown(KeyCode.F)) {
                bool isOn = flashlight.intensity == initialIntensity;

                if (isOn) {
                    flashlight.intensity = 0;
                    flashlight2.intensity = 0;
                } else {
                    flashlight.intensity = initialIntensity;
                    flashlight2.intensity = initialIntensity2;
                }
            }  
        }        
    }
}
