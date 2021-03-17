using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuardCanvas : MonoBehaviour
{
    public Sprite sus;
    public Sprite exclamation;
    public GuardController controller;
    public Canvas canvas;
    public List<Image> guardIndicators;
    public Image exampleGuard;

    private void Awake() {
        guardIndicators = new List<Image>();
        foreach (var guard in controller.guardMovements)
        {
            guardIndicators.Add(Instantiate<Image>(exampleGuard, this.transform));
        }
    }
}
