using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseEnterPostIt : MonoBehaviour
{
    public void OnMouseEnter() {
        this.GetComponent<Image>().color = new Color32(255,255,255,55);
    }
    
    public void OnMouseLeave() {
        this.GetComponent<Image>().color = new Color32(255,255,255,255);
    }
}
