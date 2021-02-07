using UnityEngine;
using Photon;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    public float speed = 5;
    [SerializeField] private Rigidbody rb;
    private TextMesh Caption = null;

 
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
        //Caption = this.gameObject.GetComponent<TextMesh>();
        //Caption.text = photonView.Owner.NickName;//string.Format("Player{0}", photonView.ViewID);


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
        
        Vector3 moveVector = new Vector3(horizontal, 0, vertical);
        Quaternion deltaRotation = Quaternion.LookRotation(moveVector);

        moveVector = moveVector.normalized * speed * Time.deltaTime;
        rb.MovePosition(transform.position + moveVector);
        
        rb.rotation = deltaRotation;
    }
}
