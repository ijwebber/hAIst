using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSin : MonoBehaviour
{

    new Light light;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light>();
        StartCoroutine(onOff());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator onOff()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            if (light.intensity == 1f)
            {
                light.intensity = 0;
            }
            else light.intensity = 1;
            
        }
    }
}
