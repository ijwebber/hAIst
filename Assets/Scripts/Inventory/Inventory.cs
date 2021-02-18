using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    // Size of inventory
    public int size = 5;

    // Lists to store info
    private bool[] isFullList;
    private GameObject[] items;
    private CollectableItem[] itemInfos;

    // Inventory Canvas
    public GameObject inventoryCanvas;
    
    // Text on GUI
    public Text itemText;
    public Text scoreText;



    void Start() {
        isFullList = new bool[size];
        items = new GameObject[size];
        itemInfos = new CollectableItem[size];

        itemText.enabled = false;
    }

    public void Add(GameObject item) {
        for (int i = 0; i < items.Length; i++) {
            if (isFullList[i] == false) {
                items[i] = item;
                isFullList[i] = true;
                itemInfos[i] = item.GetComponent<CollectableItem>();

                UpdateImage(i);
                break;
            }
        }
        UpdateScore();
    }
    void UpdateImage(int i) {
        Transform inv = inventoryCanvas.transform.Find("Inventory");
        RawImage image = inv.GetChild(i).GetChild(0).gameObject.GetComponent<RawImage>();
        image.texture = itemInfos[i].image;
    }

    public void Remove(int i) {
        if (isFullList[i - 1]) {
            isFullList[i - 1] = false;
            GameObject item = items[i - 1];

            item.SetActive(true);
            itemText.enabled = false;
            UpdateScore();
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
            CollectableItem info = itemInfos[i - 1];
            itemText.text = info.itemName + " | $" + info.value.ToString();
        }
    }

    public void HideName(int i) {
        if (isFullList[i - 1]) {
            itemText.enabled = false;
        }
    }

    public int Score() {
        int score = 0;

        for (int i = 0; i < items.Length; i++) {
            if (isFullList[i]) {
                score += itemInfos[i].value;
            }
        }

        return score;
    }

    void UpdateScore() {
        scoreText.text = "Score: $" + Score().ToString();
    }

    public void Show() {
        inventoryCanvas.SetActive(true);
    }

    public void Hide() {
        inventoryCanvas.SetActive(false);
    }

    

}
