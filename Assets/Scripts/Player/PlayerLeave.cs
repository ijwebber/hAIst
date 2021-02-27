using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerLeave : MonoBehaviourPunCallbacks
{
    void OnCollisionStay(Collision other) {
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            if (other.gameObject.tag == "grass") {
                Hashtable hash = new Hashtable() {{"leave", true}, {"win", true}};
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }
    }

    void OnCollisionExit(Collision other) {
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            if (other.gameObject.tag == "grass") {
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
