using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightSwitch : MonoBehaviour
{
    Light flashlight;

    void Start()
    {
        flashlight = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) {
            bool isOn = flashlight.intensity == 1;

            if (isOn) {
                flashlight.intensity = 0;
            } else {
                flashlight.intensity = 1;
            }
        }        
    }
}
