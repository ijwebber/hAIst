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
                Hashtable hash = new Hashtable() {{"leave", true}};
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }
    }

    void OnCollisionExit(Collision other) {
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            if (other.gameObject.tag == "grass") {
                Hashtable leaveHash = new Hashtable() {{"leave", false}};
                PhotonNetwork.LocalPlayer.SetCustomProperties(leaveHash);
            }
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable changedProps) {
        base.OnRoomPropertiesUpdate(changedProps);  
        if (changedProps["end"] != null) {
            if ((bool) changedProps["end"]){
                Debug.Log("winner"); // TODO remove
            } 
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (changedProps["leave"] != null) {
            if (PhotonNetwork.IsMasterClient) {
                bool win = true;
                foreach (Player player in PhotonNetwork.PlayerList) {
                    if (! (bool) player.CustomProperties["leave"]) {
                        win = false;
                    }
                }

                if (win) {
                    Hashtable endHash = new Hashtable() {{"end", true}, {"win", true}};
                    PhotonNetwork.CurrentRoom.SetCustomProperties(endHash);
                }
            }
        }
        
    }


}
