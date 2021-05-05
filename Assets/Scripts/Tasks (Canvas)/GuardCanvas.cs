using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuardCanvas : MonoBehaviour
{
    public Sprite disabledGuard;
    public Sprite disabledCamera;
    public Sprite cameraSprite;
    public Sprite sus;
    public Sprite exclamation;
    public Sprite hat;
    public GuardController guardController;
    public Vector3 offset;
    public Vector3 cameraOffset;
    public PlayerController playerController;
    public Canvas canvas;
    public List<Image> guardIndicators, cameraIndicators;
    public List<CameraFOV> cameraList;
    public Image exampleGuard;
    [SerializeField] private CameraSystem cameraSystem;
    private int disabledFrames;

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
                // Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(guard.gameObject.transform.position + offset);
                Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(guard.gameObject.transform.position + offset + new Vector3(0,(1-cameraSystem.zoomMultiplier)*30,-(1-cameraSystem.zoomMultiplier)*20));
                RectTransform pointerRectTransform = guardIndicator.GetComponent<RectTransform>();
                switch (guard.state)
                {
                    case State.normal:
                        guardIndicator.rectTransform.rotation = Quaternion.identity;
                        guardIndicator.sprite = hat;
                        guardIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(30,30);
                        break;
                    case State.suspicious:
                        guardIndicator.rectTransform.rotation = Quaternion.identity;
                        guardIndicator.sprite = sus;
                        guardIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(15,30);
                        break;
                    case State.chase:
                        guardIndicator.rectTransform.rotation = Quaternion.identity;
                        guardIndicator.sprite = exclamation;
                        guardIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(10,30);
                        break;
                    case State.disabled:
                        disabledFrames++;
                        disabledFrames = disabledFrames % 360;
                        guardIndicator.rectTransform.rotation = Quaternion.Euler(0,0,30 * Mathf.Floor(disabledFrames/5));
                        guardIndicator.sprite = disabledGuard;
                        guardIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(30,30);
                        break;
                }
                pointerRectTransform.anchoredPosition = new Vector2((targetPositionScreenPoint.x - canvas.GetComponent<RectTransform>().position.x)/canvas.scaleFactor, (targetPositionScreenPoint.y - canvas.GetComponent<RectTransform>().position.y)/canvas.scaleFactor+guardIndicator.GetComponent<RectTransform>().sizeDelta.y);
            } else {
                guardIndicator.sprite = null;
                guardIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(0,0);
            }
            i++;
        }
        i = 0;
        foreach (CameraFOV camera in cameraList) {
            Image cameraIndicator;
            if (cameraIndicators.Count <= i) {
                cameraIndicator = Instantiate<Image>(exampleGuard, this.GetComponent<RectTransform>());
                cameraIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(0,0);
                cameraIndicators.Add(cameraIndicator);
            } else {
                cameraIndicator = cameraIndicators[i];
            }
            if (playerController.isInView(camera.gameObject.transform.position)) {
                Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(camera.gameObject.transform.position + cameraOffset + new Vector3(0,(1-cameraSystem.zoomMultiplier)*30,-(1-cameraSystem.zoomMultiplier)*20));
                RectTransform pointerRectTransform = cameraIndicator.GetComponent<RectTransform>();
                switch (camera.cameraState)
                {
                    case State.normal:
                        cameraIndicator.sprite = cameraSprite;
                        cameraIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(30,30);
                        break;
                    case State.suspicious:
                        cameraIndicator.sprite = exclamation;
                        cameraIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(15,30);
                        break;
                }
                pointerRectTransform.anchoredPosition = new Vector2((targetPositionScreenPoint.x - canvas.GetComponent<RectTransform>().position.x)/canvas.scaleFactor, (targetPositionScreenPoint.y - canvas.GetComponent<RectTransform>().position.y)/canvas.scaleFactor+cameraIndicator.GetComponent<RectTransform>().sizeDelta.y);
            } else {
                cameraIndicator.sprite = null;
                cameraIndicator.GetComponent<RectTransform>().sizeDelta = new Vector2(0,0);

            }
            i++;
        }
    }
}
