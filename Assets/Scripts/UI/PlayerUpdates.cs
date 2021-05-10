using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerUpdates : MonoBehaviourPun
{
    [SerializeField] private GameObject sampleText;
    private List<GameObject> textAssets = new List<GameObject>();
    private bool riseOthers = false;

    public void updateDisplay(string message) {
        var newText = GameObject.Instantiate(sampleText, this.transform.position, Quaternion.identity,this.transform);
        newText.GetComponent<TextMeshProUGUI>().text = message;
        newText.GetComponent<UpdateText>().playerUpdates = this;
        newText.GetComponent<Animator>().SetTrigger("newUpdate");
        if (textAssets.Count > 0) {
            riseOthers = true;
        }
        textAssets.Add(newText);
    }

    public void destroy(TextMeshProUGUI obj) {
        // textAssets.Remove(obj.gameObject);
        // Destroy(obj.gameObject);
    }

    void Update() {
        if (riseOthers) {
            for (int i = 0; i < textAssets.Count-1; i++)
            {
                // textAssets[i].transform.position = new Vector3(168, 45 + 60*n, 0);
                textAssets[i].GetComponent<UpdateText>().rise++;
                textAssets[i].GetComponent<UpdateText>().risen = 0;
            }
            riseOthers = false;
        }
    }
}
