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
    public Sprite classic;
    public Sprite red;
    public Sprite radioactive;
    public Sprite white;
         

    public GameObject Thief;

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


        if (button.transform.GetChild(0).GetComponent<Image>().sprite.name == "red")
        {
            Thief.GetComponent<Image>().sprite = red;
        }
        if (button.transform.GetChild(0).GetComponent<Image>().sprite.name == "white")
        {
            Thief.GetComponent<Image>().sprite = white;
        }
        if (button.transform.GetChild(0).GetComponent<Image>().sprite.name == "radioactive")
        {
            Thief.GetComponent<Image>().sprite = radioactive;
        }
        if (button.transform.GetChild(0).GetComponent<Image>().sprite.name == "classic")
        {
            Thief.GetComponent<Image>().sprite = classic;
        }
        if (button.transform.GetChild(0).GetComponent<Image>().sprite.name == "cancel")
        {
            Thief.GetComponent<Image>().sprite = classic;
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
}
