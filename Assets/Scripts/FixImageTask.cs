using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixImageTask : MonoBehaviour
{
    bool inRange = false;

    bool canvasActive = false;

    public Camera mainCam;  // define camera object

    private string[] paint1 = {"MonaLisa","TheShahin","TilosPride"};

    private int[] scores = {250,600,1000};

    public GameObject pictureTask;

    // This script is to be used for the "rotate" task and should be applied to the object that is to be stolen
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;  //link camera object to main camera (that follows the player)
        mainCam.GetComponent<FollowPlayer>().seconds = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(inRange){print(gameObject.name);}      // keep track of whether player is in range or not
        else{print("NOT IN RANGE");}

        int timeLeft = mainCam.GetComponent<FollowPlayer>().seconds;  // access the seconds variable from the mainCam class/ follow player script

        if(Input.GetKey(KeyCode.E) && inRange && canvasActive == false && timeLeft == 0){
            pictureTask.SetActive(true);
            canvasActive = true;
        } 
        
        if(canvasActive == true && inRange == false){
            pictureTask.SetActive(false);
            canvasActive = false;
        }

        bool pictureCorrect = pictureTask.GetComponent<RotateTask>().win;

        if(pictureCorrect && inRange && timeLeft == 0){ // if both player is in range and the button E is pressed, then add points to the score, move this to its own method

            string paintingName = gameObject.name;
            int paintingIndex = 0;
            bool exists = false;

            for(int i = 0; i <= paint1.Length;i++){ // find if painting is part of the list etc...
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
                mainCam.GetComponent<FollowPlayer>().points+=10; // if the painting isn't "valuable", then just give it 10 points
            }

            mainCam.GetComponent<FollowPlayer>().targetTime += 11;  // increase time duration
            pictureTask.SetActive(false);
            pictureCorrect = false;
            pictureTask.GetComponent<RotateTask>().win = false;
            Destroy(gameObject);
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
