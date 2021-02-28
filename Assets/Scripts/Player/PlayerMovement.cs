using UnityEngine;
using Photon;
using Photon.Pun;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerMovement : MonoBehaviourPun
{
    public float speed = 5;
    public PlayerController playerController;
    [SerializeField] private Rigidbody rb;
    
    //private TextMesh Caption = null;
    public bool disabled = false;
    
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
        playerController = GameObject.FindObjectOfType<PlayerController>();
    }

   

  

    
 
    void FixedUpdate()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        
        

        //if not disabled then get keyboard input
        if (!disabled)
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
            if (Input.GetKey(KeyCode.LeftShift))
            {
                finalSpeed = speed * 1.5f;
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                finalSpeed = speed * 0.75f;
            }

            moveVector = moveVector.normalized * finalSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + moveVector);
        }
        

        
    }

    [PunRPC]
    void syncDisabled(bool disabledValue)
    {   
        disabled = disabledValue;
        playerController.isDisabled = disabledValue;
        if (photonView.IsMine == true && PhotonNetwork.IsConnected == true) {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() {{"leave", disabledValue}});
        }
    }
}
