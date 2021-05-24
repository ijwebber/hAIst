using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeDisplay : MonoBehaviour
{

    public Text _cardCode;

    public int keypadID;
    public KeyPad myKeypad;

    // private void OnEnable()
    // {
    //     KeyPad[] keypads = GameObject.FindObjectsOfType<KeyPad>();

    //     foreach (KeyPad keypad in keypads)
    //     {
    //         if (keypad.id == keypadID)
    //         {
    //             _cardCode.text = keypad.code;
    //         }
    //     }
    // }

    void Update() {
        if (this.enabled) {
            // Debug.Log("THIS WAS CALLED");
            KeyPad[] keypads = GameObject.FindObjectsOfType<KeyPad>();

            foreach (KeyPad keypad in keypads)
            {
                if (keypad.id == keypadID)
                {
                    // Keeps code up to date
                    myKeypad = keypad;
                    _cardCode.text = keypad.code;
                }
            }
        }

    }
}
