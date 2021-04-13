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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // check if guest!!
    public void getThresholds(string username) {
        GetMicMultiplier(username);
        GetMicThreshold(username);
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
}
