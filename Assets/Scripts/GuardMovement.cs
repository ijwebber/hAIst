using UnityEngine;

public class GuardMovement : MonoBehaviour
{
    public float speed = 5;
    private Rigidbody rb;
    public Vector3 moveDirection;
    public LayerMask isWall;
    public float maxDistFromWall = 2;
 
    void Start () {
        rb = GetComponent<Rigidbody>();
        moveDirection = ChooseDirection();
        transform.rotation = Quaternion.LookRotation(moveDirection);
    }
   
    
    void FixedUpdate () {  
        Vector3 moveVector = moveDirection * speed * Time.deltaTime;
        rb.MovePosition(transform.position + moveVector);
 
        if(Physics.Raycast (transform.position, transform.forward, maxDistFromWall, isWall))
        {
            moveDirection = ChooseDirection();
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }
 
 
    Vector3 ChooseDirection() {
        System.Random Ran = new System.Random();
        int i = Ran.Next(0, 3);
        Vector3 temp = new Vector3 ();

        switch (i) {
            case 0:
                temp = -transform.forward;
                break;

            case 1:
                temp = transform.right;
                break;

            case 2:
                temp = -transform.right;
                break;

            default:
                temp = transform.forward;
                break;
        }

        return temp;
    }
}
