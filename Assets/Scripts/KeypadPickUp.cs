﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeypadPickUp : MonoBehaviour
{
    bool inRange = false;

    bool canvasActive = false;

    public Camera mainCam;  // define camera object


    //private float startTime = 0f;
    //private float timer = 0f;
    //public float holdTime = 5.0f;
    //private bool held = false;

    private string[] paint1 = {"MonaLisa","TheShahin","TilosPride"};

    private int[] scores = {250,600,1000};

    public GameObject keycodeTask;
    private Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;  //link camera object to main camera (that follows the player)
        mainCam.GetComponent<FollowPlayer>().seconds = 0;

        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    
    }

    // Update is called once per frame
    void Update()
    {
        int timeLeft = mainCam.GetComponent<FollowPlayer>().seconds;  // access the seconds variable from the mainCam class/ follow player script

        if(Input.GetKey(KeyCode.E) && inRange && canvasActive == false && timeLeft == 0 && !inventory.isFull()){
            keycodeTask.SetActive(true);
            canvasActive = true;
        } 
        
        if(canvasActive == true && inRange == false){   // check if canvas is true but player is out of range, then disable the canvas
            keycodeTask.SetActive(false);
            canvasActive = false;
        }

        bool keyCorrect = keycodeTask.GetComponent<KeycodeTask>().codeCorrect;  // boolean which just checks if the code is correct

        if(keyCorrect && inRange && timeLeft == 0){ // if both player is in range and the button E is pressed, then add points to the score, move this to its own method

            string paintingName = gameObject.name;
            int paintingIndex = 0;
            bool exists = false;

            for(int i = 0; i < paint1.Length;i++){ // find if painting is part of the list etc...
                if(paint1[i] == paintingName){
                    exists = true;                 // if so, then set exists to true and set the index
                    paintingIndex = i;
                    break;
                }
            }

            if(exists){
                int cost = scores[paintingIndex];   // index of the painting matches the index of its score
                mainCam.GetComponent<FollowPlayer>().points+=cost;  
            }
            else{
                mainCam.GetComponent<FollowPlayer>().points+=10; // if the painting isn't "valuable", then just give player 10 points
            }

            mainCam.GetComponent<FollowPlayer>().targetTime += 11;  // increase time duration
           
            inventory.Add(gameObject); // add object to inventory
            keycodeTask.SetActive(false);
            keyCorrect = false;
            keycodeTask.GetComponent<KeycodeTask>().codeCorrect = false;
        }    
    }

    private void OnTriggerEnter(Collider other) {       // if player enters the box collider of the object, do etc...
        if((other.name == "Timmy")){
            inRange = true;
        }
        
    }

    private void OnTriggerExit(Collider other) {        // if player exits the box collider of the object, do etc...
        if((other.name == "Timmy")){
            inRange = false;
        }
    }
}
