using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System.Collections;



public class PlayerUI : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI nameText;

    private void Start() 
    {
    
        SetName();
    }

    private void SetName() 
    {
        nameText.text = photonView.Owner.NickName;
    }

        

}
