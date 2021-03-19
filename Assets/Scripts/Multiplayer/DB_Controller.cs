using System;
using System.Collections;
using System.Text;
using System.Security.Cryptography;


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

    public void GetFriends(string username)
    {
        StartCoroutine(Friends(username));
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
                    _GameLobby.GetComponent<PUN2_GameLobby1>().GetFriends();
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
                _GameLobby.GetComponent<PUN2_GameLobby1>().BalanceButton.GetComponentInChildren<Text>().text = balance;
                _GameLobby.GetComponent<PUN2_GameLobby1>().BalanceButtonPreGame.GetComponentInChildren<Text>().text = balance;
            }
        }

    }

    IEnumerator Friends(string username)
    {
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