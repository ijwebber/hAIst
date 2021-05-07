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
            guardController.GetClosestGuard(SpotLight.transform.position.x, ) (SpotLight, other.gameObject);
        }
    }
}
