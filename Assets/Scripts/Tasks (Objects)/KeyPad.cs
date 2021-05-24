using System.Collections;
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
    public float resetTime = 7;

    // Start is called before the first frame update
    void Start()
    {
        codeCorrect = false;
        if(PhotonNetwork.IsMasterClient) {
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
        // StartCoroutine(getCode());
    }

    void Update() {
        // Generate new code every resetTime seconds
        timeElapsed += Time.deltaTime;
        if (timeElapsed > resetTime) {
            timeElapsed -= resetTime;
            if(PhotonNetwork.IsMasterClient) {
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
    }

    // Completes keypad game for everyone
    [PunRPC]
    void updateKeyCode(int id) {
        if (id == this.id) {
            codeCorrect = true;
        }
    }

    // Sends new code to everyone
    [PunRPC]
    void SendCode(int queryId, string code) {
        if (queryId == id) {
            this.code = code;
            timeElapsed = 0;
        }
    }

    IEnumerator getCode() {
        yield return new WaitForSeconds(5);
        StartCoroutine(getCode());
    }

}
