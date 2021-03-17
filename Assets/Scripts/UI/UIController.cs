using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject infoMessage;
    public GameObject locationText;

    void Start()
    {
        Setup();
    }

    void Setup() {
        infoMessage.SetActive(false);
    }

    // Change what the info box text shows, also shows the box
    public void UpdateInfoText(string text) {
        infoMessage.GetComponent<Text>().text = text;
        ShowInfoBox();
    }

    public void HideInfoBox() {
        infoMessage.SetActive(false);
    }

    public void ShowInfoBox() {
        infoMessage.SetActive(true);
    }

    public void UpdateLocationText(string text) {
        locationText.GetComponent<Text>().text = text;
    }
}
