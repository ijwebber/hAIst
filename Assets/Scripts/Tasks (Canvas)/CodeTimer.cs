using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeTimer : MonoBehaviour
{

    public Image fillImage;
    public CodeDisplay code;
    public float fillAmount;

    void Update() {
        if(code.enabled) {
            fillAmount = code.myKeypad.timeElapsed/code.myKeypad.resetTime;
            fillImage.fillAmount = 1-fillAmount;
        }
    }
}
