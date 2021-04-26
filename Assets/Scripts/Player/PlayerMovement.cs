using UnityEngine;
using Photon;
using Photon.Pun;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerMovement : MonoBehaviourPun
{
    public float speed;
    public PlayerController playerController;
    [SerializeField] private Rigidbody rb;
    
    //private TextMesh Caption = null;
    public bool disabled = false;
    public bool paused = false;
    UIController uiController;


    public GameObject objectives;
    public bool objectivesEnabled;
    
    private void Start()
    {

        CameraControlPlayer camera_control = this.gameObject.GetComponent<CameraControlPlayer>();
        if (camera_control != null)
        {  
            if (photonView.IsMine)
            {
                camera_control.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> CameraControlPlayer Component on playerPrefab.", this);
        }

        uiController = GameObject.FindObjectOfType<UIController>();
        playerController = GameObject.FindObjectOfType<PlayerController>();
        objectives = GameObject.Find("Objectives");
    }
    void Update() {
        speed = playerController.moveSpeed;

        if(Input.GetKeyDown(KeyCode.O) && objectives.active == true){
            //objectivesEnabled = false;
            objectives.SetActive(false);
        }
        else if(Input.GetKeyDown(KeyCode.O) && objectives.active == false){
            //objectivesEnabled = true;
            objectives.SetActive(true);
        }
        
    }
 
    void FixedUpdate()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        
        
        

        //if not disabled then get keyboard input
        if (!disabled && !paused)
        {
            
            // player movement - forward, backward, left, right
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 moveVector = new Vector3(horizontal, 0, vertical); //changed 0 to 0.0001 toa avodd error messages

            if (moveVector != Vector3.zero)
            {
                Quaternion deltaRotation = Quaternion.LookRotation(moveVector);
                rb.rotation = deltaRotation;
            }

            if (moveVector != Vector3.zero)
            {
                Quaternion deltaRotation = Quaternion.LookRotation(moveVector);
                rb.rotation = deltaRotation;
            }

            // Checks for any adjustments to speed
            float finalSpeed = speed;
            
            if (Input.GetKey(KeyCode.Space))
            {
                finalSpeed = speed * 0.75f;
            } else if (Input.GetKey(KeyCode.LeftShift))
            {
                finalSpeed = speed * 1.5f;
            }

            moveVector = moveVector.normalized * finalSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + moveVector);
        }
        

        
    }

    [PunRPC]
    void syncDisabled(bool disabledValue)
    {   
        disabled = disabledValue;
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true) {
            playerController.isDisabled = disabledValue;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {{"disabled", disabledValue}});
        }
    }

    void OnTriggerEnter(Collider other) {
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            if (other.gameObject.CompareTag("LocatorColliders")) {
                uiController.UpdateLocationText("Location: " + other.gameObject.name);
            }
        }
    }
}
