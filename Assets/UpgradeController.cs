using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UpgradeController : MonoBehaviour
{
    public GameObject _GameLobby;

    public GameObject UpgradePanel;
    public GameObject UpgradePanelConsumables;

    public GameObject UpgradeImagePrefab;
    public GameObject UpgradeImagePrefab2;

    public Sprite speed_boots;
    public Sprite shield;
    public Sprite vision;
    public Sprite self_revive;
    public Sprite fast_hands;






    // Start is called before the first frame update
    void Start()
    {
        PopulateUpdatesPanel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void PopulateUpdatesPanel()
    {
        foreach (Transform child in UpgradePanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in UpgradePanelConsumables.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (KeyValuePair<string, int> kvp in _GameLobby.GetComponent<PUN2_GameLobby1>().PlayerInventory)
        {
            if (kvp.Key.Equals("speed_boots") & kvp.Value > 0)
            {
                GameObject obj = (GameObject)Instantiate(UpgradeImagePrefab2, UpgradePanel.transform);

                obj.transform.GetChild(0).GetComponent<Image>().sprite = speed_boots;
                obj.transform.GetChild(0).GetComponent<TooltipTrigger>().header = "speedy boots";
                obj.transform.GetChild(0).GetComponent<TooltipTrigger>().content = "20% increased movement speed.";
                obj.transform.GetChild(1).gameObject.SetActive(false);

            }
            else if (kvp.Key.Equals("shield") & kvp.Value > 0)
            {
                GameObject obj = (GameObject)Instantiate(UpgradeImagePrefab2, UpgradePanelConsumables.transform);
                obj.transform.GetChild(0).GetComponent<Image>().sprite = shield;
                obj.transform.GetChild(0).GetComponent<TooltipTrigger>().header = "shield";
                obj.transform.GetChild(0).GetComponent<TooltipTrigger>().content = "a shield that blocks your first knock-down.";
            }
            else if (kvp.Key.Equals("vision") & kvp.Value > 0)
            {
                GameObject obj = (GameObject)Instantiate(UpgradeImagePrefab2, UpgradePanel.transform);
                obj.transform.GetChild(0).GetComponent<Image>().sprite = vision;
                obj.transform.GetChild(0).GetComponent<TooltipTrigger>().header = "super glasses";
                obj.transform.GetChild(0).GetComponent<TooltipTrigger>().content = "20% increased vision radius.";
                obj.transform.GetChild(1).gameObject.SetActive(false);

            }
            else if (kvp.Key.Equals("self_revive") & kvp.Value > 0)
            {
                GameObject obj = (GameObject)Instantiate(UpgradeImagePrefab2, UpgradePanelConsumables.transform);
                obj.transform.GetChild(0).GetComponent<Image>().sprite = self_revive;
                obj.transform.GetChild(0).GetComponent<TooltipTrigger>().header = "self revive";
                obj.transform.GetChild(0).GetComponent<TooltipTrigger>().content = "ability to revive yourself once after being knocked down.";
            }
            else if (kvp.Key.Equals("fast_hands") & kvp.Value > 0)
            {
                GameObject obj = (GameObject)Instantiate(UpgradeImagePrefab2, UpgradePanel.transform);
                obj.transform.GetChild(0).GetComponent<Image>().sprite = fast_hands;
                obj.transform.GetChild(0).GetComponent<TooltipTrigger>().header = "fast hands";
                obj.transform.GetChild(0).GetComponent<TooltipTrigger>().content = "20% decreased time to steal.";
                obj.transform.GetChild(1).gameObject.SetActive(false);

            }

        }
    }
}
