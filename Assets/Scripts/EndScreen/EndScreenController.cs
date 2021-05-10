using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class EndScreenController : MonoBehaviourPunCallbacks
{

    // private DBControllerEnd dbController;
    public GameObject[] playerRows;
    public GameObject[] playerSegs;
    [SerializeField] private Color[] colors;
    [SerializeField] private GameObject deadWeight;
    [SerializeField] private GameObject GA;
    [SerializeField] private GameObject moneyBags;
    [SerializeField] private GameObject loudMouth;
    public TextMeshProUGUI totalText;
    public GameObject winScreen;
    public GameObject lossScreen;

    public Button PlayAgainWin;
    public Button PlayAgainLose;
    [SerializeField] DBControllerEnd dbController;

    private void Awake()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {

            UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby 1");
            return;
        }

        // dbController = GameObject.FindObjectOfType<DBControllerEnd>();
        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() {{"end", false}, {"special", 0}});
            PlayAgainWin.interactable = true;
            PlayAgainLose.interactable = true;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {{"ready", false}});

        bool wasWin = (bool) PhotonNetwork.CurrentRoom.CustomProperties["win"];

        if (wasWin) {
            winScreen.SetActive(true);
            lossScreen.SetActive(false);

            for (int i = 0; i < playerRows.Length; i++) {
                GameObject row = playerRows[i];
                GameObject seg = playerSegs[i];
                row.SetActive(false);
                seg.SetActive(false);
            }

            int noPlayers = PhotonNetwork.PlayerList.Length;
            List<int> moneyPlayers = new List<int>();
            int moneyValue = 0;
            List<int> GAPlayers = new List<int>();
            int GAValue = 0;
            List<int> loudPlayers = new List<int>();
            int loudValue = 0;
            List<int> deadPlayers = new List<int>();
            int deadValue = 0;

            //populate rows and segments
            for (int i = 0; i < noPlayers; i++)
            {
                GameObject row = playerRows[i];
                GameObject seg = playerSegs[i];
                row.SetActive(true);
                seg.SetActive(true);
                seg.GetComponent<Image>().fillAmount = (1f/(float)noPlayers);
                seg.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0,(360/noPlayers)*i);

                Player player = PhotonNetwork.PlayerList[i];
                row.transform.Find("Player").gameObject.GetComponent<Text>().text = player.NickName;
                row.transform.Find("Cut").gameObject.GetComponent<Text>().text = ((int)100/noPlayers).ToString() + "%";
                row.transform.Find("Earnings").gameObject.GetComponent<Text>().text = "$" + (Mathf.Floor((int) (PhotonNetwork.CurrentRoom.CustomProperties["score"])/noPlayers)).ToString();
                if ((int)player.CustomProperties["score"] >= moneyValue)  {
                    moneyPlayers.Add(i);
                    moneyValue = (int)player.CustomProperties["score"];
                    moneyBags.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = "Got the biggest haul for the gang ($" + ((int)player.CustomProperties["score"]).ToString() + ")";
                } else {
                    Debug.Log("Null score");
                }
                if (player.CustomProperties["downs"] != null) {
                    if ((int)player.CustomProperties["downs"] >= deadValue)  {
                        deadPlayers.Add(i);
                        deadValue =  (int)player.CustomProperties["downs"];
                        Debug.Log("Player " + i + " got downed " + (int)player.CustomProperties["downs"]);
                        deadWeight.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = "Letting down the side with Most downs (" + ((int)player.CustomProperties["downs"]).ToString() + ")";
                    }
                } else {
                    Debug.Log("Null downs");
                }
                if (player.CustomProperties["revives"] != null)  {
                    if ((int)player.CustomProperties["revives"] >= GAValue)  {
                        GAPlayers.Add(i);
                        GAValue = (int)player.CustomProperties["revives"];
                        GA.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = "Had everyone's backs with most saves (" + ((int)player.CustomProperties["revives"]).ToString() + ")";
                    }
                }
                if (player.CustomProperties["alerts"] != null)  {
                    if ((int)player.CustomProperties["alerts"] >= loudValue)  {
                        loudPlayers.Add(i);
                        loudValue = (int)player.CustomProperties["alerts"];
                        loudMouth.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = "Couldn't shut up and alerted the most guards (" + ((int)player.CustomProperties["alerts"]).ToString() + ")" ;
                    }
                }
            }
            Debug.Log("END money " + PhotonNetwork.PlayerList[moneyPlayers[0]].NickName + " // " + moneyValue);
            Debug.Log("END loud " + loudPlayers[0] + " // " + loudValue);

            int start = 0;
            int finalEarnings = 0;
            float startingCut = (1f/(float)noPlayers);
            for (int i = 0; i < noPlayers; i++)
            {
                GameObject row = playerRows[i];
                GameObject seg = playerSegs[i];
                float cut = startingCut;
                seg.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,0,(360/noPlayers)*i + start);
                if (moneyPlayers.Contains(i)) {
                    seg.GetComponent<Image>().fillAmount += .1f;
                    cut += .1f;
                    startingCut -= .1f/noPlayers;
                    moneyBags.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[i].NickName;
                    moneyBags.transform.Find("Player").GetComponent<TextMeshProUGUI>().color = colors[i];
                    start -= 36;
                }
                if (loudPlayers.Contains(i)) {
                    seg.GetComponent<Image>().fillAmount -= .05f;
                    cut -= .05f;
                    startingCut += .05f/noPlayers;
                    loudMouth.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[i].NickName;
                    loudMouth.transform.Find("Player").GetComponent<TextMeshProUGUI>().color = colors[i];
                    start += 18;
                }
                if (GAPlayers.Contains(i)) {
                    seg.GetComponent<Image>().fillAmount += .05f;
                    cut += .05f;
                    startingCut -= .05f/noPlayers;
                    GA.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[i].NickName;
                    GA.transform.Find("Player").GetComponent<TextMeshProUGUI>().color = colors[i];
                    start -= 18;
                }
                if (deadPlayers.Contains(i)) {
                    seg.GetComponent<Image>().fillAmount -= .1f;
                    startingCut += .1f/noPlayers;
                    cut -= .1f;
                    deadWeight.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = PhotonNetwork.PlayerList[i].NickName;
                    deadWeight.transform.Find("Player").GetComponent<TextMeshProUGUI>().color = colors[i];
                    start += 18;
                }
                row.transform.Find("Cut").gameObject.GetComponent<Text>().text = ((int)(cut*100)).ToString() + "%";
                if (PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[i]) {
                    finalEarnings = (int)Mathf.Floor((int) (PhotonNetwork.CurrentRoom.CustomProperties["score"]) * cut);
                }
                row.transform.Find("Earnings").gameObject.GetComponent<Text>().text = "$" + (Mathf.Floor((int) (PhotonNetwork.CurrentRoom.CustomProperties["score"]) * cut)).ToString();
            }
            if (moneyPlayers.Count == 0) {
                moneyBags.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "No-one";
                moneyBags.transform.Find("Player").GetComponent<TextMeshProUGUI>().color = Color.gray;
            }
            if (loudPlayers.Count == 0) {
                loudMouth.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "No-one";
                loudMouth.transform.Find("Player").GetComponent<TextMeshProUGUI>().color = Color.gray;
            }
            if (GAPlayers.Count == 0) {
                GA.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "No-one";
                GA.transform.Find("Player").GetComponent<TextMeshProUGUI>().color = Color.gray;
            }
            if (deadPlayers.Count == 0) {
                deadWeight.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "No-one";
                deadWeight.transform.Find("Player").GetComponent<TextMeshProUGUI>().color = Color.gray;
            }
            
            //update total score
            totalText.text = "$" + ((int)PhotonNetwork.CurrentRoom.CustomProperties["score"]).ToString();

            //update database
            if (PlayerPrefs.GetInt("isGuest", -1) == 0) {
                Debug.Log("New balance = " + PlayerPrefs.GetInt("PlayerBalance", 0) + (finalEarnings));
                dbController.EditCoinBalance(PhotonNetwork.NickName, (PlayerPrefs.GetInt("PlayerBalance", 0) + finalEarnings),0);
            }
        } else {
            winScreen.SetActive(false);
            lossScreen.SetActive(true);
        }

    }

    public void QuitButton()
    {
        PhotonNetwork.LeaveRoom();
    }

    //Go back to main meny when you leave game
    public override void OnLeftRoom()
    {
        Debug.Log("On left Room callback");

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.LoadLevel("GameLobby 1");
        Debug.Log("gamelobby 1 loaded");
    }

    public void PlayAgainButton() {
        PhotonNetwork.LoadLevel("GameLobby 1");

    }

}
