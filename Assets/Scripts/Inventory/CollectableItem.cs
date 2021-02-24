using UnityEngine;
using Photon.Pun;

public class CollectableItem : MonoBehaviourPun
{
    public string itemName;
    public int value;
    public bool special = false;

    [PunRPC]
    public void UpdateObject(bool newSpecial, int newValue) {
        Debug.Log(itemName);
        this.value = newValue;
        this.special = newSpecial;
    }
}
