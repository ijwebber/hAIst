using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerLeave : MonoBehaviourPunCallbacks
{

    UIController uiController;
    PlayerMovement playerMovement;

    void Start() {
        uiController = GameObject.FindObjectOfType<UIController>();
        playerMovement = GameObject.FindObjectOfType<PlayerMovement>();
    }

    void OnTriggerEnter(Collider other) {
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            if (other.gameObject.CompareTag("ExitBox")) {
                if ((int)PhotonNetwork.CurrentRoom.CustomProperties["roomSpecial"] == 3) {
                    uiController.UpdateInfoText("Ready to leave? Press E");
                } else  {
                    uiController.UpdateInfoText("You haven't got all of the artifacts!");
                }
            }
        }
    }

    void OnTriggerStay(Collider other) {
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            if (other.gameObject.CompareTag("ExitBox")) {
                if (Input.GetKeyDown(KeyCode.E) && (int)PhotonNetwork.CurrentRoom.CustomProperties["roomSpecial"] == 3) {
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
                bool end = true; // Set end & win to true
                bool win = true;
                foreach (Player player in PhotonNetwork.PlayerList) { // Check if all players want to win
                    if (player.CustomProperties != null) {
                        if (player.CustomProperties["leave"] != null && !(bool) player.CustomProperties["leave"]) {
                            end = false;
                            break;
                        }

                        if (!(bool) player.CustomProperties["win"]) { // If all players don't have a win prop then win is false
                            win = false;
                        }
                    }
                }

                if (end) {
                    Hashtable endHash = new Hashtable() {{"end", true}, {"win", win}};
                    PhotonNetwork.CurrentRoom.SetCustomProperties(endHash);
                } else {
                    photonView.RPC("UpdateWaitingText", RpcTarget.All);
                }
            }
        } else if (changedProps["disabled"] != null) {
            if ((bool) changedProps["disabled"]) {
                StartCoroutine(CheckIfPlayersAreDown());
            } else {
                bool cond = true;
                foreach (Player player in PhotonNetwork.PlayerList) {
                    if ((bool) player.CustomProperties["disabled"]) {
                        cond = false;
                    }
                }

                if (cond) {
                    StopCoroutine(CheckIfPlayersAreDown());
                }
            }

            if (PhotonNetwork.IsMasterClient) {
                if ((bool) changedProps["disabled"]) {
                    bool end = true;
                    foreach (Player player in PhotonNetwork.PlayerList) {
                        if (!(bool) player.CustomProperties["disabled"]) {
                            end = false;
                        }
                    }

                    if (end) {
                        Hashtable endHash = new Hashtable() {{"end", true}, {"win", false}};
                        PhotonNetwork.CurrentRoom.SetCustomProperties(endHash);
                    }
                }
            }
        }
    }

    [PunRPC]
    void UpdateWaitingText() {
        int total = 0;
        int length = PhotonNetwork.PlayerList.Length;
        if ((bool) PhotonNetwork.LocalPlayer.CustomProperties["leave"]) {
            foreach (Player player in PhotonNetwork.PlayerList) {
                if ((bool) player.CustomProperties["leave"]) {
                    total += 1;
                }
            }

            uiController.UpdateInfoText("Waiting for others " + total.ToString() + "/" + length.ToString());
        }
    }

    IEnumerator CheckIfPlayersAreDown() {
		while (true) {
			yield return new WaitForSeconds (.2f);
            bool end = true;
            foreach (Player player in PhotonNetwork.PlayerList) {
                if (!(bool) player.CustomProperties["disabled"]) {
                    end = false;
                    break;
                }
		    }

            if (end) {
                Hashtable endHash = new Hashtable() {{"end", true}, {"win", false}};
                PhotonNetwork.CurrentRoom.SetCustomProperties(endHash);
            }
	    }
    }
}
