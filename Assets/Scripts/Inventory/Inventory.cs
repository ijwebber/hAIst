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
        if (isFullList[i]) {
            itemText.enabled = true;
            itemText.text = items[i].name; 
        }
    }
}
