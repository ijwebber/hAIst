using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseEnterMap : MonoBehaviour
{
    public GameObject mapOutline;

    public void OnMouseEnter() {
        mapOutline.GetComponent<Image>().color = new Color(255,255,0,200);
    }

    public void OnMouseLeave() {
        mapOutline.GetComponent<Image>().color = new Color(255,255,0,0);
    }

}
