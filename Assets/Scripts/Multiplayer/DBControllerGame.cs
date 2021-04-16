using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.Security.Cryptography;
using Photon.Pun;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DBControllerGame : MonoBehaviour
{
    string users;
    bool login_bool;
    string login = "https://brasspig.online/check_login_1.php";
    string user_url = "https://brasspig.online/get_users.php";
    string create_url = "https://brasspig.online/put_test.php";
    string get_balance_url = "https://brasspig.online/get_balance.php?";
    string get_friends_url = "https://brasspig.online/get_friends.php?";
    string add_friend_url = "https://brasspig.online/add_friend.php?";
    string edit_balance_url = "https://brasspig.online/edit_balance.php";
    string get_threshold_url = "https://brasspig.online/get_mic_threshold.php?";
    string edit_threshold_url = "https://brasspig.online/set_mic_threshold.php?";
    string get_multiplier_url = "https://brasspig.online/get_mic_multiplier.php?";
    string edit_multiplier_url = "https://brasspig.online/set_mic_multiplier.php?";
    string get_upgrades_url = "https://brasspig.online/get_upgrades.php?";
    string add_upgrade_url = "https://brasspig.online/add_upgrade.php?";
    [SerializeField] private SoundController soundController;
    [SerializeField] private PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        // player = playerController.getPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // check if guest!!
    public void getThresholds(string username) {
        if (PlayerPrefs.GetInt("isGuest", -1) == 0) {
            // default values for guest
            soundController.threshold = PlayerPrefs.GetInt("Threshold", 2);
            soundController.multiplier = PlayerPrefs.GetInt("Multiplier", 240);
        } else {
            GetMicMultiplier(username);
            GetMicThreshold(username);
        }

    }

    public void GetUpgradeList(string username)
    {
        StartCoroutine(UpgradeList(username));
    }

    IEnumerator UpgradeList(string username)
    {
        // _GameLobby.GetComponent<PUN2_GameLobby1>().InventoryWaitPanel.SetActive(true);

        string uri = get_upgrades_url + "user=" + username;
        Debug.Log(uri);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
            }
            else
            {
                string result = webRequest.downloadHandler.text.Trim();
                if (result == "empty")
                {
                    Debug.Log("User has no upgrades");
                }
                else
                {
                    char[] delimiterChars = { ',' };
                    
                    string[] upgrade_list = result.Split(delimiterChars);
                    foreach (var upgrade in upgrade_list)
                    {
                        Debug.Log("IN GAME " + upgrade);
                        switch (upgrade.ToString())
                        {
                            case "speed_boots":
                                playerController.upgrades.speed_boots++;
                                break;
                            case "shield":
                                playerController.upgrades.shield = true;

                                break;
                            case "vision":
                                playerController.upgrades.vision++;

                                break;
                            case "self_revive":
                                playerController.upgrades.self_revive = true;

                                break;
                            case "fast_hands":
                                playerController.upgrades.fast_hands++;

                                break;
                            default:
                                break;
                        }

                        // _GameLobby.GetComponent<PUN2_GameLobby1>().PlayerInventory[upgrade] += 1;

                    }
                    // foreach (KeyValuePair<string, int> kvp in _GameLobby.GetComponent<PUN2_GameLobby1>().PlayerInventory)
                    // {
                    //     switch (kvp.Key)
                    //     {
                    //         case "speed_boots":
                    //             playerController.upgrades.speed_boots = kvp.Value;
                    //             break;
                    //         case "shield":
                    //             playerController.upgrades.speed_boots = kvp.Value;

                    //             break;
                    //         case "vision":
                    //             playerController.upgrades.speed_boots = kvp.Value;

                    //             break;
                    //         case "self_revive":
                    //             playerController.upgrades.speed_boots = kvp.Value;

                    //             break;
                    //         case "fast_hands":
                    //             playerController.upgrades.speed_boots = kvp.Value;

                    //             break;
                    //         default:
                    //             break;
                    //     }
                    //     Debug.Log("Key = " + kvp.Key + ", Value = " + kvp.Value);
                    // }

                }
            }
        }
        playerController.applyUpdates();
        // _GameLobby.GetComponent<PUN2_GameLobby1>().InventoryWaitPanel.SetActive(false);




    }

    public void EditCoinBalance(string username, int new_balance, int type)
    {
        StartCoroutine(EditBalance(username,new_balance,type));
    }
    public void GetMicThreshold(string username)
    {
        StartCoroutine(GetThreshold(username));
    }

    public void GetMicMultiplier(string username)
    {
        StartCoroutine(GetMultiplier(username));
    }
    
    IEnumerator GetMultiplier(string username)
    {
        string uri = get_multiplier_url + "user=" + username;
        string outMulti = "240";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
            }
            else
            {
                outMulti = webRequest.downloadHandler.text;
            }
            soundController.multiplier = Int32.Parse(outMulti);
        }
    }
    
    IEnumerator GetThreshold(string username)
    {
        string uri = get_threshold_url + "user=" + username;
        string threshOut = "0";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
            }
            else
            {
                threshOut = webRequest.downloadHandler.text;
            }
        }
        soundController.threshold = Int32.Parse(threshOut);
    }
    IEnumerator EditBalance(string username, int new_balance,int type)
    {
        string uri = edit_balance_url;
        string post_data = "{ \"username\": \"" + username + "\", \"new_balance\": " + new_balance + "  }";

        using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(post_data);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                //Debug.Log(webRequest.error);
            }
            else
            {
                if (webRequest.downloadHandler.text == "true")
                {
                    Debug.Log("Balance edited succesfully.");

                }
                else
                {
                    Debug.Log("Balance edit unsuccesful.");
                }


            }
        }
    }
}
