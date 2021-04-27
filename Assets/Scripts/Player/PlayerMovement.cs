﻿using UnityEngine;
using Photon;
using Photon.Pun;
using System.Collections;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerMovement : MonoBehaviourPun
{
    public float speed;
    public PlayerController playerController;
    [SerializeField] private Rigidbody rb;
    
    //private TextMesh Caption = null;
    public bool disabled = false;
    public bool paused = false;
    private Image staminaBar;
    private float stamina = 1;
    private float alpha = 1;
    private bool tired = false;
    private float staminaR = 1;
    private float staminaB = 1;
    private float staminaG = 1;
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
        staminaBar = GameObject.Find("StaminaBar").GetComponent<Image>();
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

            // Checks for any adjustments to speed
            float finalSpeed = speed;
            
            if (Input.GetKey(KeyCode.LeftShift)) {
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
            staminaBar.color = new Color(staminaR,staminaG,staminaB,alpha);

            moveVector = moveVector.normalized * finalSpeed * Time.deltaTime;
            staminaBar.fillAmount = stamina;
            rb.MovePosition(transform.position + moveVector);
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

    void OnTriggerEnter(Collider other) {
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true)
        {
            if (other.gameObject.CompareTag("LocatorColliders")) {
                uiController.UpdateLocationText("Location: " + other.gameObject.name);
            }
        }
    }
}
