using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
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

        /*if(Input.GetKeyDown(KeyCode.E)){    // if player is holding down E, start a timer
            startTime = Time.time;
            timer = startTime;
        }

        if(Input.GetKey(KeyCode.E) && held == false && timeLeft == 0){   // for each time E is being held down, count/increment the timer
            timer += Time.deltaTime;                    // Needs fix so that if there's still time left, the user doesn't hold down E from 5 seconds for example
            if(timer>(startTime + holdTime)){   // if the time reaches 5s, then set held to true, else set to false
                held = true;
            }
            else{
                held = false;
            }
        }*/
        
        /*if(Input.GetKey(KeyCode.E) == false){ // if key is not being pressed, just set held to false
            held = false;
        }*/

        if(Input.GetKey(KeyCode.E) && inRange && canvasActive == false && timeLeft == 0){
            keycodeTask.SetActive(true);
            canvasActive = true;
        } 
        
        if(canvasActive == true && inRange == false){
            keycodeTask.SetActive(false);
            canvasActive = false;
        }

        bool keyCorrect = keycodeTask.GetComponent<KeycodeTask>().codeCorrect;

        if(keyCorrect && inRange && timeLeft == 0){ // if both player is in range and the button E is pressed, then add points to the score, move this to its own method

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
            keycodeTask.SetActive(false);
            keyCorrect = false;
            keycodeTask.GetComponent<KeycodeTask>().codeCorrect = false;
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
