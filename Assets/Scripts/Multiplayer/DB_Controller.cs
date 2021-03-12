using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DB_Controller : MonoBehaviour
{
    public GameObject _GameLobby;
    string users;
    bool login_bool;
    string login = "https://brasspig.online/check_login.php?";
    string user_url = "https://brasspig.online/get_users.php";
    string create_url = "https://brasspig.online/create_user.php?";

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

    IEnumerator CheckLogin(string username, string password)
    {
        string uri = login + "user=" + username + "&pass=" + password;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                //Debug.Log(webRequest.error);
            }
            else
            {
                if (webRequest.downloadHandler.text == "true ")
                {
                    Debug.Log("login succesfull");
                    _GameLobby.GetComponent<PUN2_GameLobby1>().Status.GetComponent<Text>().text = "Success!";
                    _GameLobby.GetComponent<PUN2_GameLobby1>().Status.GetComponent<Text>().color = new Color(0, 1, 0, 1);
                    _GameLobby.GetComponent<PUN2_GameLobby1>().Status.SetActive(true);
                    _GameLobby.GetComponent<PUN2_GameLobby1>().SetUserName();


                }
                else
                {
                    Debug.Log(webRequest.downloadHandler.text);
                    Debug.Log("login unsucesfull");
                    _GameLobby.GetComponent<PUN2_GameLobby1>().Status.SetActive(true);

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


    IEnumerator CreateUser(string username, string password)
    {
        string uri = create_url + "user=" + username + "&pass=" + password;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                //Debug.Log(webRequest.error);
            }
            else
            {
                if (webRequest.downloadHandler.text == "true ")
                {
                    Debug.Log("account creation succesfull");
                    _GameLobby.GetComponent<PUN2_GameLobby1>().NewStatus.GetComponent<Text>().text = "Success!";
                    _GameLobby.GetComponent<PUN2_GameLobby1>().NewStatus.GetComponent<Text>().color = new Color(0, 1, 0, 1);



                }
                else
                {
                    Debug.Log("account creation unsucesfull");
                    _GameLobby.GetComponent<PUN2_GameLobby1>().NewStatus.SetActive(true);


                }

            }
        }
    }
}
