using UnityEngine;
using Photon;
using Photon.Pun;
using System.Collections;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerMovement : MonoBehaviourPun, IPunObservable
{
    public float speed;
    public PlayerController playerController;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Gradient col;
    
    //private TextMesh Caption = null;
    public bool disabled = false;
    public bool paused = false;
    private Image staminaBar, staminaIndicator, staminaBarBg;
    private float stamina = 1;
    private float alpha = 1;
    private bool tired = false;
    private float staminaR = 1;
    private float staminaB = 1;
    private float staminaG = 1;
    private float finalSpeed;
    UIController uiController;
    private new Rigidbody rigidbody;
    private Vector3 networkPosition;
    private Vector3 finalmoveVector;
    private float networkSpeed;
    private Quaternion networkRotation;


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
        staminaBar = GameObject.Find("StaminaBar").GetComponent<Image>();
        staminaBarBg = GameObject.Find("StaminaBarBG").GetComponent<Image>();
        staminaIndicator = GameObject.Find("StaminaBolt").GetComponent<Image>();
        uiController = GameObject.FindObjectOfType<UIController>();
        playerController = GameObject.FindObjectOfType<PlayerController>();
        objectives = GameObject.Find("Objectives");
        rigidbody = this.GetComponent<Rigidbody>();
    }

    void Update() {
        speed = playerController.moveSpeed;

        if(Input.GetKeyDown(KeyCode.O) && objectives.activeInHierarchy == true){
            //objectivesEnabled = false;
            objectives.SetActive(false);
        }
        else if(Input.GetKeyDown(KeyCode.O) && objectives.activeInHierarchy == false){
            //objectivesEnabled = true;
            objectives.SetActive(true);
        }
        
    }
 
 // lag compensation
public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    if (stream.IsWriting)
    {
        stream.SendNext(rigidbody.position);
        stream.SendNext(rigidbody.rotation);
        stream.SendNext(finalmoveVector);
        stream.SendNext(finalSpeed);
    }
    else
    {
        networkPosition = (Vector3) stream.ReceiveNext();
        networkRotation = (Quaternion) stream.ReceiveNext();
        Vector3 networkMoveVector = (Vector3) stream.ReceiveNext();
        networkSpeed = (float) stream.ReceiveNext();

        float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
        networkPosition += (networkMoveVector * lag);
    }
}
    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            //lag compensation
            // rb.MovePosition(networkPosition);
            rb.position = Vector3.Lerp(rigidbody.position, networkPosition, Time.fixedDeltaTime*networkSpeed);
            rb.rotation = Quaternion.RotateTowards(rigidbody.rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
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

            // Checks for any adjustments to speed
            finalSpeed = speed;
            
            if (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Space)) {
                if (!tired && moveVector != Vector3.zero) {
                    stamina -= 0.005f;
                    finalSpeed = speed * 1.5f;
                } else {
                    //flash stamina bar
                    stamina += .003f;
                    if (stamina >= 1) {
                        stamina = 1;
                    }
                }
                if (stamina <= 0) {
                    tired = true;
                    stamina = 0;
                }
            } else {
                if (Input.GetKey(KeyCode.Space))
                {
                    finalSpeed = speed * 0.75f;
                }
                stamina += .003f;
                if (stamina >= 1) {
                    stamina = 1;
                }
            }
            if (tired && stamina >= 1) {
                tired = false;
            }
            if (tired) {
                staminaR = staminaB = staminaG = .25f;
            } else {
                staminaR = staminaB = staminaG = 1;
            }
            if (stamina >= 1) {
                alpha -= .05f;
            } else {
                alpha += .05f;
            }
            if (alpha > 1) {
                alpha = 1;
            } else if (alpha < 0) {
                alpha = 0;
            }
            // update stamina bar
            staminaBar.color = new Color(col.Evaluate(stamina).r * staminaR,col.Evaluate(stamina).g * staminaG,col.Evaluate(stamina).b * staminaB,alpha);
            staminaBarBg.color = new Color(0.06603771f,0.05606976f,0.05606976f,alpha);
            staminaIndicator.color = new Color(staminaR,staminaG,staminaB,alpha);

            finalmoveVector = moveVector.normalized * finalSpeed * Time.deltaTime;
            staminaBar.fillAmount = stamina;
            rb.MovePosition(transform.position + finalmoveVector);
        }
        

        
    }

    [PunRPC]
    void syncDisabled(bool disabledValue)
    {   
        disabled = disabledValue;
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true) {
            playerController.isDisabled = disabledValue;
            PlayerController playerContoller = GameObject.Find("PlayerController").GetComponent<PlayerController>();
            bool selfRevive = playerContoller.self_revive;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {{"disabled", disabledValue && !selfRevive}});
        }
    }

    // Update the location of the player on their UI.
    void OnTriggerEnter(Collider other) {
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            // update location (top right of screen)
            if (other.gameObject.CompareTag("LocatorColliders")) {
                uiController.UpdateLocationText("Location: " + other.gameObject.name);
            }
        }
    }
}
