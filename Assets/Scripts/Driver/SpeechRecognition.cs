using System.Runtime.InteropServices;
using UnityEngine;
using Photon.Pun;

public class SpeechRecognition : MonoBehaviourPun
{
    public string currentText;

    bool listening = false;

    [DllImport("__Internal")]
    private static extern void StartListening();
    [DllImport("__Internal")]
    private static extern void StopListening();

    public IntentActions intentActions;

    void Update() {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Y)) {
            if (!listening) {
                listening = true;
                StartListening();
            }
        } else {
            if (listening) {
                listening = false;
                StopListening();
            }
        }
    }

    /// <summary>
    /// The browser sends the result of speech recognition in this role.
    /// </summary>
    /// <param name="info"></param>
    public void GetIntent(string info)
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        
        string[] infoSplit = info.Split('|');
        string intent = infoSplit[0];
        float score = float.Parse(infoSplit[1]);
        intentActions.CarryOutIntent(intent, score);
    }

    /// <summary>
    /// The browser sends the result of speech recognition in this role.
    /// </summary>
    /// <param name="result"></param>
    public void GetText(string result)
    {
        Debug.Log("£££ " + result);
    }

}
