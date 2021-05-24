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
            if (!other.gameObject.GetComponent<PlayerPickUp>().down && other.gameObject.GetComponent<PhotonView>().IsMine) {
                SpotLight.GetComponent<Light>().color = Color.red;
                guardController.MoveClosestGuard(other.gameObject.transform.position);
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == 9) {
            SpotLight.GetComponent<Light>().color = Color.white;
        }
    }
}
