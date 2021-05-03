using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CosmeticSkinGroup : MonoBehaviour
{
    public List<CosmeticSkinButton> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public CosmeticSkinButton selectedTab;

    //SPRITES
    public Sprite tuxedo;
    public Sprite pumpkin;

    public GameObject _GameLobby;
    public GameObject Thief;
    public GameObject BuyPanel;

    public void Subscribe(CosmeticSkinButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<CosmeticSkinButton>();
        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(CosmeticSkinButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.background.sprite = tabHover;
        }
    }

    public void OnTabExit(CosmeticSkinButton button)
    {
        ResetTabs();
    }


    public void OnTabSelected(CosmeticSkinButton button)
    {
        selectedTab = button;
        ResetTabs();
        button.background.sprite = tabActive;
        int index = button.transform.GetSiblingIndex();


        if (button.transform.GetChild(0).GetComponent<Image>().sprite.name == "tuxedo")
        {
            Thief.GetComponent<Image>().sprite = tuxedo;
            if (_GameLobby.GetComponent<PUN2_GameLobby1>().PlayerSkins["tuxedo"])
            {
                BuyPanel.transform.GetChild(1).gameObject.SetActive(false);
                BuyPanel.transform.GetChild(2).gameObject.SetActive(true);

            }
            else
            {
                BuyPanel.transform.GetChild(1).gameObject.SetActive(true);
                BuyPanel.transform.GetChild(2).gameObject.SetActive(false);
            }
            
            BuyPanel.SetActive(true);
        }
        else if (button.transform.GetChild(0).GetComponent<Image>().sprite.name == "pumpkin")
        {
            Thief.GetComponent<Image>().sprite = pumpkin;
            if (_GameLobby.GetComponent<PUN2_GameLobby1>().PlayerSkins["pumpkin"])
            {
                BuyPanel.transform.GetChild(1).gameObject.SetActive(false);
                BuyPanel.transform.GetChild(2).gameObject.SetActive(true);

            }
            else
            {
                BuyPanel.transform.GetChild(1).gameObject.SetActive(true);
                BuyPanel.transform.GetChild(2).gameObject.SetActive(false);
            }

            BuyPanel.SetActive(true);
        }



    }

    public void ResetTabs()
    {
        foreach (CosmeticSkinButton button in tabButtons)
        {
            if (selectedTab != null & button == selectedTab) { continue; }
            button.background.sprite = tabIdle;
        }
    }

    public void ResetTabsClose()
    {
        foreach (CosmeticSkinButton button in tabButtons)
        {
            button.background.sprite = tabIdle;
        }
        BuyPanel.SetActive(false);
    }
}
