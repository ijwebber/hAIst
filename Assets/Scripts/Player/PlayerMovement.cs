﻿using UnityEngine;
using Photon;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    public float speed = 5;
    [SerializeField] private Rigidbody rb;
    //private TextMesh Caption = null;
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
    }

    
 
    void FixedUpdate()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        // player movement - forward, backward, left, right
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 moveVector = new Vector3(horizontal, 0, vertical); //changed 0 to 0.0001 toa avodd error messages

        if (moveVector != Vector3.zero) 
        {
        Quaternion deltaRotation = Quaternion.LookRotation(moveVector);
        rb.rotation = deltaRotation;
        }

        if (moveVector != Vector3.zero) {
            Quaternion deltaRotation = Quaternion.LookRotation(moveVector);
            rb.rotation = deltaRotation;
        }

        // Checks for any adjustments to speed
        float finalSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift)) {
            finalSpeed =  speed * 1.5f;
        } else if (Input.GetKey(KeyCode.Space)) {
            finalSpeed = speed * 0.75f;
        }

        moveVector = moveVector.normalized * finalSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + moveVector);
    }
}