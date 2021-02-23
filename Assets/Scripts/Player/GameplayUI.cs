using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameplayUI : MonoBehaviourPunCallbacks
{
    public Text scoreText;
    public Text[] playerScores;

    void Awake() {
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
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);  

        string name = targetPlayer.NickName;
        int i = targetPlayer.ActorNumber - 1;
        string playerText = name + ": $" + changedProps["score"];
        playerScores[i].text = playerText;          
    }

    public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        base.OnRoomPropertiesUpdate(changedProps);
        scoreText.text = "Score: $" + changedProps["score"];
    }
}
