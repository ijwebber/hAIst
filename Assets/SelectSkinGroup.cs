using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSkinGroup : MonoBehaviour
{
    public List<SelectSkinButton> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public SelectSkinButton selectedTab;

    //SPRITES
    public Sprite classic;
    public Sprite red;
    public Sprite radioactive;
    public Sprite white;
    public Sprite tuxedo;
         

    public GameObject Thief;

    public void Subscribe(SelectSkinButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<SelectSkinButton>();
        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(SelectSkinButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.background.sprite = tabHover;
        }
    }

    public void OnTabExit(SelectSkinButton button)
    {
        ResetTabs();
    }


    public void OnTabSelected(SelectSkinButton button)
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
        if (button.transform.GetChild(0).GetComponent<Image>().sprite.name == "tuxedo")
        {
            Thief.GetComponent<Image>().sprite = tuxedo;
        }
        if (button.transform.GetChild(0).GetComponent<Image>().sprite.name == "cancel")
        {
            Thief.GetComponent<Image>().sprite = classic;
        }


    }

    public void ResetTabs()
    {
        foreach (SelectSkinButton button in tabButtons)
        {
            if (selectedTab != null & button == selectedTab) { continue; }
            button.background.sprite = tabIdle;
        }
    }
}
