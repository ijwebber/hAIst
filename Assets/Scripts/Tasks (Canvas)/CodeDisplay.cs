using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// display the timer for the code reset
public class CodeDisplay : MonoBehaviour
{

    public Text _cardCode;

    public int keypadID;
    public KeyPad myKeypad;

    void Update() {
        if (this.enabled) {
            // Debug.Log("THIS WAS CALLED");
            KeyPad[] keypads = GameObject.FindObjectsOfType<KeyPad>();

            foreach (KeyPad keypad in keypads)
            {
                if (keypad.id == keypadID)
                {
                    myKeypad = keypad;
                    _cardCode.text = keypad.code;
                }
            }
        }

    }
}
