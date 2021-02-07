using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class CameraControlPlayer : MonoBehaviourPun
{

    [Tooltip("The distance in the local x-z plane to the target")]
        [SerializeField]
        private float distance = 0.0f;
        
        [Tooltip("The height we want the camera to be above the target")]
        [SerializeField]
        private float height = 12.0f;
        
        [Tooltip("Allow the camera to be offseted vertically from the target, for example giving more view of the sceneray and less ground.")]
        [SerializeField]
        private Vector3 centerOffset = Vector3.zero;

        [Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
        [SerializeField]
        private bool followOnStart = false;

        [Tooltip("The Smoothing for the camera to follow the target")]
        [SerializeField]
        private float smoothSpeed = 0.125f;

        // cached transform of the target
        Transform cameraTransform;

        // maintain a flag internally to reconnect if target is lost or camera is switched
        bool isFollowing;
        
        // Cache for camera offset
        Vector3 cameraOffset = Vector3.zero;



    // Start is called before the first frame update
    void Start()
        {
            // Start following the target if wanted.
            if (followOnStart)
            {
                OnStartFollowing();
            }
            
        }


    public void OnStartFollowing()
        {         
            cameraTransform = Camera.main.transform;
            isFollowing = true;
            // we don't smooth anything, we go straight to the right camera shot
            //Cut();
        }

    // Update is called once per frame
    void Update()
    {
        if (cameraTransform == null && isFollowing)
            {
                OnStartFollowing();
            }

            // only follow is explicitly declared
            if (isFollowing) {
                Follow ();
            }
    }

    void Follow() 
    {
        cameraOffset.z = distance;
        cameraOffset.y = height;
        cameraTransform.position = this.transform.position + cameraOffset;
    }

}








