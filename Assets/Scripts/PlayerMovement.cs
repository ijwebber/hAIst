using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    
    private Rigidbody rb;

    
 
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

    }
 
    void FixedUpdate()
    {
        // player movement - forward, backward, left, right
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 moveVector = new Vector3(horizontal, 0, vertical);

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
