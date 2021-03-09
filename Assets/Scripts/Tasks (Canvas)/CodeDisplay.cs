using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeDisplay : MonoBehaviour
{

    public Text _cardCode;

    public int keypadID = 0;

    private void OnEnable()
    {
        KeyPad[] keypads = GameObject.FindObjectsOfType<KeyPad>();

        foreach (KeyPad keypad in keypads)
        {
            if (keypad.id == keypadID)
            {
                _cardCode.text = keypad.code;
            }
        }
    }
}
