using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;



public class PlayerUI : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI nameText;
    //script for player name tag
    private void Start() 
    {
    
        SetName();
    }

    private void SetName() 
    {
        nameText.text = photonView.Owner.NickName;
    }


}
