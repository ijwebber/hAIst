using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerLeave : MonoBehaviourPunCallbacks
{

    GameObject messageBox;

    void Awake() {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MessageBox"))
        {
            if (obj.name == "InfoMessage") {
                messageBox = obj;
            }
        } 
    }

    void OnCollisionEnter(Collision other) {
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            if (other.gameObject.tag == "Van") {
                messageBox.GetComponent<Text>().text = "Ready to leave? Press E";
                messageBox.SetActive(true);
            }
        }
    }

    void OnCollisionStay(Collision other) {
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            if (other.gameObject.tag == "Van") {
                if (Input.GetKeyDown(KeyCode.E)) {
                    Hashtable hash = new Hashtable() {{"leave", true}, {"win", true}};
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

                    messageBox.GetComponent<Text>().text = "Ready 1/4"; // TODO replace this with a dynamic ready system
                }
            }
        }
    }

    void OnCollisionExit(Collision other) {
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            if (other.gameObject.tag == "Van") {
                messageBox.SetActive(false);

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
                }
            }
        }
        
    }


}
