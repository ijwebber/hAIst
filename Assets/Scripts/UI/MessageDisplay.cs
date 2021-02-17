using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MessageDisplay : MonoBehaviour
{

    public string message = "";
    public Text messageBox;

    public Camera  mainCam;

    public bool inRange = false;

    
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        
        message = mainCam.GetComponent<Message>().messageD;
        messageBox.text = message;
    }
}
