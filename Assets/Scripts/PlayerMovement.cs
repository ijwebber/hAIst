﻿using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    private Rigidbody rb;
    Vector3 m_EulerAngleVelocity;
 
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
        Quaternion deltaRotation = Quaternion.LookRotation(moveVector);

        moveVector = moveVector.normalized * speed * Time.deltaTime;
        rb.MovePosition(transform.position + moveVector);
        
        rb.rotation = deltaRotation;
    }
}
