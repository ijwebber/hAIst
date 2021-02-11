using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    public int size = 5;
    private bool[] isFullList;

    private GameObject[] items;

    public GameObject[] slots;

    public Text itemText;


    void Start() {
        isFullList = new bool[size];
        items = new GameObject[size];

        itemText.enabled = false;
    }

    public void Add(GameObject item) {
        for (int i = 0; i < items.Length; i++) {
            if (isFullList[i] == false) {
                items[i] = item;

                item.transform.position = slots[i].transform.position; // Move object to the inventory camera setup
                item.layer = 14;    // Update to the correct layer to be visible

                isFullList[i] = true;

                break;
            }
        }
    }

    public bool isFull() {
        for (int i = 0; i < items.Length; i++) {
            if (isFullList[i] == false) {
                return false;
            }
        }
        return true;
    }

    public void ShowName(int i) {
        if (isFullList[i - 1]) {
            itemText.enabled = true;
            itemText.text = items[i - 1].name;

            items[i - 1].transform.localScale *= 1.1f; 
        }
    }

    public void HideName(int i) {
        if (isFullList[i - 1]) {
            itemText.enabled = false;
            items[i - 1].transform.localScale /= 1.1f; 
        }
    }
}
