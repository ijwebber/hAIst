using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{

    public int slotNumber;
    private Inventory inventory;
    public GameObject player;
    
    void Start() {
        inventory = player.GetComponent<Inventory>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        inventory.ShowName(slotNumber);
    }

    public void OnPointerExit(PointerEventData eventData) {
        inventory.HideName(slotNumber);
    }

}
