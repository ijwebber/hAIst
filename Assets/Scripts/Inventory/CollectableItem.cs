using UnityEngine;
using Photon.Pun;

public class CollectableItem : MonoBehaviourPun
{
    public string itemName;
    public int value;
    public bool special = false;

    public int gameSelection;

    [PunRPC]
    public void UpdateObject(bool newSpecial, int newValue, int chooseMinigame) {
        this.value = newValue;
        this.special = newSpecial;
        this.gameSelection = chooseMinigame;
    }
}
