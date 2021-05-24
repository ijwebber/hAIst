using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CrateBehaviour : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private SoundController soundController;
    [SerializeField] private float soundMul;
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    void OnCollisionEnter(Collision collision) {
        // if collision with wall or floor
        if (collision.gameObject.layer == 8 || collision.gameObject.layer == 24) {
            if (collision.relativeVelocity.magnitude > 0.2f) {
                int calculatedSound = (int)(collision.relativeVelocity.magnitude*soundMul);
                if (calculatedSound > 240) {
                    calculatedSound = 240;
                }
                // send output sound to grid
                soundController.sendGrid(this.transform.position, calculatedSound);
            }
        }
    }

    // lag compensation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(GetComponent<Rigidbody>().position);
            stream.SendNext(GetComponent<Rigidbody>().rotation);
            stream.SendNext(GetComponent<Rigidbody>().velocity);
        }
        else
        {
            networkPosition = (Vector3) stream.ReceiveNext();
            networkRotation = (Quaternion) stream.ReceiveNext();
            GetComponent<Rigidbody>().velocity = (Vector3) stream.ReceiveNext();

            float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.timestamp));
            networkPosition += (GetComponent<Rigidbody>().velocity * lag);
        }
    }

    //lag compensation
    void FixedUpdate() {
        if (!photonView.IsMine)
        {
            GetComponent<Rigidbody>().position = Vector3.MoveTowards(GetComponent<Rigidbody>().position, networkPosition, Time.fixedDeltaTime);
            GetComponent<Rigidbody>().rotation = Quaternion.RotateTowards(GetComponent<Rigidbody>().rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
        }

    }
}
