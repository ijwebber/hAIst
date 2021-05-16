using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
public class DifficultySettings : MonoBehaviourPun
{
    public TextMeshProUGUI moveSliderText, moveMultiplier, radiusSliderText, radiusMultiplier, timerSliderText, timerMultiplier, totalMultiplier;
    public Slider moveSlider, radiusSlider, timerSlider;

    public void btnSaveClick() {
        PhotonHashTable defaultSettings = new PhotonHashTable();
        defaultSettings.Add("movespeedSetting", (float)(moveSlider.value/2));
        defaultSettings.Add("radiusSetting", (int)radiusSlider.value);
        defaultSettings.Add("timeSetting", (int)timerSlider.value);
        defaultSettings.Add("scoreMultiplier", (float)((moveSlider.value - 7)*5 + (radiusSlider.value-10)*2 + (4 - timerSlider.value)*2));
        PhotonNetwork.CurrentRoom.SetCustomProperties(defaultSettings);
    }
    public void SetupDifficulties() {
        float moveSpeedSetting = (float)PhotonNetwork.CurrentRoom.CustomProperties["movespeedSetting"];
        int radiusSetting = (int)PhotonNetwork.CurrentRoom.CustomProperties["radiusSetting"];
        int timeSetting = (int)PhotonNetwork.CurrentRoom.CustomProperties["timeSetting"];
        moveSlider.value = moveSpeedSetting*2;
        radiusSlider.value = radiusSetting;
        timerSlider.value = timeSetting;
        moveSlider.interactable = PhotonNetwork.IsMasterClient;
        radiusSlider.interactable = PhotonNetwork.IsMasterClient;
        timerSlider.interactable = PhotonNetwork.IsMasterClient;
        moveSliderText.text = (moveSpeedSetting).ToString();
        radiusSliderText.text = radiusSetting.ToString();
        timerSliderText.text = timeSetting.ToString();
        moveMultiplier.text = (5*(moveSpeedSetting*2 - 7)).ToString() + "%";
        radiusMultiplier.text = ((radiusSetting-10)*2).ToString() + "%";
        timerMultiplier.text = ((4-timeSetting)*2).ToString() + "%";
        totalMultiplier.text = "Total: " + ((moveSlider.value - 7)*5 + (radiusSlider.value-10)*2 + (4 - timerSlider.value)*2).ToString() + "%";
    }

    public void moveSliderUpdate() {
        moveSliderText.text = (moveSlider.value/2).ToString();
        moveMultiplier.text = ((moveSlider.value-7)*5).ToString() + "%";
        totalMultiplier.text = "Total: " + ((moveSlider.value - 7)*5 + (radiusSlider.value-10)*2 + (4 - timerSlider.value)*2).ToString() + "%";
    }

    public void radiusSliderUpdate() {
        radiusSliderText.text = radiusSlider.value.ToString();
        radiusMultiplier.text = ((radiusSlider.value - 10)*2).ToString() + "%";
        totalMultiplier.text = "Total: " + ((moveSlider.value - 7)*5 + (radiusSlider.value-10)*2 + (4 - timerSlider.value)*2).ToString() + "%";
    }

    public void timerSliderUpdate() {
        timerSliderText.text = timerSlider.value.ToString();
        timerMultiplier.text = ((4 - timerSlider.value)*2).ToString() + "%";
        totalMultiplier.text = "Total: " + ((moveSlider.value - 7)*5 + (radiusSlider.value-10)*2 + (4 - timerSlider.value)*2).ToString() + "%";
    }

    public void btnDefaultClick() {
        float moveSpeedSetting = 3.5f;
        int radiusSetting = 10;
        int timeSetting = 4;
        moveSlider.value = moveSpeedSetting*2;
        radiusSlider.value = radiusSetting;
        timerSlider.value = timeSetting;
        moveSliderText.text = (moveSpeedSetting).ToString();
        radiusSliderText.text = radiusSetting.ToString();
        timerSliderText.text = timeSetting.ToString();
        moveMultiplier.text = (2*(moveSpeedSetting*2 - 6)).ToString() + "%";
        radiusMultiplier.text = ((radiusSetting-10)*2).ToString() + "%";
        timerMultiplier.text = ((4-timeSetting)*2).ToString() + "%";
        totalMultiplier.text = "Total: " + ((moveSlider.value - 6)*2 + (radiusSlider.value-10)*2 + (4 - timerSlider.value)*2).ToString() + "%";
    }
}
