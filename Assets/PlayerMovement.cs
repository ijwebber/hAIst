using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MovementSpeed =1;
 
    private void Start()
    {
        
    }
 
    void Update()
    {
        // player movement - forward, backward, left, right
        float horizontal = Input.GetAxis("Horizontal") * MovementSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * MovementSpeed * Time.deltaTime;
        
        transform.position += new Vector3(horizontal, 0, vertical);

    }
}
