using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerLeave : MonoBehaviourPunCallbacks
{

    UIController uiController;

    void Start() {
        uiController = GameObject.FindObjectOfType<UIController>();
    }

    void OnTriggerEnter(Collider other) {
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            if (other.gameObject.CompareTag("ExitBox")) {
                uiController.UpdateInfoText("Ready to leave? Press E");
            }
        }
    }

    void OnTriggerStay(Collider other) {
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            if (other.gameObject.CompareTag("ExitBox")) {
                if (Input.GetKeyDown(KeyCode.E)) {
                    Hashtable hash = new Hashtable() {{"leave", true}, {"win", true}};
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                    UpdateWaitingText();
                }
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            if (other.gameObject.CompareTag("ExitBox")) {
                uiController.HideInfoBox();

                Hashtable leaveHash = new Hashtable() {{"leave", false}, {"win", false}};
                PhotonNetwork.LocalPlayer.SetCustomProperties(leaveHash);
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (changedProps["leave"] != null) {
            if (PhotonNetwork.IsMasterClient) {
                bool end = true;
                bool win = true;
                foreach (Player player in PhotonNetwork.PlayerList) {
                    if (! (bool) player.CustomProperties["leave"]) {
                        end = false;
                        break;
                    }

                    if (! (bool) player.CustomProperties["win"]) {
                        win = false;
                    }
                }

                if (end) {
                    Hashtable endHash = new Hashtable() {{"end", true}, {"win", win}};
                    PhotonNetwork.CurrentRoom.SetCustomProperties(endHash);
                } else {
                    UpdateWaitingText();
                }
            }
        }
        
    }

    void UpdateWaitingText() {
        int total = 0;
        int length = PhotonNetwork.PlayerList.Length;
        if ((bool) PhotonNetwork.LocalPlayer.CustomProperties["leave"]) {
            foreach (Player player in PhotonNetwork.PlayerList) {
                if ((bool) player.CustomProperties["leave"]) {
                    total += 1;
                }
            }
        }

        uiController.UpdateInfoText("Waiting for others " + total.ToString() + "/" + length.ToString());

    }


}
