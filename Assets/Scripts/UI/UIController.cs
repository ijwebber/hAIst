using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UIController : MonoBehaviourPunCallbacks
{
    public GameObject infoMessage;
    public GameObject locationText;
    public Text scoreText;
    public Text[] playerScores;
    public Image[] stars;
    public Sprite star1, star2, star3;

    void Start()
    {
        // Disable all player tags on ui
        for (int i = 0; i < playerScores.Length; i++) {
            Text text = playerScores[i];
            Image artifacts = stars[i];
            artifacts.sprite = null;
            text.enabled = false;
        }
        
        // For each player enable the player tag and set to their name
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player player = PhotonNetwork.PlayerList[i];

            string name = player.NickName;
            string playerText = name + ": $0";

            playerScores[i].enabled = true;
            playerScores[i].text = playerText;
        }

        // Make sure the information message is not active
        infoMessage.SetActive(false);
    }

    // Change what the info box text shows, also shows the box
    public void UpdateInfoText(string text) {
        infoMessage.GetComponent<Text>().text = text;
        ShowInfoBox();
    }

    public void HideInfoBox() {
        infoMessage.SetActive(false);
    }

    public void ShowInfoBox() {
        infoMessage.SetActive(true);
    }
    
    // Update the location text with any text 
    public void UpdateLocationText(string text) {
        locationText.GetComponent<Text>().text = text;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);  
        if (changedProps["specialStolen"] != null) {
            int i = 0;
            Sprite newStars = null;
            Color newColor = Color.clear;
            for (int i1 = 0; i1 < PhotonNetwork.PlayerList.Length; i1++) {
                Player p = PhotonNetwork.PlayerList[i1];
                if (p == targetPlayer) {
                    i = i1;
                    switch (p.CustomProperties["specialStolen"]) {
                        case 0:
                            newStars = null;
                            break;
                        case 1:
                            newStars = star1;
                            newColor = Color.white;
                            break;
                        case 2:
                            newStars = star2;
                            newColor = Color.white;
                            break;
                        case 3:
                            newStars = star3;
                            newColor = Color.white;
                            break;
                        default:
                            newStars = star3;
                            newColor = Color.white;
                            break;
                    }
                }
            }
            stars[i].sprite = newStars;
            stars[i].color = newColor;
        }        


        // If the score has changed for a player update that on all, then master adds to the total score.
        if (changedProps["score"] != null) {
            string name = targetPlayer.NickName;
            int i = 0;
            for (int i1 = 0; i1 < PhotonNetwork.PlayerList.Length; i1++)
            {
                Player p = PhotonNetwork.PlayerList[i1];
                if (p == targetPlayer) {
                    i =  i1;
                }
            }
            string playerText = name + ": $" + changedProps["score"];
            playerScores[i].text = playerText;

            if (PhotonNetwork.LocalPlayer.IsMasterClient) {
                int total = 0;
                foreach (Player player in PhotonNetwork.PlayerList) {
                    if (player.CustomProperties["score"] != null) {
                        total += (int) player.CustomProperties["score"];
                    }
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
        }
    }
}
