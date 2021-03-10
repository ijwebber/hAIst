﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
            timeElapsed -= 5;
            this.gameObject.GetComponent<PhotonView>().RPC("GetCode", RpcTarget.MasterClient, id);
        }
    }

    [PunRPC]
    void GetCode(int queryId) {
        if (queryId == id) {
            code = string.Empty;

            for (int i = 0; i < keycodeGame.GetComponent<KeycodeTask>().codeLength; i++)
            {
                code += Random.Range(1, 10);
            }
            if (codeDisplay != null) {
                codeDisplay.keypad.code = code;
                // Debug.Log(id + " // " + code);
            }
            this.gameObject.GetComponent<PhotonView>().RPC("SendCode",RpcTarget.Others, id, code);
        }
    }

    [PunRPC]
    void SendCode(int queryId, string code) {
        if (queryId == id) {
            this.code = code;
        }
    }

    IEnumerator getCode() {
        yield return new WaitForSeconds(5);
        StartCoroutine(getCode());
    }

}
