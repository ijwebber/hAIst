using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float bound = 90f;
    public float speed = 0.1f;

    private bool goingRight = true;
    private float initialYRotation;
    private float maxBound;
    private float minBound;
    private float currentRotation;

    void Start() {
        initialYRotation = transform.eulerAngles.y;
        maxBound = bound;
        minBound = -bound;

        currentRotation = 0;
    }

    void Update()
    {   
        if (!GetComponent<CameraProps>().disabled) {
            if (currentRotation > maxBound || currentRotation < minBound) {
                goingRight = !goingRight;
            }
            
            float rot;
            if (goingRight) {
                rot = speed;
            } else {
                rot = -speed;
            }
            
            currentRotation += rot;
            transform.Rotate(0, rot, 0);
        }        
    }
}
