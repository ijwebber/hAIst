using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

    //int gameSelection;
    public float cooldown = 1;

    private float startTime = 0f;
    private float timer = 0f;
    public float holdTime = 5.0f;
    private bool held = false;

    public bool down;

    public ProgressBarController progressBar;

    // Update is called once per frame
    void Update()
    {
        
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true){

            down = GetComponent<PlayerMovement>().disabled;

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
                                              
        }
    }

    private void OnCollisionStay(Collision other) {    // what to do whilst players are in range of object

        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {

            if (other.gameObject.tag == "steal") {

                currentObject = other.gameObject;  // added the current game object

                int gameSelection = currentObject.GetComponent<CollectableItem>().gameSelection;

                if (gameSelection == 0) {

                    if(Input.GetKey(KeyCode.E) && seconds == 0 && !down) {
                        keycodeGame.SetActive(true);
                        displayMessage(2);
                    } 
                    else if (keyCorrect && seconds == 0) {   
                        
                        targetTime += cooldown;

                        //other.gameObject.SetActive(false);
                        UpdateScore(currentObject);
                        CheckIfSpecial(currentObject);

                        int objID = currentObject.GetComponent<PhotonView>().ViewID;
                        gameObject.GetComponent<PhotonView>().RPC("hideObject", RpcTarget.All, objID);
                        keycodeGame.SetActive(false);
                        fixPaintingGame.SetActive(false);

                        keycodeGame.GetComponent<KeycodeTask>().codeCorrect = false;
                        fixPaintingGame.GetComponent<RotateTask>().win = false;
                        keycodeGame.SetActive(false);
                        held = false;
                        
                    } 
                    else if (seconds != 0 ){
                        displayMessage(1);

                    }
                    else if (!Input.GetKey(KeyCode.E)){
                        displayMessage(0);
                    }

                }

                else if (gameSelection == 1) {

                    if (Input.GetKey(KeyCode.E) && seconds == 0 && !down) {
                        // whip out mini game
                        fixPaintingGame.SetActive(true);
                        displayMessage(2);
                    }
                    else if (paintingCorrect && seconds == 0) {

                        
                        targetTime += cooldown;

                        //other.gameObject.SetActive(false);
                        UpdateScore(currentObject);
                        CheckIfSpecial(currentObject);

                        int objID = currentObject.GetComponent<PhotonView>().ViewID;
                        gameObject.GetComponent<PhotonView>().RPC("hideObject", RpcTarget.All, objID);
                        keycodeGame.SetActive(false);
                        fixPaintingGame.SetActive(false);

                        keycodeGame.GetComponent<KeycodeTask>().codeCorrect = false;
                        fixPaintingGame.GetComponent<RotateTask>().win = false;
                        fixPaintingGame.SetActive(false);
                        held = false;
                        
                    }
                    else if (seconds != 0 ){
                        displayMessage(1);

                    }
                    else if (!Input.GetKey(KeyCode.E)) {
                        displayMessage(0);
                        // check cool down
                    }

                }

                else if(gameSelection == 2){        // hold down task

                    if(seconds == 0 && !down){
                        holdDownTask();
                    }
                    
                    else if(seconds != 0){
                        displayMessage(1);
                    }
                    

                    if(held && seconds == 0){ // if both player is in range and the button E is pressed for 5 secpmds, then add points to the score, move this to its own method
                        targetTime += cooldown;

                        //other.gameObject.SetActive(false);
                        UpdateScore(currentObject);
                        CheckIfSpecial(currentObject);

                        int objID = currentObject.GetComponent<PhotonView>().ViewID;
                        gameObject.GetComponent<PhotonView>().RPC("hideObject", RpcTarget.All, objID);
                        
                        // reset game components
                        keycodeGame.SetActive(false);
                        fixPaintingGame.SetActive(false);

                        keycodeGame.GetComponent<KeycodeTask>().codeCorrect = false;
                        fixPaintingGame.GetComponent<RotateTask>().win = false;

                        held = false;
                        
                          
                    }   
                }
            }  else if (other.gameObject.tag == "button") {
                displayMessage("Press E to press button");
                if (Input.GetKey(KeyCode.E) && seconds == 0 && !down) {
                    int id = other.gameObject.GetComponent<PressButton>().id;
                    other.gameObject.GetComponent<PhotonView>().RPC("ButtonPressed", RpcTarget.All, id);
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
                held = false;
                progressBar.Hide();
                

                displayMessage(2);
        }   
    }


    void displayMessage(int n){

        if(n==0){messageBox.text = "Press E to pick up";}
        else if(n==1){messageBox.text = "Wait for Cooldown";}
        else if(n==2){messageBox.text = "";}
        else if(n==3){messageBox.text = "Inventory Full";}
        else if(n==4){messageBox.text = "Hold E for 5 seconds to pick up";}
    }
    void displayMessage(string text){
        messageBox.text = text;
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

    void holdDownTask(){

        if(Input.GetKeyDown(KeyCode.E)){    // if player is holding down E, start a timer                     
            startTime = Time.time;
            timer = startTime;
            displayMessage(2);
            progressBar.Show();
            progressBar.ResetBar();
        }

        if(Input.GetKey(KeyCode.E) && held == false && seconds == 0){   // for each time E is being held down, count/increment the timer and remove the onscreen text
            timer += Time.deltaTime;
            progressBar.UpdateBar((timer - startTime), 0, holdTime);      

            if(timer>(startTime + holdTime)) {   // if the time reaches 5s, then set held to true, else set to false
                held = true;
                displayMessage(2);
                progressBar.Hide();
                progressBar.ResetBar();
            }
            else {
                held = false;
            }
        }

        if(Input.GetKey(KeyCode.E) == false){ // if key is not being pressed, just set held to false
            held = false;
            progressBar.ResetBar();
            progressBar.Hide();
            displayMessage(4);
        
        }

    }


    // Updates a players score 
    void UpdateScore(GameObject obj) {

        // Get the item and it's value
        CollectableItem item = obj.GetComponent<CollectableItem>();
        int value = item.value;

        Hashtable props = PhotonNetwork.LocalPlayer.CustomProperties;
        // Increase the current score by the value
        int currentPlayerScore = (int) props["score"]; 
        int newPlayerScore = currentPlayerScore + value;
        
        // Create a hashtable entry with the new score
        Hashtable playerHash = new Hashtable();
        playerHash.Add("score", newPlayerScore);

        int itemStolenCount = (int) PhotonNetwork.LocalPlayer.CustomProperties["itemsStolen"];
        playerHash.Add("itemsStolen", itemStolenCount + 1);

        if (item.special) {
            int specialStolenCount = (int) props["specialStolen"];
            Debug.Log("isaac " + itemStolenCount);
            playerHash.Add("specialStolen", specialStolenCount + 1);
        }

        // Set the player property to the new score
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerHash);
    }

    void CheckIfSpecial(GameObject obj) {
        CollectableItem item = obj.GetComponent<CollectableItem>();

        if (item.special) {
            int currentFound = (int) PhotonNetwork.CurrentRoom.CustomProperties["special"];
            Hashtable hash = new Hashtable();
            hash.Add("special", currentFound + 1);

            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }
    }

    [PunRPC]
    void hideObject(int objID) {  // do the following
       PhotonView.Find(objID).gameObject.SetActive(false);
       
    }
}
