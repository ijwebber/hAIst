using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    public GameObject front;
    public Gradient gradient;

    public void UpdateBar(float x, float min, float max) {
        float width = (x / (max - min)) * 200;
        front.GetComponent<RectTransform>().sizeDelta = new Vector2 (width, 16); 
        front.GetComponent<Image>().color = gradient.Evaluate(x/(max-min));
    }

    public void ResetBar() {
        front.GetComponent<RectTransform>().sizeDelta = new Vector2 (0, 16); 
    }

    public void Show() {
        transform.gameObject.SetActive(true);
    }

    public void Hide() {
        transform.gameObject.SetActive(false);
    }
}
