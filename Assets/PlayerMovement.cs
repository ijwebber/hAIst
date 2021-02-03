using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 0.2f;

    void Update() {
        if (Input.GetKey("w")) {
            this.transform.localPosition += new Vector3(0, 0, speed);
        }
        if (Input.GetKey("a")) {
            this.transform.localPosition += new Vector3(-speed, 0, 0);
        }
        if (Input.GetKey("s")) {
            this.transform.localPosition += new Vector3(0, 0, -speed);
        }
        if (Input.GetKey("d")) {
            this.transform.localPosition += new Vector3(speed, 0, 0);
        }
        
        
    }
}
