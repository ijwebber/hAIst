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

    public GameObject wireManual;
    public GameObject wireGame;

    public GameObject codeDisplay;
    public GameObject keycodeGame;

    public GameObject fixPaintingGame;
    [SerializeField] private GameController gameController;
    private PlayerController playerController;
    AudioController audioController;

    private GameObject currentObject;

    public bool keyCorrect;

    public bool paintingCorrect;

    //int gameSelection;
    public float cooldown = 1;
    public bool eDown = false;

    private float startTime = 0f;
    private float timer = 0f;
    private GameObject inTrigger = null;
    public float holdTime = 3.0f;
    private bool held = false;
    private bool codeActive = false;
    public GameObject canvasFromPlayer;

    public bool down;

    public ProgressBarController progressBar;

    void Awake() {
        canvasFromPlayer = GameObject.Find("CanvasFromPlayer");
        gameController = GameObject.FindObjectOfType<GameController>();
        playerController = GameObject.FindObjectOfType<PlayerController>();
        audioController = GameObject.FindObjectOfType<AudioController>();
        Transform[] canvasElements = canvasFromPlayer.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in canvasElements) {
            switch (t.name)
            {
                case "KeycodeTaskCode":
                    codeDisplay = t.gameObject;
                    break;
                case "KeycodeTaskSN":
                    keycodeGame = t.gameObject;
                    Debug.Log("KEY TASK GAME");
                    break;
                case "WireManual":
                    wireManual = t.gameObject;
                    break;
                case "WireTask":
                    wireGame = t.gameObject;
                    break;
                case "PictureTask":
                    fixPaintingGame = t.gameObject;
                    break;
                case "DisplayMessagePlayer":
                    messageBox = t.gameObject.GetComponent<Text>();
                    break;
                case "ProgressBar":
                    progressBar = t.gameObject.GetComponent<ProgressBarController>();
                    break;
                default:
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //game tag thing
        if (photonView.IsMine && PhotonNetwork.IsConnected && inTrigger != null)
        {
            // Debug.Log("Currently in trigger: " + inTrigger.gameObject.tag);
            switch (inTrigger.gameObject.tag) {

                case "button":
                    displayMessage("Press E to press button");
                    if (Input.GetKey(KeyCode.E) && seconds == 0 && !down) {
                        int id = inTrigger.gameObject.GetComponent<PressButton>().id;
                        inTrigger.gameObject.GetComponent<PhotonView>().RPC("ButtonPressed", RpcTarget.All, id);
                    }
                    break;
                case "codedisplay":
                    displayMessage("Press E to see code");
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (!codeDisplay.activeInHierarchy) {
                            Debug.Log("AYO not active " + inTrigger.gameObject.name);
                            codeDisplay.SetActive(true);
                            CodeDisplayObject display = inTrigger.gameObject.GetComponent<CodeDisplayObject>();
                            codeDisplay.GetComponent<CodeDisplay>().keypadID = display.keypad.id;
                            codeActive = true;
                        } else {
                            Debug.Log("AYO active " + inTrigger.gameObject.name);
                            codeDisplay.SetActive(false);
                            codeActive = false;
                        }
                    }
                    break;

                case "keypad":
                    KeyPad keypad = inTrigger.gameObject.GetComponent<KeyPad>();
                    keycodeGame.GetComponent<KeycodeTask>().keypadID = keypad.id;

                    if (Input.GetKeyDown(KeyCode.E) && keycodeGame.activeInHierarchy) {
                        keycodeGame.SetActive(false);
                    }
                    else if (keypad.codeCorrect && !down)
                    {
                        displayMessage("Code already entered");
                    }
                    else if (Input.GetKeyDown(KeyCode.E) && seconds == 0 && !down)
                    {
                        keycodeGame.SetActive(true);
                        displayMessage(2);
                    }
                    else if (keyCorrect && seconds == 0)
                    {

                        targetTime += cooldown;

                        keycodeGame.GetComponent<KeycodeTask>().codeCorrect = false;
                        keycodeGame.SetActive(false);
                        held = false;

                    }
                    else if (!Input.GetKey(KeyCode.E))
                    {
                        displayMessage("Press E to enter code.");
                    }
                    break;
                case "wiremanual":
                    displayMessage("Press E to read manual");
                    if (Input.GetKey(KeyCode.E))
                    {
                        WireManualObject manual = inTrigger.gameObject.GetComponent<WireManualObject>();
                        wireManual.GetComponent<WireManual>().wiresID = manual.wires.id;

                        wireManual.SetActive(true);
                    }
                    break;

                case "wires":
                    Wires wires = inTrigger.gameObject.GetComponent<Wires>();
                    wireGame.GetComponent<WireTask>().wiresID = wires.id;

                    if (wires.complete && !down)
                    {
                        wireGame.SetActive(false);
                        if (wires.correct) displayMessage("Correct wire");
                        else displayMessage("Wrong wire");
                    }
                    else if (Input.GetKey(KeyCode.E) && seconds == 0 && !down)
                    {
                        wireGame.SetActive(true);
                        displayMessage(2);
                    }
                    else if (keyCorrect && seconds == 0)
                    {

                        targetTime += cooldown;

                        wireGame.GetComponent<WireTask>().complete = false;
                        wireGame.SetActive(false);
                        held = false;

                    }
                    else if (!Input.GetKey(KeyCode.E))
                    {
                        displayMessage("Press E to open electrical box");
                    }
                    break;
                case "MetalDoorHandle":
                    if (!inTrigger.gameObject.GetComponent<DoorHandlerKey>().keyPad.codeCorrect && (gameController.gameState == 0 || gameController.gameState == 1)) {
                        displayMessage("This door requires a code");
                        if (gameController.gameState == 0) {
                            gameController.gameState = 1;
                        }
                    }
                    break;
                case "BackDoorHandle":
                    CollectableItem ashes = GameObject.Find("isaacs-ashes").GetComponent<CollectableItem>();
                    if (!inTrigger.gameObject.GetComponent<DoorHandlerKey>().keyPad.codeCorrect) {
                        displayMessage("This door requires a code");
                        if (!ashes.discovered) {
                            this.photonView.RPC("DiscoverAshes", RpcTarget.All);
                        }
                    } else {
                        if (ashes.hidden) {
                            this.photonView.RPC("UnHideAshes", RpcTarget.All);
                        }
                    }
                    break;
            }
        }
        holdTime = playerController.holdTime;
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true){

            down = GetComponent<PlayerMovement>().disabled;

            // if(targetTime > 0){
            //     displayCooldown();
            // }
            // else{cooldownBox.text = "";}

            if(codeDisplay.activeInHierarchy || keycodeGame.activeInHierarchy || fixPaintingGame.activeInHierarchy){
                    displayMessage(2);
            }
       

            keyCorrect = keycodeGame.GetComponent<KeycodeTask>().codeCorrect;   //dont forget to reset these values
            paintingCorrect = fixPaintingGame.GetComponent<RotateTask>().win;

        }
        
    }

    [PunRPC]
    void DiscoverAshes() {
        CollectableItem ashes = GameObject.Find("isaacs-ashes").GetComponent<CollectableItem>();
        ashes.discovered = true;
        gameController.updateQuest();

    }

    [PunRPC]
    void UnHideAshes() {
        CollectableItem ashes = GameObject.Find("isaacs-ashes").GetComponent<CollectableItem>();
        ashes.hidden = false;
        gameController.updateQuest();

    }



    void OnTriggerEnter(Collider other) {
        inTrigger = other.gameObject;
    }

    void OnTriggerExit(Collider other) {
        inTrigger = null;
        if(photonView.IsMine == true && PhotonNetwork.IsConnected == true){
                
            currentObject = null;
            keycodeGame.SetActive(false);
            fixPaintingGame.SetActive(false);
            codeDisplay.SetActive(false);
            wireGame.SetActive(false);
            wireManual.SetActive(false);

            wireGame.GetComponent<WireTask>().complete = false;
            keycodeGame.GetComponent<KeycodeTask>().codeCorrect = false;
            fixPaintingGame.GetComponent<RotateTask>().win = false;
            held = false;
            progressBar.Hide();

            displayMessage(2);
        }   
    }

    private void OnCollisionStay(Collision other) {    // what to do whilst players are in range of object

        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            switch (other.gameObject.tag) {
                case "steal":

                    currentObject = other.gameObject;  // added the current game object

                    int gameSelection = currentObject.GetComponent<CollectableItem>().gameSelection;

                    if (seconds == 0 && !down && held == false)
                    {
                        holdDownTask();
                    }


                    if (held && seconds == 0)
                    { // if both player is in range and the button E is pressed for 5 secpmds, then add points to the score, move this to its own method
                        targetTime += cooldown;

                        //other.gameObject.SetActive(false);
                        UpdateScore(currentObject);
                        bool isSpecial = CheckIfSpecial(currentObject);


                        int objID = currentObject.GetComponent<PhotonView>().ViewID;
                        gameObject.GetComponent<PhotonView>().RPC("hideObject", RpcTarget.All, objID);

                        if (isSpecial) {
                            audioController.PlayHighValue();
                            gameController.playerUpdates.updateDisplay("You have stolen " + other.gameObject.GetComponent<CollectableItem>().itemName + "!");
                            gameController.updateDisp(PhotonNetwork.NickName + " has stolen " + other.gameObject.GetComponent<CollectableItem>().itemName + "!");
                        } else {
                            audioController.PlayLowValue();
                        }

                        // reset game components
                        keycodeGame.SetActive(false);
                        fixPaintingGame.SetActive(false);

                        keycodeGame.GetComponent<KeycodeTask>().codeCorrect = false;
                        fixPaintingGame.GetComponent<RotateTask>().win = false;

                        held = false;


                    }
                    break;

            }
        }      
    }

    private void OnCollisionExit(Collision other) {    // what to do once player leaves

        if(photonView.IsMine == true && PhotonNetwork.IsConnected == true){
                
            currentObject = null;
            keycodeGame.SetActive(false);
            fixPaintingGame.SetActive(false);
            codeDisplay.SetActive(false);
            wireGame.SetActive(false);
            wireManual.SetActive(false);

            wireGame.GetComponent<WireTask>().complete = false;
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
        else if(n==4){messageBox.text = "Hold E for " + holdTime.ToString() + " seconds to pick up"; }
    }
    void displayMessage(string text){
        //Debug.Log("Displaying " + text);
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

        if(Input.GetKeyDown(KeyCode.E) && seconds == 0){    // if player is holding down E, start a timer                     
            startTime = Time.time;
            timer = startTime;
            displayMessage(2);
            progressBar.Show();
            progressBar.ResetBar();
        }

        if(Input.GetKey(KeyCode.E) && held == false && seconds == 0){   // for each time E is being held down, count/increment the timer and remove the onscreen text
            timer += Time.deltaTime;
            progressBar.UpdateBar((timer - startTime), 0, holdTime);      

            if(timer > startTime + holdTime) {   // if the time reaches 5s, then set held to true, else set to false
                held = true;
                startTime = 0f;
                timer = 0f;

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
            displayMessage("Hold E for " + holdTime.ToString() + " seconds to pick up");
        }

    }


    // Updates a players score 
    public void UpdateScore(GameObject obj) {

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
    public void UpdateScore(int value, bool loss) {

        Hashtable props = PhotonNetwork.LocalPlayer.CustomProperties;
        // Increase the current score by the value
        int currentPlayerScore = (int) props["score"]; 
        int newPlayerScore = currentPlayerScore + value;
        
        // Create a hashtable entry with the new score
        Hashtable playerHash = new Hashtable();
        playerHash.Add("score", newPlayerScore);

        int itemStolenCount = (int) PhotonNetwork.LocalPlayer.CustomProperties["itemsStolen"];

        int specialStolenCount = (int) props["specialStolen"];
        Debug.Log("isaac " + itemStolenCount);
        if (loss) {
            specialStolenCount--;
            itemStolenCount--;
        } else {
            specialStolenCount++;
            itemStolenCount++;
        }
        playerHash.Add("itemsStolen", itemStolenCount + 1);
        playerHash.Add("specialStolen", specialStolenCount + 1);

        // Set the player property to the new score
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerHash);
    }

    bool CheckIfSpecial(GameObject obj) {
        CollectableItem item = obj.GetComponent<CollectableItem>();

        if (item.special) {
            int currentFound = (int) PhotonNetwork.CurrentRoom.CustomProperties["special"];
            Hashtable hash = new Hashtable();
            hash.Add("special", currentFound + 1);
            gameController.gameState++;
            playerController.Specials.Add(obj);
            item.stolen = true;

            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }

        return item.special;
    }

    [PunRPC]
    void hideObject(int objID) {  // do the following
       PhotonView.Find(objID).gameObject.SetActive(false);
       PhotonView.Find(objID).GetComponent<CollectableItem>().stolen = true;
    }
}
