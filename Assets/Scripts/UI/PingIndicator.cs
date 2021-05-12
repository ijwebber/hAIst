using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PingIndicator : MonoBehaviourPun
{
    [SerializeField] private Sprite pingGood, pingOkay, pingPoor, pingAwful;
    [SerializeField] private Image pingIndicator;
    [SerializeField] private TextMeshProUGUI pingText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int ping = PhotonNetwork.GetPing();
        pingText.text = ping.ToString();
        if (ping < 100) {
            pingIndicator.sprite = pingGood;
        } else if (ping < 150) {
            pingIndicator.sprite = pingOkay;
        } else if (ping < 200) {
            pingIndicator.sprite = pingPoor;
        } else {
            pingIndicator.sprite = pingAwful;
        }
    }
}
