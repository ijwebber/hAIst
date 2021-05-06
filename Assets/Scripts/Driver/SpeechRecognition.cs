using System.Runtime.InteropServices;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SpeechRecognition : MonoBehaviourPun
{
    public string currentText;

    bool listening = false;
    bool shownMessage = false;

    [DllImport("__Internal")]
    private static extern void StartListening();
    [DllImport("__Internal")]
    private static extern void StopListening();

    public IntentActions intentActions;

    private GameController gameController;

    void Start() {
        gameController = GameObject.FindObjectOfType<GameController>();
    }

    void Update() {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Y)) {
            bool isDriverBusy = (bool) PhotonNetwork.CurrentRoom.CustomProperties["isDriverBusy"];
            if (!listening && !isDriverBusy) {
                listening = true;
                Hashtable setBusy = new Hashtable() {{"isDriverBusy", true}};
                PhotonNetwork.CurrentRoom.SetCustomProperties(setBusy);
                StartListening(); 
            } else if (!listening && isDriverBusy && !shownMessage) {
                gameController.playerUpdates.updateDisplay("The driver is busy talking to someone else!");
                shownMessage = true;
            }
        } else {
            shownMessage = false;
            if (listening) {
                listening = false;
                Hashtable setBusy = new Hashtable() {{"isDriverBusy", false}};
                PhotonNetwork.CurrentRoom.SetCustomProperties(setBusy);
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
