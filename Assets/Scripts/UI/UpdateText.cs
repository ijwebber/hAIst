
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class UpdateText : MonoBehaviour
{
    public PlayerUpdates playerUpdates;
    public int rise = 0;
    public float risen = 0;
    public void destroyObject() {
        playerUpdates.destroy(this.GetComponent<TextMeshProUGUI>());
    }

    void Update() {
        if (rise > 0 && risen < 40*rise) {
            this.transform.position += new Vector3(0,5,0);
            risen+=5;
        }
    }
}