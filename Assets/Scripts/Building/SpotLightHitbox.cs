using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private GuardController guardController;
    [SerializeField] private GameObject SpotLight;
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 8) {
            if (!guardController.playersSpotted) {
                guardController.cutsceneSpotlight(SpotLight, other.gameObject);
            }
        }
    }
}
