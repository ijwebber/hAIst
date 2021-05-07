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

    public List<GameObject> Specials = new List<GameObject>();

    public UpgradeList upgrades = new UpgradeList();
    // upgradables
    public float viewAngle;
    public float moveSpeed = 8;
    public float ninjaMultiplier = 1;
    public float holdTime = 3;
    public bool shield = false;
    public bool self_revive = false;
    [SerializeField] private GameObject shieldObj;
    public int invincibleFrames = 0;
    void Start()
    {
        GetUpgrades();
        DebugUpgrades();
        player = getPlayer();
        Debug.Log("EQUIPED SKIN IS " + PlayerPrefs.GetString("skin"));
        if (PlayerPrefs.GetInt("isGuest", -1) == 0) {
            isGuest = false;
            applyUpdates();
        } else {
            isGuest = true;
        }
    }

    void Update() {
        if (invincibleFrames > 0) {
            shieldObj.transform.position = player.transform.position;
            invincibleFrames--;
        } else {
            shieldObj.SetActive(false);
        }
    }
    public void disableShield() {
        shieldObj.SetActive(true);
        Debug.Log("Shield consumed");
        shield = false;
        // TODO remove shield from database
        invincibleFrames = 60;
    }

    public void GetUpgrades()
    {
        upgrades.speed_boots = PlayerPrefs.GetInt("speed_boots", 0);
        upgrades.ninja = PlayerPrefs.GetInt("ninja", 0);
        upgrades.vision = PlayerPrefs.GetInt("vision", 0);
        upgrades.fast_hands = PlayerPrefs.GetInt("fast_hands", 0);
        upgrades.shield = PlayerPrefs.GetInt("shield", 0) == 1;
        upgrades.self_revive = PlayerPrefs.GetInt("self_revive",0) == 1;
    }

    public void DebugUpgrades()
    {
        if (upgrades.speed_boots > 0)
        {
            Debug.Log("SPEED BOOTS ENABLED. LEVEL " + upgrades.speed_boots);
        }
        if (upgrades.vision > 0)
        {
            Debug.Log("VISION. LEVEL " + upgrades.vision);
        }
        if (upgrades.fast_hands > 0)
        {
            Debug.Log("FAST HANDS ENABLED. LEVEL " + upgrades.fast_hands);
        }
        if (upgrades.ninja > 0)
        {
            Debug.Log("NINJA ENABLED. LEVEL " + upgrades.ninja);
        }
        if (upgrades.shield)
        {
            Debug.Log("SHIELD ENABLED.");
        }
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
        viewRadius = 15 + 15*(upgrades.vision*.05f);
        moveSpeed *= (1 + .05f*upgrades.speed_boots);
        holdTime -= .3f*upgrades.fast_hands;
        shield = upgrades.shield;
        self_revive = upgrades.self_revive;
        ninjaMultiplier = 1 - .1f*upgrades.ninja;
        Debug.Log("Updates loaded");
    }

    public bool isInView(Vector3 targetObject)  {
        return (!Physics.Raycast(targetObject, (player.transform.position - targetObject).normalized, Mathf.Min(viewRadius+3, Vector3.Distance(player.transform.position, targetObject)), ObMask, QueryTriggerInteraction.Ignore) && Vector3.Distance(player.transform.position, targetObject) <= viewRadius+3);
    }
}
