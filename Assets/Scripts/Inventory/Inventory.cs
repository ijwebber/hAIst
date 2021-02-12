using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    public int size = 5;
    private bool[] isFullList;

    private GameObject[] items;

    private CollectableItem[] itemInfos;

    public GameObject[] slots; // Inventory object slots (give position for the inventory)

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

                item.transform.position = slots[i].transform.position; // Move object to the inventory camera setup
                item.layer = 14;    // Update to the correct layer to be visible
                
                break;
            }
        }

        UpdateScore();
    }

    public void Remove(int i) {
        if (isFullList[i - 1]) {
            isFullList[i - 1] = false;
            GameObject item = items[i - 1];
            item.layer = 0;
            items[i - 1].transform.localScale /= 1.1f;

            itemInfos[i - 1].ReturnToInitialPosition();
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
            

            items[i - 1].transform.localScale *= 1.1f; 
        }
    }

    public void HideName(int i) {
        if (isFullList[i - 1]) {
            itemText.enabled = false;
            items[i - 1].transform.localScale /= 1.1f; 
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
}
