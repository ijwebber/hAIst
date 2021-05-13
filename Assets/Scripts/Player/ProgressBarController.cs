using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    public GameObject front;
    public Gradient gradient;

    public void UpdateBar(float x, float min, float max) {
        front.GetComponent<Image>().fillAmount = (x/(max-min));
        front.GetComponent<Image>().color = gradient.Evaluate(x/(max-min));
    }

    public void ResetBar() {
        // front.GetComponent<RectTransform>().sizeDelta = new Vector2 (0, 16); 
        front.GetComponent<Image>().fillAmount = 0;
        front.GetComponent<Image>().color = gradient.Evaluate(0);
    }

    public void Show() {
        transform.gameObject.SetActive(true);
    }

    public void Hide() {
        transform.gameObject.SetActive(false);
    }
}
