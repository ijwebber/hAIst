using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;
using Photon.Pun;
using Cinemachine;

public class Window_QuestPointer : MonoBehaviourPun
{

    public List<GameObject> targetObjects = new List<GameObject>();
    public Vector3 offset;
    public Canvas canvas;
    private Vector3 targetPosition;
    public PlayerController playerController;
    public GameObject pointerPrefab;
    private List<RectTransform> pointerRectTransforms = new List<RectTransform>();
    private List<Image> pointerImages = new List<Image>();
    public Sprite arrowSprite;
    public Sprite crossSprite;
    public List<GameObject> pointers = new List<GameObject>();
    [SerializeField] private CameraSystem cameraSystem;
    void Awake()
    {
        // targetPosition = new Vector3(-39.615f,42.6f);
        Debug.Log(targetPosition);
        // pointerRectTransform = transform.Find("Pointer").GetComponent<RectTransform>();
        // pointerImage = transform.Find("Pointer").GetComponent<Image>();
    }

    public void updateTarget(string[] gameNames, int gameState) {
        foreach (GameObject pointer in pointers)
        {
            Destroy(pointer);
        }
        targetObjects.Clear();
        pointerRectTransforms.Clear();
        pointerImages.Clear();
        pointers.Clear();
        foreach (string gameName in gameNames)
        {
            if (gameName == "null") {
                // targetObjects.Add(null);
            } else {
                targetObjects.Add(GameObject.Find(gameName));
                GameObject newPointer = Instantiate(pointerPrefab, new Vector3(0,0,0), Quaternion.identity,transform);
                pointers.Add(newPointer);
                pointerRectTransforms.Add(newPointer.GetComponent<RectTransform>());
                pointerImages.Add(newPointer.GetComponent<Image>());
            }
        }
        GameObject.FindObjectOfType<GameController>().gameState = gameState;
    }

    public void UpdateWindowPointer()
    {
        if (targetObjects.Count != 0) {
            for (int i = 0; i < targetObjects.Count; i++)
            {
                targetPosition = new Vector3(targetObjects[i].transform.position.x, targetObjects[i].transform.position.z);
                Vector3 toPosition = targetPosition;
                Vector3 fromPosition = new Vector3(playerController.player.transform.position.x, playerController.player.transform.position.z);
                // fromPosition.y = 0;
                Vector3 dir = (toPosition - fromPosition).normalized;
                float angle = UtilsClass.GetAngleFromVectorFloat(dir);
                pointerRectTransforms[i].localEulerAngles = new Vector3(0,0,angle);
                
                Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetObjects[i].transform.position + offset - cameraSystem.playerCam.GetCinemachineComponent<CinemachineTransposer>().EffectiveOffset);
                bool isOffScreen = !playerController.isInView(targetObjects[i].transform.position);

                if (isOffScreen) {
                    pointerRectTransforms[i].sizeDelta = new Vector2(40,30);
                    pointerImages[i].sprite = arrowSprite;
                    pointerImages[i].color = Color.yellow;
                    pointerRectTransforms[i].localPosition = new Vector3(100*dir.x,100*dir.y);
                } else {
                        pointerRectTransforms[i].sizeDelta = new Vector2(40,30);
                        pointerRectTransforms[i].localEulerAngles = new Vector3(0,0,-90);
                        pointerImages[i].color = Color.green;
                        // pointerImage.sprite = crossSprite;
                        pointerRectTransforms[i].anchoredPosition = new Vector2((targetPositionScreenPoint.x - canvas.GetComponent<RectTransform>().position.x) / canvas.scaleFactor, ((targetPositionScreenPoint.y - canvas.GetComponent<RectTransform>().position.y)/canvas.scaleFactor + 30));
                        // pointerRectTransform.anchoredPosition = new Vector2(Screen.width - targetPositionScreenPoint.x / canvas.scaleFactor, (Screen.height - targetPositionScreenPoint.y) + 60);
                }
            }
        } else {
            // pointerRectTransforms[i].sizeDelta = new Vector2(0,0);
        }
    }
}
