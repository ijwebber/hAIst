﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour
{
    public PlayerController playerController;
    public Vector3 offset;
    public Canvas canvas;
    public Image indicator;
    [SerializeField] private Sprite crown;
    [SerializeField] private Sprite downed;
    private void Update() {
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(playerController.player.transform.position + offset);
        RectTransform pointerRectTransform = indicator.GetComponent<RectTransform>();
        if (playerController.isDisabled) {
            indicator.sprite = downed;
        } else {
            indicator.sprite = crown;
        }
        pointerRectTransform.anchoredPosition = new Vector2((targetPositionScreenPoint.x - canvas.GetComponent<RectTransform>().position.x) / canvas.scaleFactor, (targetPositionScreenPoint.y - canvas.GetComponent<RectTransform>().position.y) / canvas.scaleFactor +indicator.GetComponent<RectTransform>().sizeDelta.y);
    }
}