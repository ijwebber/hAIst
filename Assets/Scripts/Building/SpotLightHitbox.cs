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
            guardController.GetClosestGuard(SpotLight.transform.position.x, SpotLight.transform.position.y, SpotLight.transform.position.z);
        }
    }
    void OnTriggerLeave(Collider other) {
        if (other.gameObject.layer == 9) {
            SpotLight.GetComponent<Light>().color = Color.white;
            guardController.GetClosestGuard(SpotLight.transform.position.x, SpotLight.transform.position.y, SpotLight.transform.position.z);
        }
    }
}
