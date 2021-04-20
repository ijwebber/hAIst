using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.Security.Cryptography;
using Photon.Pun;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DB_Controller : MonoBehaviour
{
    public GameObject _GameLobby;
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
    string edit_upgrade_url = "https://brasspig.online/edit_upgrade_list.php?";
    string get_skins_url = "https://brasspig.online/get_skin_list.php?";
    string add_skin_url = "https://brasspig.online/add_skin.php?";




    [SerializeField] Slider multiplier, threshold;

    private void Start()
    {
    }



    public void CheckUsername(string username)
    {
        StartCoroutine(GetUsers(username));
    }

    public void Login(string username, string password)
    {
        StartCoroutine(CheckLogin(username, password));
    }

    public void Create(string username, string password)
    {
        StartCoroutine(CreateUser(username, password));
    }

    public void GetCoinBalance(string username)
    {
        StartCoroutine(CoinBalance(username));
    }

    public void EditCoinBalance(string username, int new_balance, int type)
    {
        StartCoroutine(EditBalance(username,new_balance,type));
    }

    public void EditMicThreshold(string username, int value)
    {
        StartCoroutine(EditThreshold(username, value));
    }

    public void GetMicThreshold(string username, int state)
    {
        StartCoroutine(GetThreshold(username, state));
    }

    public void EditMicMultiplier(string username, int value)
    {
        StartCoroutine(EditMultiplier(username, value));
    }

    public void saveMicSettings() {
        if (_GameLobby.GetComponent<PUN2_GameLobby1>().IsGuest) {
            PlayerPrefs.SetInt("Threshold", (int)threshold.value);
            PlayerPrefs.SetInt("Multiplier", (int)multiplier.value);
        } else {
            EditMicMultiplier(PhotonNetwork.NickName, (int)multiplier.value);
            EditMicThreshold(PhotonNetwork.NickName, (int)threshold.value);
        }

    }

    public void getThresholds(string username) {
        GetMicMultiplier(username, 0);
        GetMicThreshold(username, 0);
    }

    public void GetMicMultiplier(string username, int state)
    {
        StartCoroutine(GetMultiplier(username, state));
    }

    public void GetUpgradeList(string username)
    {
        StartCoroutine(UpgradeList(username));
    }
    public void GetSkinList(string username)
    {
        StartCoroutine(SkinList(username));
    }

    public void AddUpgrade(string username, string upgrade)
    {
        StartCoroutine(Add_Upgrade(username, upgrade));
    }

    public void AddSkin(string username, string skin)
    {
        StartCoroutine(Add_Skin(username, skin));
    }

    public void RemoveUpgrade(string username, List<string> upgrades)
    {
        StartCoroutine(Remove_Upgrade(username, upgrades));
    }

    public void RemoveUpgrade2(string username, string upgrade, string upgrade2)
    {
        StartCoroutine(Remove_Upgrade2(username, upgrade, upgrade2));
    }

    public void EditUpgradeList(string username, string new_upgrade_list)
    {
        StartCoroutine(Edit_Upgrade_List(username, new_upgrade_list));
    }


    public void GetFriends(string username, int type)
    {
        StartCoroutine(Friends(username,type));
    }

    public void CheckIfExists(string username, string friend)
    {
        StartCoroutine(CheckIfUserExists(username, friend));
    }

    public void AddFriend(string username, string friend)
    {
        StartCoroutine(Add(username, friend));
    }

    IEnumerator CheckLogin(string username, string password)
    {
        string uri = login;
        string post_data = "{ \"username\": \"" + username + "\"}";

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
                if (webRequest.downloadHandler.text == "false")
                {
                    Debug.Log(webRequest.downloadHandler.text);
                    Debug.Log("username doesnt exist");
                    _GameLobby.GetComponent<PUN2_GameLobby1>().Status.GetComponent<Text>().text = "username doesnt exist";
                    _GameLobby.GetComponent<PUN2_GameLobby1>().Status.SetActive(true);



                }
                else
                {
                    //Debug.Log(webRequest.downloadHandler.text);
                    if (SecurePasswordHasher.Verify(password, webRequest.downloadHandler.text))
                    {
                        Debug.Log("login succesfull");
                        _GameLobby.GetComponent<PUN2_GameLobby1>().Status.GetComponent<Text>().text = "Success!";
                        _GameLobby.GetComponent<PUN2_GameLobby1>().Status.GetComponent<Text>().color = new Color(0, 1, 0, 1);
                        _GameLobby.GetComponent<PUN2_GameLobby1>().Status.SetActive(true);
                        GetCoinBalance(username);
                        _GameLobby.GetComponent<PUN2_GameLobby1>().SetUserName();
                    }
                    else
                    {
                        Debug.Log("wrong password");
                        _GameLobby.GetComponent<PUN2_GameLobby1>().Status.GetComponent<Text>().text = "wrong password";
                        _GameLobby.GetComponent<PUN2_GameLobby1>().Status.SetActive(true);
                    }

                }

            }
        }

    }


    IEnumerator GetUsers(string username)
    {
        bool isValid = true;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(user_url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
            }
            else
            {
                char[] delimiterChars = { ',' };
                //Debug.Log(webRequest.downloadHandler.text);
                users = webRequest.downloadHandler.text;
                string[] user_list = users.Split(delimiterChars);
                foreach (var user in user_list)
                {
                    if (username.Equals(user))
                    {
                        isValid = false;
                        break;
                    }
                }
                if (isValid)
                {
                    Debug.Log("login succesfull");

                    _GameLobby.GetComponent<PUN2_GameLobby1>().UsernameLoginInput.text = username;
                    _GameLobby.GetComponent<PUN2_GameLobby1>().IsGuest = true;
                    _GameLobby.GetComponent<PUN2_GameLobby1>().SetUserName();
                }
                else
                {
                    Debug.Log("login unsucesfull");

                    _GameLobby.GetComponent<PUN2_GameLobby1>().StatusGuest.SetActive(true);
                }
            }
        }
    }

    IEnumerator CheckIfUserExists(string username, string friend)
    {
        bool Exists = false;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(user_url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
            }
            else
            {
                char[] delimiterChars = { ',' };
                users = webRequest.downloadHandler.text;
                string[] user_list = users.Split(delimiterChars);
                foreach (var user in user_list)
                {
                    if (friend.Equals(user))
                    {
                        Exists = true;
                        break;
                    }
                }
                if (Exists)
                {
                    Debug.Log("User exists.");
                    int pos = Array.IndexOf(_GameLobby.GetComponent<PUN2_GameLobby1>().FriendList, friend);
                    if (pos > -1)
                    {
                        Debug.Log("Friend is already added.");
                        _GameLobby.GetComponent<PUN2_GameLobby1>().AddFriendStatus.GetComponent<Text>().text = "Friend already added.";
                        _GameLobby.GetComponent<PUN2_GameLobby1>().AddFriendStatus.SetActive(true);

                    }
                    else
                    {
                        AddFriend(username,friend);
                    }
                    
                

                }
                else
                {
                    Debug.Log("User doesnt exist.");
                    _GameLobby.GetComponent<PUN2_GameLobby1>().AddFriendStatus.GetComponent<Text>().text = "User doesnt exist";
                    _GameLobby.GetComponent<PUN2_GameLobby1>().AddFriendStatus.SetActive(true);


                }
            }
        }
    }

    IEnumerator Add(string username, string friend)
    {
        string uri = add_friend_url + "user=" + username + "&friend=" + friend;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
            }
            else
            {
                if (webRequest.downloadHandler.text.Equals("true"))
                {
                    Debug.Log("Friend added succesfully.");
                    _GameLobby.GetComponent<PUN2_GameLobby1>().AddFriendStatus.GetComponent<Text>().text = "Friend added succesfully";
                    _GameLobby.GetComponent<PUN2_GameLobby1>().AddFriendStatus.SetActive(true);
                    _GameLobby.GetComponent<PUN2_GameLobby1>().GetFriends(username,0);
                }
                else
                {
                    Debug.Log(webRequest.downloadHandler.text);
                    _GameLobby.GetComponent<PUN2_GameLobby1>().AddFriendStatus.GetComponent<Text>().text = "An error occured";
                    _GameLobby.GetComponent<PUN2_GameLobby1>().AddFriendStatus.SetActive(true);
                }
            }
        }
    }

    IEnumerator CreateUser(string username, string password)
    {
        string uri = create_url;
        string hashed_password = SecurePasswordHasher.Hash(password);
        string post_data = "{ \"username\": \"" + username + "\", \"password\": \"" + hashed_password + "\", \"balance\": 0 }";

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
                Debug.Log(uri);
                Debug.Log(webRequest.downloadHandler.text);
                //Debug.Log(webRequest.downloadHandler.text);
                if (webRequest.downloadHandler.text == "true")
                {
                    Debug.Log("account creation succesfull");
                    _GameLobby.GetComponent<PUN2_GameLobby1>().NewStatus.GetComponent<Text>().text = "account created";
                    _GameLobby.GetComponent<PUN2_GameLobby1>().Status.GetComponent<Text>().color = new Color(0, 1, 0, 1);
                    _GameLobby.GetComponent<PUN2_GameLobby1>().NewStatus.SetActive(true);
                    _GameLobby.GetComponent<PUN2_GameLobby1>().UsernameLoginInput.text = username;
                    GetCoinBalance(username);
                    _GameLobby.GetComponent<PUN2_GameLobby1>().SetUserName();
                }
                else
                {
                    Debug.Log("account creation unsuccesfull");
                    _GameLobby.GetComponent<PUN2_GameLobby1>().NewStatus.GetComponent<Text>().text = "username already exists";
                    _GameLobby.GetComponent<PUN2_GameLobby1>().Status.GetComponent<Text>().color = new Color(1, 0, 0, 1);
                    _GameLobby.GetComponent<PUN2_GameLobby1>().NewStatus.SetActive(true);

                }
            }
        }
    }

    IEnumerator CoinBalance(string username)
    {
        string uri = get_balance_url + "user=" + username;
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
                string balance = webRequest.downloadHandler.text;
                Debug.Log("BALANCE SET: "+balance);
                _GameLobby.GetComponent<PUN2_GameLobby1>().PlayerBalance = Int32.Parse(balance);
                _GameLobby.GetComponent<PUN2_GameLobby1>().BalanceButton.GetComponentInChildren<Text>().text = balance;
                _GameLobby.GetComponent<PUN2_GameLobby1>().BalanceButtonPreGame.GetComponentInChildren<Text>().text = balance;
                _GameLobby.GetComponent<PUN2_GameLobby1>().BalanceButtonLobby.GetComponentInChildren<Text>().text = balance;

            }
        }

    }

    IEnumerator EditBalance(string username, int new_balance,int type)
    {
        if (type == 10)
        {
            _GameLobby.GetComponent<PUN2_GameLobby1>().UnlockPanel.SetActive(true);
            _GameLobby.GetComponent<PUN2_GameLobby1>().UnlockPanelPre.SetActive(true);

        }
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
                    _GameLobby.GetComponent<PUN2_GameLobby1>().BalanceButton.GetComponentInChildren<Text>().text = new_balance.ToString();
                    _GameLobby.GetComponent<PUN2_GameLobby1>().BalanceButtonPreGame.GetComponentInChildren<Text>().text = new_balance.ToString();
                    _GameLobby.GetComponent<PUN2_GameLobby1>().BalanceButtonLobby.GetComponentInChildren<Text>().text = new_balance.ToString();

                }
                else
                {
                    Debug.Log("Balance edit unsuccesful.");
                }


            }
        }
        if (type == 10)
        {
            _GameLobby.GetComponent<PUN2_GameLobby1>().UnlockPanel.SetActive(false);
            _GameLobby.GetComponent<PUN2_GameLobby1>().UnlockPanelPre.SetActive(false);

        }
    }

    IEnumerator UpgradeList(string username)
    {
        //_GameLobby.GetComponent<PUN2_GameLobby1>().InventoryWaitPanel.SetActive(true);

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
                        Debug.Log("UPGRADE " + upgrade);

                        _GameLobby.GetComponent<PUN2_GameLobby1>().PlayerInventory[upgrade] += 1;

                    }
                    foreach (KeyValuePair<string, int> kvp in _GameLobby.GetComponent<PUN2_GameLobby1>().PlayerInventory)
                    {
                        switch (kvp.Key)
                        {
                            case "speed_boots":
                                _GameLobby.GetComponent<PUN2_GameLobby1>().speed_boots_Inventory.text = kvp.Value.ToString();
                                _GameLobby.GetComponent<PUN2_GameLobby1>().speed_boots_InventoryNew.text = kvp.Value.ToString();
                                _GameLobby.GetComponent<PUN2_GameLobby1>().speed_boots_InventoryPre.text = kvp.Value.ToString();
                                break;
                            case "shield":
                                //_GameLobby.GetComponent<PUN2_GameLobby1>().shield_Inventory.text = kvp.Value.ToString();
                                //_GameLobby.GetComponent<PUN2_GameLobby1>().shield_InventoryNew.text = kvp.Value.ToString();
                                //_GameLobby.GetComponent<PUN2_GameLobby1>().shield_InventoryPre.text = kvp.Value.ToString();

                                break;
                            case "vision":
                                _GameLobby.GetComponent<PUN2_GameLobby1>().vision_Inventory.text = kvp.Value.ToString();
                                _GameLobby.GetComponent<PUN2_GameLobby1>().vision_InventoryNew.text = kvp.Value.ToString();
                                _GameLobby.GetComponent<PUN2_GameLobby1>().vision_InventoryPre.text = kvp.Value.ToString();

                                break;
                            case "self_revive":
                                //_GameLobby.GetComponent<PUN2_GameLobby1>().self_revive_Inventory.text = kvp.Value.ToString();
                                //_GameLobby.GetComponent<PUN2_GameLobby1>().self_revive_InventoryNew.text = kvp.Value.ToString();
                                //_GameLobby.GetComponent<PUN2_GameLobby1>().self_revive_InventoryPre.text = kvp.Value.ToString();

                                break;
                            case "fast_hands":
                                _GameLobby.GetComponent<PUN2_GameLobby1>().fast_hands_Inventory.text = kvp.Value.ToString();
                                _GameLobby.GetComponent<PUN2_GameLobby1>().fast_hands_InventoryNew.text = kvp.Value.ToString();
                                _GameLobby.GetComponent<PUN2_GameLobby1>().fast_hands_InventoryPre.text = kvp.Value.ToString();

                                break;
                            case "ninja":
                                _GameLobby.GetComponent<PUN2_GameLobby1>().ninja_Inventory.text = kvp.Value.ToString();
                                _GameLobby.GetComponent<PUN2_GameLobby1>().ninja_InventoryPre.text = kvp.Value.ToString();


                                break;
                            default:
                                break;
                        }
                        Debug.Log("Key = " + kvp.Key + ", Value = " + kvp.Value);
                    }

                }
            }
        }
        //_GameLobby.GetComponent<PUN2_GameLobby1>().InventoryWaitPanel.SetActive(false);
        _GameLobby.GetComponent<PUN2_GameLobby1>().SetOwnedStatus();

    }

    IEnumerator SkinList(string username)
    {

        string uri = get_skins_url + "user=" + username;
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
                    Debug.Log("User has no skins");
                }
                else
                {
                    char[] delimiterChars = { ',' };

                    string[] skin_list = result.Split(delimiterChars);
                    foreach (var skin in skin_list)
                    {
                        Debug.Log("Skin :" + skin);

                        _GameLobby.GetComponent<PUN2_GameLobby1>().PlayerSkins[skin] = true;

                    }
                    

                }
            }
        }
        //_GameLobby.GetComponent<PUN2_GameLobby1>().ControlSkins();

    }

    IEnumerator Add_Upgrade(string username, string upgrade)
    {
        string uri = add_upgrade_url + "user=" + username + "&upgrade=" + upgrade;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
            }
            else
            {
                if (webRequest.downloadHandler.text.Equals("true"))
                {
                    Debug.Log("Upgrade added succesfully.");

                }
                else
                {
                    Debug.Log("Upgrade NOT added succesfully.");

                }
            }
        }
        _GameLobby.GetComponent<PUN2_GameLobby1>().GetInventory();
    }

    IEnumerator Add_Skin(string username, string skin)
    {
        string uri = add_skin_url + "user=" + username + "&skin=" + skin;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
            }
            else
            {
                if (webRequest.downloadHandler.text.Equals("true"))
                {
                    Debug.Log("Skin added succesfully.");

                }
                else
                {
                    Debug.Log("Skin NOT added succesfully.");

                }
            }
        }
    }

    IEnumerator Remove_Upgrade(string username, List<string> upgrade_names)
    {
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
                    List<string> new_upgrade_list = new List<string>();

                    foreach (var upgrade in upgrade_list)
                    {
                        if (!upgrade_names.Contains(upgrade))
                        {
                            new_upgrade_list.Add(upgrade);
                        }
                    }
                    string new_result = String.Join(",", new_upgrade_list.ToArray());
                    EditUpgradeList(username, new_result);
                }
            }
        }
    }

    IEnumerator Remove_Upgrade2(string username, string upgrade_name1, string upgrade_name2)
    {
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
                    List<string> new_upgrade_list = new List<string>();
                    List<string> new_upgrade_list2 = new List<string>();


                    foreach (var upgrade in upgrade_list)
                    {
                        if (!upgrade_name1.Equals(upgrade))
                        {
                            new_upgrade_list.Add(upgrade);
                        }
                    }
                    foreach (var upgrade in new_upgrade_list)
                    {
                        if (!upgrade_name2.Equals(upgrade))
                        {
                            new_upgrade_list2.Add(upgrade);
                        }
                    }
                    string new_result = String.Join(",", new_upgrade_list2.ToArray());
                    EditUpgradeList(username, new_result);
                }
            }
        }
    }

    IEnumerator Edit_Upgrade_List(string username, string new_upgrade_list)
    {
        string uri = edit_upgrade_url + "user=" + username + "&upgrades=" + new_upgrade_list;
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
                if (webRequest.downloadHandler.text.Equals("true "))
                {
                    Debug.Log("Upgrade Succesfully removed");

                }
                else
                {
                    Debug.Log("Upgrade unsuccesfully removed");
                }

            }
        }
        _GameLobby.GetComponent<PUN2_GameLobby1>().GetInventory();


    }



    IEnumerator EditThreshold(string username, int value)
    {
        string uri = edit_threshold_url + "user=" + username + "&value=" + value;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
            }
            else
            {
                if (webRequest.downloadHandler.text.Equals("true"))
                {
                    Debug.Log("Mic threshold changed succesfully.");
                    
                }
                else
                {
                    Debug.Log("Mic threshold change failed.");
                }
            }
        }
    }

    IEnumerator GetThreshold(string username, int state)
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
        if (state == 0) {
            threshold.value = Int32.Parse(threshOut);
        }
    }

    IEnumerator EditMultiplier(string username, int value)
    {
        string uri = edit_multiplier_url + "user=" + username + "&value=" + value;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
            }
            else
            {
                if (webRequest.downloadHandler.text.Equals("true"))
                {
                    Debug.Log("Mic multiplier changed succesfully.");

                }
                else
                {
                    Debug.Log("Mic threshold change failed.");
                }
            }
        }
    }

    IEnumerator GetMultiplier(string username, int state)
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
        }
        if (state == 0) {
            multiplier.value = Int32.Parse(outMulti);
        }
    }


    IEnumerator Friends(string username, int type)
    {
        bool refresh = false;
        if (_GameLobby.GetComponent<PUN2_GameLobby1>().FriendList.Length == 0)
        {
            Debug.Log("FRIEND LIST NULL. REFRESHING");
            refresh = true;
        }
        if (type == 1)
        {
            _GameLobby.GetComponent<PUN2_GameLobby1>().FriendWaitPanel.SetActive(true);
        }
        string uri = get_friends_url + "user=" + username;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
            }
            else
            {
                string friends = webRequest.downloadHandler.text.Trim();
                Debug.Log("Friends: " + friends);
                if (!friends.Equals(" "))
                {
                    char[] delimiterChars = { ',' };
                    string[] friendList = friends.Split(delimiterChars);
                    _GameLobby.GetComponent<PUN2_GameLobby1>().FriendList = friendList;
                }
                
            }
        }
        if (type == 0 || refresh == true)
        {
            _GameLobby.GetComponent<PUN2_GameLobby1>().ContentFriendsNew.GetComponent<PopulateGridFriends>().OnRefresh();
        }
        if (type == 1)
        {
            _GameLobby.GetComponent<PUN2_GameLobby1>().FriendWaitPanel.SetActive(false);
        }

    }

    public static class SecurePasswordHasher
    {
        /// <summary>
        /// Size of salt.
        /// </summary>
        private const int SaltSize = 16;

        /// <summary>
        /// Size of hash.
        /// </summary>
        private const int HashSize = 20;

        /// <summary>
        /// Creates a hash from a password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="iterations">Number of iterations.</param>
        /// <returns>The hash.</returns>
        public static string Hash(string password, int iterations)
        {
            // Create salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

            // Create hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            var hash = pbkdf2.GetBytes(HashSize);

            // Combine salt and hash
            var hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convert to base64
            var base64Hash = Convert.ToBase64String(hashBytes);

            // Format hash with extra information
            return string.Format("$MYHASH$V1${0}${1}", iterations, base64Hash);
        }

        /// <summary>
        /// Creates a hash from a password with 10000 iterations
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>The hash.</returns>
        public static string Hash(string password)
        {
            return Hash(password, 10000);
        }

        /// <summary>
        /// Checks if hash is supported.
        /// </summary>
        /// <param name="hashString">The hash.</param>
        /// <returns>Is supported?</returns>
        public static bool IsHashSupported(string hashString)
        {
            return hashString.Contains("$MYHASH$V1$");
        }

        /// <summary>
        /// Verifies a password against a hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="hashedPassword">The hash.</param>
        /// <returns>Could be verified?</returns>
        public static bool Verify(string password, string hashedPassword)
        {
            // Check hash
            if (!IsHashSupported(hashedPassword))
            {
                throw new NotSupportedException("The hashtype is not supported");
            }

            // Extract iteration and Base64 string
            var splittedHashString = hashedPassword.Replace("$MYHASH$V1$", "").Split('$');
            var iterations = int.Parse(splittedHashString[0]);
            var base64Hash = splittedHashString[1];

            // Get hash bytes
            var hashBytes = Convert.FromBase64String(base64Hash);

            // Get salt
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            // Create hash with given salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Get result
            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}