using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPad : MonoBehaviour
{

    public int id;

    public string code;
    public bool codeCorrect;

    public GameObject keycodeGame;

    // Start is called before the first frame update
    void Start()
    {
        code = string.Empty;
        codeCorrect = false;

        for (int i = 0; i < keycodeGame.GetComponent<KeycodeTask>().codeLength; i++)
        {
            code += Random.Range(1, 10);
        }
        Debug.Log(code);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
