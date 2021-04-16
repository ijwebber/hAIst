using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.Security.Cryptography;
using Photon.Pun;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DBControllerEnd : MonoBehaviour
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

    public void EditCoinBalance(string username, int new_balance, int type)
    {
        StartCoroutine(EditBalance(username,new_balance,type));
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
