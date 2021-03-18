using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuardCanvas : MonoBehaviour
{
    public Sprite sus;
    public Sprite exclamation;
    public Sprite hat;
    public GuardController guardController;
    public Vector3 offset;
    public PlayerController playerController;
    public Canvas canvas;
    public List<Image> guardIndicators;
    public Image exampleGuard;

    private void Awake() {
        guardIndicators = new List<Image>();
    }

    private void Update() {
        int i = 0;
        foreach (GuardMovement guard in guardController.guardMovements)
        {
            Image guardIndicator;
            if (guardIndicators.Count <= i) {
                guardIndicator = Instantiate<Image>(exampleGuard, this.GetComponent<RectTransform>());
                guardIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(0,0);
                guardIndicators.Add(guardIndicator);
            } else {
                guardIndicator = guardIndicators[i];
            }
            if (playerController.isInView(guard.gameObject.transform.position)) {
                Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(guard.gameObject.transform.position + offset);
                RectTransform pointerRectTransform = guardIndicator.GetComponent<RectTransform>();
                switch (guard.state)
                {
                    case State.normal:
                        guardIndicator.sprite = hat;
                        guardIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(30,30);
                        break;
                    case State.suspicious:
                        guardIndicator.sprite = sus;
                        guardIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(15,30);
                        break;
                    case State.chase:
                        guardIndicator.sprite = exclamation;
                        guardIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(10,30);
                        break;
                }
                pointerRectTransform.anchoredPosition = new Vector2((targetPositionScreenPoint.x - canvas.GetComponent<RectTransform>().position.x)/canvas.scaleFactor, (targetPositionScreenPoint.y - canvas.GetComponent<RectTransform>().position.y)/canvas.scaleFactor+guardIndicator.GetComponent<RectTransform>().sizeDelta.y);
            } else {
                guardIndicator.sprite = null;
                guardIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(0,0);
            }
            i++;
        }
    }
}
