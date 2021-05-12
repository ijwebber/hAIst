using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateBehaviour : MonoBehaviour
{
    [SerializeField] private SoundController soundController;
    [SerializeField] private float soundMul;
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 24) {
            if (collision.relativeVelocity.magnitude > 0.2f) {
                int calculatedSound = (int)(collision.relativeVelocity.magnitude*soundMul);
                if (calculatedSound > 240) {
                    calculatedSound = 240;
                }
                soundController.sendGrid(this.transform.position, calculatedSound);
            }
            Debug.Log("COLLISION magnitude " + collision.relativeVelocity.magnitude);
        }
    }
}
