using UnityEngine;
using Photon.Pun;

public class CollectableItem : MonoBehaviourPun
{
    public string itemName;
    public int value;
    public bool stolen = false;
    public bool special = false;
    public GameObject guardPoint = null;
    public bool hidden;
    public bool discovered;
    public bool doorOpened;
    public GameObject keyPad;
    public GameObject codeDisplay;

    public int gameSelection;

    [PunRPC]
    public void UpdateObject(int newValue, int chooseMinigame) {
        this.value = newValue;
        this.gameSelection = chooseMinigame;
    }
}
