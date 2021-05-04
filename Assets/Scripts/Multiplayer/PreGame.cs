using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Photon.Pun;
using Photon.Realtime;
//using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class PreGame : MonoBehaviourPunCallbacks
{
    public GameObject _GameLobby;


    public Button StartGameButton;
    public Button SetReadyButton;
    public ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
    public ExitGames.Client.Photon.Hashtable customPropertiesRoom = new ExitGames.Client.Photon.Hashtable();


    public GameObject ThiefSkins;
    public GameObject thief_1_home;
    public GameObject thief_1;
    public GameObject thief_2;
    public GameObject thief_3;
    public GameObject thief_4;

    public GameObject StartGameWaitPanel;
    public GameObject ChooseUpgradesPanel;
    public GameObject LockerPanel;

    public Dictionary<string, bool> EnabledUpgrades = new Dictionary<string, bool>();

    public GameObject SkinPanelContent;

    public SelectSkinGroup skinGroup;
    public GameObject SkinIconPrefab;

    //SPRITES
    public Sprite classic;
    public Sprite red;
    public Sprite radioactive;
    public Sprite white;
    public Sprite tuxedo;
    public Sprite pumpkin;

    private int noReady = 0;


    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {

            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        }
        PlayerPrefs.SetInt("speed_boots", 0);
        PlayerPrefs.SetInt("vision", 0);
        PlayerPrefs.SetInt("shield", 0);
        PlayerPrefs.SetInt("self_revive", 0);
        PlayerPrefs.SetInt("fast_hands", 0);
        PlayerPrefs.SetInt("ninja", 0);


        //Debug.Log("USERID: "+PhotonNetwork.LocalPlayer.UserId);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //just spawns in player object
    private void Awake()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.CurrentRoom == null)
            {
                Debug.Log("Is not in the room, returning back to Lobby");
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby 1");
                return;
            }

            if (!customProperties.ContainsKey("ready"))
            {
                customProperties.Add("ready", "false");
            }
            if (!customProperties.ContainsKey("skin"))
            {
                customProperties.Add("skin", thief_1.GetComponent<Image>().sprite.name);
            }
            customPropertiesRoom = PhotonNetwork.CurrentRoom.CustomProperties;
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        }
    }

    public void ControlSkins()
    {
        Debug.Log("SKIN CONTROLLER");
        /*SkinPanelContent.gameObject.transform.GetChild(1).GetComponent<SelectSkinButton>().tabGroup = skinGroup;
        SkinPanelContent.gameObject.transform.GetChild(2).GetComponent<SelectSkinButton>().tabGroup = skinGroup;
        SkinPanelContent.gameObject.transform.GetChild(3).GetComponent<SelectSkinButton>().tabGroup = skinGroup;*/

        foreach (KeyValuePair<string, bool> kvp in _GameLobby.GetComponent<PUN2_GameLobby1>().PlayerSkins)
        {
            if (kvp.Key == "red" & kvp.Value)
            {
                GameObject obj = (GameObject)Instantiate(SkinIconPrefab, SkinPanelContent.transform);
                obj.GetComponent<SelectSkinButton>().tabGroup = skinGroup;
                obj.transform.GetChild(0).GetComponent<Image>().sprite = red;
            }
            else if (kvp.Key == "white" & kvp.Value)
            {
                GameObject obj = (GameObject)Instantiate(SkinIconPrefab, SkinPanelContent.transform);
                obj.GetComponent<SelectSkinButton>().tabGroup = skinGroup;
                obj.transform.GetChild(0).GetComponent<Image>().sprite = white;
            }
            else if (kvp.Key == "radioactive" & kvp.Value)
            {
                GameObject obj = (GameObject)Instantiate(SkinIconPrefab, SkinPanelContent.transform);
                obj.GetComponent<SelectSkinButton>().tabGroup = skinGroup;
                obj.transform.GetChild(0).GetComponent<Image>().sprite = radioactive;
            }

        }

    }

    public void EquipSkin()
    {
        string skin_name = ThiefSkins.GetComponent<Image>().sprite.name;
        thief_1.GetComponent<Image>().sprite = ThiefSkins.GetComponent<Image>().sprite;
    }

    public void SetUpgradesForGame()
    {

        List<string> remove_upgrades = new List<string>();
        // PERMA UPGRADES
        foreach (KeyValuePair<string, int> kvp in _GameLobby.GetComponent<PUN2_GameLobby1>().PlayerInventory)
        {
            if (kvp.Key.Equals("speed_boots") & kvp.Value > 0)
            {
                EnabledUpgrades[kvp.Key] = true;
                PlayerPrefs.SetInt(kvp.Key, kvp.Value);
            }
            if (kvp.Key.Equals("vision") & kvp.Value > 0)
            {
                EnabledUpgrades[kvp.Key] = true;
                PlayerPrefs.SetInt(kvp.Key, kvp.Value);

            }
            if (kvp.Key.Equals("fast_hands") & kvp.Value > 0)
            {
                EnabledUpgrades[kvp.Key] = true;
                PlayerPrefs.SetInt(kvp.Key, kvp.Value);

            }
            if (kvp.Key.Equals("ninja") & kvp.Value > 0)
            {
                EnabledUpgrades[kvp.Key] = true;
                PlayerPrefs.SetInt(kvp.Key, kvp.Value);

            }
        }

        // POWER UPS
        foreach (KeyValuePair<string, int> kvp in _GameLobby.GetComponent<PUN2_GameLobby1>().PlayerInventory)
        {
            if (kvp.Key.Equals("shield") & kvp.Value > 0)
            {
                if (ChooseUpgradesPanel.GetComponent<UpgradeController>().shield_toggle != null)
                {
                    if (ChooseUpgradesPanel.GetComponent<UpgradeController>().shield_toggle.isOn)
                    {
                        EnabledUpgrades[kvp.Key] = true;
                        PlayerPrefs.SetInt(kvp.Key, 1);
                        remove_upgrades.Add(kvp.Key);


                    }
                }
            }
            if (kvp.Key.Equals("self_revive") & kvp.Value > 0)
            {
                if (ChooseUpgradesPanel.GetComponent<UpgradeController>().self_revive_toggle != null)
                {
                    if (ChooseUpgradesPanel.GetComponent<UpgradeController>().self_revive_toggle.isOn)
                    {
                        EnabledUpgrades[kvp.Key] = true;
                        PlayerPrefs.SetInt(kvp.Key, 1);
                        remove_upgrades.Add(kvp.Key);


                    }
                }
            }
        }
        _GameLobby.GetComponent<PUN2_GameLobby1>().RemoveUpgrade(remove_upgrades);
        foreach (KeyValuePair<string, bool> kvp in EnabledUpgrades)
        {
            Debug.Log("Key = " + kvp.Key + ", Value = " + kvp.Value);
        }
        PlayerPrefs.Save();

    }


    public void SetReady() 
    {
        Debug.Log("Set Ready Function");
        if (customProperties["ready"].Equals("false"))
        {
            customProperties["ready"] = "true";
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
            Debug.Log("Ready state has beed updated to :" + (string)customProperties["ready"]);
            SetReadyButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            customProperties["ready"] = "false";
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
            Debug.Log("Ready state has beed updated to :" + (string)customProperties["ready"]);
            SetReadyButton.GetComponent<Image>().color = Color.red;
        }
        
        
    }

    public void SetPlayerSkins()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].NickName.Equals(thief_2.GetComponentInChildren<Text>().text))
            {
                if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("classic"))
                {
                    thief_2.GetComponent<Image>().sprite = classic;
                }
                else if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("red"))
                {
                    thief_2.GetComponent<Image>().sprite = red;
                }
                else if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("white"))
                {
                    thief_2.GetComponent<Image>().sprite = white;
                }
                else if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("radioactive"))
                {
                    thief_2.GetComponent<Image>().sprite = radioactive;
                }
                else if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("tuxedo"))
                {
                    thief_2.GetComponent<Image>().sprite = tuxedo;
                }
                else if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("pumpkin"))
                {
                    thief_2.GetComponent<Image>().sprite = pumpkin;
                }
            }
            else if (PhotonNetwork.PlayerList[i].NickName.Equals(thief_3.GetComponentInChildren<Text>().text))
            {
                if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("classic"))
                {
                    thief_3.GetComponent<Image>().sprite = classic;
                }
                else if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("red"))
                {
                    thief_3.GetComponent<Image>().sprite = red;
                }
                else if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("white"))
                {
                    thief_3.GetComponent<Image>().sprite = white;
                }
                else if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("radioactive"))
                {
                    thief_3.GetComponent<Image>().sprite = radioactive;
                }
                else if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("tuxedo"))
                {
                    thief_3.GetComponent<Image>().sprite = tuxedo;
                }
                else if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("pumpkin"))
                {
                    thief_3.GetComponent<Image>().sprite = pumpkin;
                }
            }
            else if (PhotonNetwork.PlayerList[i].NickName.Equals(thief_4.GetComponentInChildren<Text>().text))
            {
                if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("classic"))
                {
                    thief_4.GetComponent<Image>().sprite = classic;
                }
                else if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("red"))
                {
                    thief_4.GetComponent<Image>().sprite = red;
                }
                else if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("white"))
                {
                    thief_4.GetComponent<Image>().sprite = white;
                }
                else if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("radioactive"))
                {
                    thief_4.GetComponent<Image>().sprite = radioactive;
                }
                else if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("tuxedo"))
                {
                    thief_4.GetComponent<Image>().sprite = tuxedo;
                }
                else if (PhotonNetwork.PlayerList[i].CustomProperties["skin"].Equals("pumpkin"))
                {
                    thief_4.GetComponent<Image>().sprite = pumpkin;
                }
            }
        }
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log(targetPlayer.NickName + " changed a value");
        int num_ready = GetNumReady();
        customPropertiesRoom["num_ready"] = num_ready;
        PhotonNetwork.CurrentRoom.SetCustomProperties(customPropertiesRoom);
        SetReadyChecks();
        SetPlayerSkins();
        CheckAllReady();

    }

    public void EnableChooseUpgradesPanel()
    {
        ChooseUpgradesPanel.SetActive(true);
        ChooseUpgradesPanel.GetComponent<UpgradeController>().PopulateUpdatesPanel();
    }

    public void EnableLockerPanel()
    {
        _GameLobby.GetComponent<PUN2_GameLobby1>().ControlSkins();
        LockerPanel.SetActive(true);
    }

    public void DisableLockerPanel()
    {
        //EquipSkin();
        _GameLobby.GetComponent<PUN2_GameLobby1>().EquipSkinPre();
        LockerPanel.SetActive(false);
    }

    public void DisableChooseUpgradesPanel()
    {
        ChooseUpgradesPanel.SetActive(false);
    }

    public void QuitButton()
    {
        PhotonNetwork.LeaveRoom();   
    }
    public void StartGame()
    {
        StartGameWaitPanel.SetActive(true);
        this.GetComponent<PhotonView>().RPC("SetupGame", RpcTarget.All);
    }

    [PunRPC]
    void SetupGame() {
        StartGameWaitPanel.SetActive(true);
        PUN2_GameLobby1 gameLobby1 = GameObject.FindObjectOfType<PUN2_GameLobby1>();
        // this.GetComponent<
        int guest = 0;
        if (gameLobby1.IsGuest) {
            guest = 1;
        }
        PlayerPrefs.SetInt("PlayerBalance", gameLobby1.PlayerBalance);
        // PlayerPrefs.SetInt("PlayerBalance", );
        PlayerPrefs.SetInt("isGuest", guest);
        string equiped_skin = ThiefSkins.GetComponent<Image>().sprite.name;
        PlayerPrefs.SetString("skin", equiped_skin);


        PlayerPrefs.Save();
        if (guest == 0)
        { 
            SetUpgradesForGame();
        }

        this.GetComponent<PhotonView>().RPC("ReadyToStart", RpcTarget.MasterClient);
    }

    [PunRPC]
    void ReadyToStart(){
        noReady++;
        if (noReady == PhotonNetwork.PlayerList.Length){
            PhotonNetwork.LoadLevel("ArtLevel");
        }
    }
    
    public int GetNumReady()
    {
        Debug.Log("GET NUM READY");
        int total_ready = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) 
        {
            if (PhotonNetwork.PlayerList[i].CustomProperties["ready"].Equals("true")) 
            {
                total_ready ++;
            }
       }
        return total_ready;
    }


    public void SetReadyChecks()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i].NickName.Equals(thief_1.GetComponentInChildren<Text>().text))
            {
              if (PhotonNetwork.PlayerList[i].CustomProperties["ready"].Equals("true"))
              {
                  Debug.Log("READY CHECK");
                  thief_1.transform.GetChild(1).gameObject.SetActive(true);
              }
              else
              {
                  thief_1.transform.GetChild(1).gameObject.SetActive(false);
              }
            }
            else if (PhotonNetwork.PlayerList[i].NickName.Equals(thief_2.GetComponentInChildren<Text>().text))
            {
              if (PhotonNetwork.PlayerList[i].CustomProperties["ready"].Equals("true"))
              {
                  Debug.Log("READY CHECK");
                  thief_2.transform.GetChild(1).gameObject.SetActive(true);
              }
              else
              {
                  thief_2.transform.GetChild(1).gameObject.SetActive(false);
              }
            }
            else if (PhotonNetwork.PlayerList[i].NickName.Equals(thief_3.GetComponentInChildren<Text>().text))
            {
              if (PhotonNetwork.PlayerList[i].CustomProperties["ready"].Equals("true"))
              {
                  Debug.Log("READY CHECK");
                  thief_3.transform.GetChild(1).gameObject.SetActive(true);
              }
              else
              {
                  thief_3.transform.GetChild(1).gameObject.SetActive(false);
              }
            }
            else if (PhotonNetwork.PlayerList[i].NickName.Equals(thief_4.GetComponentInChildren<Text>().text))
            {
              if (PhotonNetwork.PlayerList[i].CustomProperties["ready"].Equals("true"))
              {
                  Debug.Log("READY CHECK");
                  thief_4.transform.GetChild(1).gameObject.SetActive(true);
              }
              else
              {
                  thief_4.transform.GetChild(1).gameObject.SetActive(false);
              }
            }
        }
    }

    public void ShowMap()
    {

    }

    public bool CheckAllReady() 
    {
        int total_ready = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) 
        {
            if (PhotonNetwork.PlayerList[i].CustomProperties["ready"].Equals("false")) 
            {
                StartGameButton.interactable = false;
                StartGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Waiting for players\nto ready up...";
                return false;
            }
            total_ready ++;

        }
        if (total_ready > 0) {
            //PhotonNetwork.LoadLevel("ArtLevel");
            if (PhotonNetwork.IsMasterClient)
            {
                StartGameButton.interactable = true;
                StartGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "START";
            } else {
                StartGameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Waiting for leader\nto start...";
            }
            return true; 
        }
        StartGameButton.interactable = false;
        return false;
    }


    //Go back to main meny when you leave game
    /*public override void OnLeftRoom()
    {
        //We have left the Room, return back to the GameLobby
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameLobby 1");
    }
    */
}
