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


        if (button.transform.GetChild(0).GetComponent<Image>().sprite.name == "tuxedo")
        {
            Thief.GetComponent<Image>().sprite = tuxedo;
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
