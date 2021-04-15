using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    public GameObject player;
    public float viewRadius;
    public LayerMask ObMask;

    [Range(0,360)]
    public bool isDisabled = false;
    public bool isGuest;

    [SerializeField] DBControllerGame dBController;
    // Start is called before the first frame update

    public UpgradeList upgrades = new UpgradeList();
    // upgradables
    public float viewAngle;
    public float moveSpeed = 8;
    public float holdTime = 3;
    public bool shield = false;
    public int invincibleFrames = 0;
    void Start()
    {
        player = getPlayer();
        if (PlayerPrefs.GetInt("isGuest", -1) == 1) {
            isGuest = true;
        } else {
            isGuest = false;
            dBController.GetUpgradeList(PhotonNetwork.NickName);
        }
    }

    void Update() {
        if (invincibleFrames > 0) {
            invincibleFrames--;
        }
    }
    public void disableShield() {
        Debug.Log("Shield consumed");
        shield = false;
        // TODO remove shield from database
        invincibleFrames = 60;
    }

    public GameObject getPlayer() {
        GameObject returnedPlayer = null;
        foreach (var play in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (play.GetComponent<PhotonView>().IsMine && play.name == "Timmy") {
                returnedPlayer = play;
            }
        }
        return returnedPlayer;
    }

    public void applyUpdates() {
        viewRadius += upgrades.vision*.5f;
        moveSpeed *= (1 + .05f*upgrades.speed_boots);
        holdTime -= .1f*upgrades.fast_hands;
        shield = upgrades.shield;
        Debug.Log("Updates loaded");
    }

    public bool isInView(Vector3 targetObject)  {
        return (!Physics.Raycast(targetObject, (player.transform.position - targetObject).normalized, Mathf.Min(viewRadius+3, Vector3.Distance(player.transform.position, targetObject)), ObMask, QueryTriggerInteraction.Ignore) && Vector3.Distance(player.transform.position, targetObject) <= viewRadius+3);
    }
}
