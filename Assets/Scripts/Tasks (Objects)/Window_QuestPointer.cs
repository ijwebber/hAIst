using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;
using Photon.Pun;

public class Window_QuestPointer : MonoBehaviourPun
{

    public GameObject targetObject;
    public Vector3 offset;
    public Canvas canvas;
    private Vector3 targetPosition;
    public PlayerController playerController;
    private RectTransform pointerRectTransform;
    private Image pointerImage;
    public Sprite arrowSprite;
    public Sprite crossSprite;
    void Awake()
    {
        // targetPosition = new Vector3(-39.615f,42.6f);
        Debug.Log(targetPosition);
        pointerRectTransform = transform.Find("Pointer").GetComponent<RectTransform>();
        pointerImage = transform.Find("Pointer").GetComponent<Image>();
    }

    [PunRPC]
    void updateTarget(string gameName) {
        if (gameName == "null") {
            targetObject = null;
        } else {
            targetObject = GameObject.Find(gameName);
        }
    }

    void LateUpdate()
    {
        if (targetObject != null) {
            targetPosition = new Vector3(targetObject.transform.position.x, targetObject.transform.position.z);
            Vector3 toPosition = targetPosition;
            Vector3 fromPosition = new Vector3(GameObject.Find("Timmy").transform.position.x, GameObject.Find("Timmy").transform.position.z);
            // fromPosition.y = 0;
            Vector3 dir = (toPosition - fromPosition).normalized;
            float angle = UtilsClass.GetAngleFromVectorFloat(dir);
            pointerRectTransform.localEulerAngles = new Vector3(0,0,angle);
            
            Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetObject.transform.position + offset);
            bool isOffScreen = !playerController.isInView(targetObject.transform.position);

            if (isOffScreen) {
                pointerRectTransform.sizeDelta = new Vector2(40,30);
                pointerImage.sprite = arrowSprite;
                pointerImage.color = Color.yellow;
                pointerRectTransform.localPosition = new Vector3(100*dir.x,100*dir.y);
            } else {
                    pointerRectTransform.sizeDelta = new Vector2(40,30);
                    pointerRectTransform.localEulerAngles = new Vector3(0,0,-90);
                    pointerImage.color = Color.green;
                    // pointerImage.sprite = crossSprite;
                    pointerRectTransform.anchoredPosition = new Vector2((targetPositionScreenPoint.x - canvas.GetComponent<RectTransform>().position.x) / canvas.scaleFactor, ((targetPositionScreenPoint.y - canvas.GetComponent<RectTransform>().position.y)/canvas.scaleFactor + 30));
                    // pointerRectTransform.anchoredPosition = new Vector2(Screen.width - targetPositionScreenPoint.x / canvas.scaleFactor, (Screen.height - targetPositionScreenPoint.y) + 60);
            }
        } else {
            pointerRectTransform.sizeDelta = new Vector2(0,0);
        }
    }
}
