using UnityEngine;
using Photon;
using Photon.Pun;
using System.Collections;

public class PlayerMovement : MonoBehaviourPun
{
    public float speed = 5;
    [SerializeField] private Rigidbody rb;
    //private TextMesh Caption = null;
    public bool disabled = false;
    private bool timedOut = false;
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

    IEnumerator disableForTime(float disableTime)
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        transform.Rotate(0, 0, 90);
        
        yield return new WaitForSeconds(disableTime);
        disabled = false;
        timedOut = false;
    }

  

    
 
    void FixedUpdate()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        
        //starts disabled timer if knocked out
        if (disabled && !timedOut)
        {
            timedOut = true;
            StartCoroutine(disableForTime(3.0f));

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
}
