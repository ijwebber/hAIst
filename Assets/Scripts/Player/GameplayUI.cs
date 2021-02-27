using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameplayUI : MonoBehaviourPunCallbacks
{
    public Text scoreText;
    public Text specialUpdateText;
    public Text[] playerScores;
    public GameObject[] specialImages;

    void Start() {
        foreach (Text text in playerScores) {
            text.enabled = false;
        }
        
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player player = PhotonNetwork.PlayerList[i];

            string name = player.NickName;
            string playerText = name + ": $0";

            playerScores[i].enabled = true;
            playerScores[i].text = playerText;
        }

        foreach (GameObject image in specialImages) {
            image.SetActive(false);
        }

        int remaining = (int) PhotonNetwork.CurrentRoom.CustomProperties["specialMax"] - (int) PhotonNetwork.CurrentRoom.CustomProperties["special"];
        Debug.Log("issue " + PhotonNetwork.CurrentRoom.CustomProperties["specialMax"]);
        Debug.Log("issue2 " + PhotonNetwork.CurrentRoom.CustomProperties["special"]);
        specialUpdateText.text = "Special Items Remaining: " + remaining.ToString();

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);  

        if (changedProps["score"] != null) {
            string name = targetPlayer.NickName;
            int i = targetPlayer.ActorNumber - 1;
            string playerText = name + ": $" + changedProps["score"];
            playerScores[i].text = playerText;

            if (PhotonNetwork.LocalPlayer.IsMasterClient) {
                int total = 0;
                foreach (Player player in PhotonNetwork.PlayerList) {
                    total += (int) player.CustomProperties["score"];
                }

                Hashtable roomScore = new Hashtable();
                roomScore.Add("score", total);
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomScore);
            }    
        }      
    }

    public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        base.OnRoomPropertiesUpdate(changedProps);

        if (changedProps["score"] != null) {
            scoreText.text = "Score: $" + changedProps["score"];
        } else if (changedProps["special"] != null) {
            int i = (int) changedProps["special"] - 1;

            if (i > -1) {
                specialImages[i].SetActive(true);
            }

            int remaining = (int) PhotonNetwork.CurrentRoom.CustomProperties["specialMax"] - (int) changedProps["special"];
            if (remaining > 0) {
                specialUpdateText.text = "Special Items Remaining: " + remaining.ToString();
            } else {
                specialUpdateText.text = "The escape has opened!";

                GameObject[] objs = GameObject.FindGameObjectsWithTag("exit");
                foreach (GameObject tag in objs)
                {
                    GameObject mainObj = tag.transform.parent.gameObject;
                    mainObj.SetActive(false);
                }

            }
        } else if (changedProps["win"] != null) {
            specialUpdateText.text = "Number 1 Victory Royale";
        }
    }
}
