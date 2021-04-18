using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{

    public List<TabButton> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public TabButton selectedTab;
    public List<GameObject> objectsToSwap;
    
    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.background.sprite = tabHover;
        }
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }


    public void OnTabSelected(TabButton button)
    {
        selectedTab = button;
        ResetTabs();
        button.background.sprite = tabActive;
        int index = -1;

        if (button.gameObject.name == "SpeedTab")
        {
            index = 0;
        }
        if (button.gameObject.name == "ShieldTab")
        {
            index = 1;
        }
        if (button.gameObject.name == "VisionTab")
        {
            index = 2;
        }
        if (button.gameObject.name == "SelfTab")
        {
            index = 3;
        }
        if (button.gameObject.name == "FastTab")
        {
            index = 4;
        }
        if (button.gameObject.name == "NinjaTab")
        {
            index = 5;
        }


        for (int i = 0; i < objectsToSwap.Count; i ++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else{
                objectsToSwap[i].SetActive(false);
            }
        }


    }

    public void ResetTabs()
    {
        foreach(TabButton button in tabButtons)
        {
            if (selectedTab != null & button == selectedTab) { continue; }
            button.background.sprite = tabIdle;
        }
    }
}
