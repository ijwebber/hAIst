using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPad : MonoBehaviour
{

    public int id;

    public string code;
    public bool codeCorrect;

    public GameObject keycodeGame;
    public CodeDisplayObject codeDisplay;
    public float timeElapsed;

    // Start is called before the first frame update
    void Start()
    {
        codeCorrect = false;
        // StartCoroutine(getCode());
    }

    void Update() {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > 5) {
            code = string.Empty;
            timeElapsed -= 5;

            for (int i = 0; i < keycodeGame.GetComponent<KeycodeTask>().codeLength; i++)
            {
                code += Random.Range(1, 10);
            }
            if (codeDisplay != null) {
                codeDisplay.keypad.code = code;
                // Debug.Log(id + " // " + code);
            }
        }
    }

    IEnumerator getCode() {
        yield return new WaitForSeconds(5);
        StartCoroutine(getCode());
    }

}
