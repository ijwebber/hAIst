using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class PlayerPickUp : MonoBehaviourPun
{

    bool stealObject = false;
    float targetTime = 0;
    public int seconds = 0;
    public Text messageBox;
    public Text cooldownBox;

    public GameObject keycodeGame;

    public GameObject fixPaintingGame;

    private GameObject currentObject;

    public bool keyCorrect;

    public bool paintingCorrect;

    int gameSelection;
    public float cooldown = 1;


    // Update is called once per frame
    void Update()
    {
        
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true){
            if(targetTime > 0){
                displayCooldown();
            }
            else{cooldownBox.text = "";}

            if(keycodeGame.active || fixPaintingGame.active){
                    displayMessage(2);
            }
       

            keyCorrect = keycodeGame.GetComponent<KeycodeTask>().codeCorrect;   //dont forget to reset these values
            paintingCorrect = fixPaintingGame.GetComponent<RotateTask>().win;

        }
        
    }


    private void OnCollisionEnter(Collision other) {    // what to do once player enters

        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true){

            if(other.gameObject.tag == "steal"){            // check to see if item is stealable
                                                        // pick random number here on first collision 
            stealObject = true;        

            currentObject = other.gameObject;

        
            System.Random r = new System.Random();
            gameSelection = r.Next(0,2);
            }                                              
        }
    }

    private void OnCollisionStay(Collision other) {    // what to do whilst players are in range of object

        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            
            Inventory inventory = GetComponent<Inventory>();

            if (other.gameObject.tag == "steal") {

                currentObject = other.gameObject;  // added the current game object

                if (gameSelection == 0) {

                    if(Input.GetKey(KeyCode.E) && !inventory.isFull() && seconds == 0) {
                        keycodeGame.SetActive(true);
                        displayMessage(2);
                    } 
                    else if (keyCorrect && seconds == 0) {   
                        inventory.Add(other.gameObject);
                        targetTime += cooldown;

                        //other.gameObject.SetActive(false);
                        int objID = currentObject.GetComponent<PhotonView>().ViewID;

                        gameObject.GetComponent<PhotonView>().RPC("hideObject", RpcTarget.All, objID);
                        keycodeGame.SetActive(false);
                    } 
                    else if (seconds != 0 ){
                        displayMessage(1);

                    }
                    else if (inventory.isFull()) {
                        displayMessage(3);
                    }
                    else if (!Input.GetKey(KeyCode.E)){
                        displayMessage(0);
                    }

                }

                else if (gameSelection == 1) {

                    if (Input.GetKey(KeyCode.E) && !inventory.isFull() && seconds == 0) {
                        // whip out mini game
                        fixPaintingGame.SetActive(true);
                        displayMessage(2);
                    }
                    else if (paintingCorrect && seconds == 0) {

                        inventory.Add(other.gameObject);
                        targetTime += cooldown;

                        //other.gameObject.SetActive(false);
                        int objID = currentObject.GetComponent<PhotonView>().ViewID;

                        gameObject.GetComponent<PhotonView>().RPC("hideObject", RpcTarget.All, objID);
                        fixPaintingGame.SetActive(false);
                    }
                    else if (seconds != 0 ){
                        displayMessage(1);

                    }
                    else if (inventory.isFull()) {
                        displayMessage(3);
                    }
                    else if (!Input.GetKey(KeyCode.E)) {
                        displayMessage(0);
                        // check cool down
                    }

                }
            }
        }      
    }

    private void OnCollisionExit(Collision other) {    // what to do once player leaves

        if(photonView.IsMine == true && PhotonNetwork.IsConnected == true){
                
                currentObject = null;
                keycodeGame.SetActive(false);
                fixPaintingGame.SetActive(false);

                keycodeGame.GetComponent<KeycodeTask>().codeCorrect = false;
                fixPaintingGame.GetComponent<RotateTask>().win = false;

                displayMessage(2);
        }   
    }


    void displayMessage(int n){

        if(n==0){messageBox.text = "Press E to pick up";}
        else if(n==1){messageBox.text = "Wait for Cooldown";}
        else if(n==2){messageBox.text = "";}
        else if(n==3){messageBox.text = "Inventory Full";}

    }

    void displayCooldown(){
        
        if(targetTime != 0 && !(seconds < 0)){      // perhaps change this so from the pick up script, you set target time to 0 if the time left is 0/ seconds
            targetTime -= Time.deltaTime;           // in float
            seconds = (int)(targetTime % 60);       // convert from float, updating the seconds variable
        }                                           // this chunk of code can be moved to the HUD script
        else if(targetTime < 0 || seconds < 0){
            seconds = 0;
            targetTime = 0;
        }
        else{
            seconds = 0;
            targetTime = 0;
        }

        cooldownBox.text = "Cooldown: " + seconds;

    }

    [PunRPC]
    void hideObject(int objID) {  // do the following
       PhotonView.Find(objID).gameObject.SetActive(false);
    }
}
