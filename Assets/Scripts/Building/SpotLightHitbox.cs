using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpotLightHitbox : MonoBehaviour
{
    [SerializeField] private GuardController guardController;
    [SerializeField] private GameObject SpotLight;
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 9) {
            SpotLight.GetComponent<Light>().color = Color.red;
            guardController.GetClosestGuard(other.gameObject.transform.position.x, other.gameObject.transform.position.y, other.gameObject.transform.position.z);
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == 9) {
            SpotLight.GetComponent<Light>().color = Color.white;
        }
    }
}
