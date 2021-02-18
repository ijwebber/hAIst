using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class FixImageTask : MonoBehaviour
{
    bool inRange = false;

    bool canvasActive = false;

    public GameObject cooldown;

    public GameObject pictureTask;

    // This script is to be used for the "rotate" task and should be applied to the object that is to be stolen
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
        int timeLeft = cooldown.GetComponent<CooldownScript>().seconds;  // access the seconds variable from the mainCam class/ follow player script

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
            cooldown.GetComponent<CooldownScript>().targetTime += 11;  // increase time duration
            
            //inventory.Add(gameObject);
            pictureTask.SetActive(false);
            
            pictureCorrect = false;
            pictureTask.GetComponent<RotateTask>().win = false;
            gameObject.GetComponent<PhotonView>().RPC("destroyObject",RpcTarget.All);
            
        }    
    }

    [PunRPC]
    void destroyObject(){
        Destroy(gameObject);
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
