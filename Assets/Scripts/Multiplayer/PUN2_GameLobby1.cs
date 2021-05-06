using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


using Photon.Pun;
using Photon.Realtime;
using TMPro;

/* PLACES TO ADD NEW UPGRADES:
1. BuyUpgrade function
2. Upgrade image in upgrade menu (and description)
3. Image in inventory
4. Text object in pregamelobby script
5. Switch statement in DB_Controller
6. SetUsername function
*/

/* PLACES TO ADD NEW SKINS:
1. Import sprite asses in cosmetic folder
2. Add skin in dictionary in set username and the rest
3. Cosmetic skin group file additions
4. ControlSkin fuction in pun2gamelobby and pun2gamelobby sprite gameobject
5. Select skin group
6. New character model prefab
7. Pregame set player skins function and sprite gameobject
*/
public class PUN2_GameLobby1 : MonoBehaviourPunCallbacks
{

string gameVersion = "0.9";
bool joiningRoom = false;
public GameObject DB_Controller;
public GameObject PreGameScript;
private ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

public int PlayerBalance;
public Dictionary<string, int> PlayerInventory = new Dictionary<string, int>();
public Dictionary<string, bool> PlayerSkins = new Dictionary<string, bool>();


// MENUS
[SerializeField] private GameObject GuestMenu;
[SerializeField] private GameObject StartMenu;
[SerializeField] private GameObject UserManagerMenu;
[SerializeField] private GameObject ExistingUserMenu;
[SerializeField] private GameObject NewUserMenu;
[SerializeField] private GameObject HomeMenu;
[SerializeField] private GameObject LobbyMenu;
[SerializeField] private GameObject NewLobbyMenu;
[SerializeField] private GameObject LoadingScreenLogIn;


[SerializeField] private GameObject PreGameMenu;
[SerializeField] private GameObject CreditsMenu;
[SerializeField] private GameObject RejoinWaitPanel;

[SerializeField] private GameObject PreGameHome;
[SerializeField] private GameObject MapScreen;

// SCRIPTS
[SerializeField] private GameObject menu_script;
[SerializeField] private GameObject LobbyScript;
[SerializeField] private GameObject ContentLobby;
[SerializeField] private GameObject ContentFriends;
[SerializeField] public GameObject ContentFriendsNew;




// USER MANAGER GAMEOBJECTS
public GameObject StatusGuest;

// EXISTING USER GAMEOBJECTS
public TMP_InputField UsernameLoginInput;
[SerializeField] TMP_InputField PasswordLoginInput;
public GameObject Status;

// NEW USER GAMEOBJECTS
public GameObject NewStatus;
public GameObject UsernameShort;

public GameObject PasswordError;
public TMP_InputField UsernameCreationInput;
[SerializeField] TMP_InputField PasswordCreationInput;

// GUEST GAMEOBJECTS
[SerializeField] TMP_InputField UsernameInput;
[SerializeField] private GameObject StartButton;
private AuthenticationValues authValues;

// HOME SCREEN GAMEOBJECTS
public Button BalanceButton;
public GameObject thief_1;
public GameObject thief_1_home;
public GameObject thief_2;
public GameObject thief_3;
public GameObject thief_4;
public GameObject FriendsMenu;
public GameObject MicMenu;
public GameObject Home_Home;
public GameObject NewHome;
public GameObject UpgradeMenu;
public GameObject InventoryMenu;

//Locker
public GameObject LockerPanelHome;
public GameObject ThiefSkins;
public GameObject ThiefSkinsPre;
public SelectSkinGroup skinGroup;
public SelectSkinGroup skinGroupPre;

//Cosmetic Store
public GameObject CosmeticPanelHome;
public Button CosmeticStoreButton;
public CosmeticSkinGroup skinGroupCosmetic;
public GameObject UnlockingPanelCosmetics;
public GameObject ThiefCosmetics;
public Button BuySkinButton;
public GameObject OwnedSkinButton;



// SKINS
public GameObject SkinIconPrefab;
public GameObject SkinPanelContent;
public GameObject SkinPanelContentPre;


public Sprite classic;
public Sprite red;
public Sprite radioactive;
public Sprite white;
public Sprite tuxedo;
public Sprite pumpkin;


public GameObject UnlockPanel;
public GameObject UnlockPanelPre;
public GameObject FriendWaitPanel;

public GameObject InventoryWaitPanel;

public GameObject FriendPanel;


public string[] FriendList;
public GameObject AddFriendStatus;
public TMP_InputField AddFriendInput;
public bool IsGuest = false;
public Button FriendsMenuButton;
public Button UpgradesMenuButton;
public GameObject BalanceInfoHome;
[SerializeField] private Button btnHome, btnUpgrades, btnMic, btnRooms;

// UPGRADES OBJECTS
public TMP_Text speed_boots_Inventory;
public TMP_Text shield_Inventory;
public TMP_Text vision_Inventory;
public TMP_Text self_revive_Inventory;
public TMP_Text fast_hands_Inventory;
public TMP_Text ninja_Inventory;


public TMP_Text speed_boots_InventoryNew;
public TMP_Text shield_InventoryNew;
public TMP_Text vision_InventoryNew;
public TMP_Text self_revive_InventoryNew;
public TMP_Text fast_hands_InventoryNew;


public TextMeshProUGUI speed_boots_cost;
public TextMeshProUGUI fast_hands_cost;
public TextMeshProUGUI vision_cost;


public Button shield_unlock;
public Button self_revive_unlock;
public Button speed_boots_unlock;

public Button shield_owned;
public Button self_revive_owned;
public Button speed_boots_owned;



public Button shield_unlock_pre;
public Button self_revive_unlock_pre;
public Button speed_boots_unlock_pre;

public Button shield_owned_pre;
public Button self_revive_owned_pre;
public Button speed_boots_owned_pre;



public TMP_Text speed_boots_InventoryPre;
public TMP_Text shield_InventoryPre;
public TMP_Text vision_InventoryPre;
public TMP_Text self_revive_InventoryPre;
public TMP_Text fast_hands_InventoryPre;
public TMP_Text ninja_InventoryPre;


public GameObject speed_boots_page;
public GameObject vision_page;
public GameObject fast_hands_page;
public GameObject ninja_page;

public GameObject speed_boots_page_pre;
public GameObject vision_page_pre;
public GameObject fast_hands_page_pre;
public GameObject ninja_page_pre;



// LOBBY MENU OBJECTS
public Button BalanceButtonLobby;
public GameObject BalanceInfoLobby;
public GameObject UpgradeButtonFromLobby;



// MIC MENU OBJECT
private bool MicCheck;
[SerializeField] private Button saveMicButton;
[SerializeField] private TextMeshProUGUI multiplierTextAsset;
[SerializeField] private TextMeshProUGUI threshTextAsset;
public Slider Multiplier;
public Slider Threshold;
public Slider slider;



// PRE GAME OBJECTS
public AnimateBG bg;
public Button BalanceButtonPreGame;
public GameObject BalanceInfoPre;
public GameObject RoomNameButton;
public GameObject LobbyScreen;
public GameObject UpgradeScreen;

public GameObject ChooseUpgradesButton;
public GameObject UpgradeButtonFromPreGame;

// menu buttons
[SerializeField] private Button btnPreHome, btnPreUpgrade, btnPreMap, btnPreLobby;

// PHOTON NETWORK GAMEOBJECTS 

// MAP SCREEN OBJECTS
[SerializeField] private GameObject Notes;
[SerializeField] private GameObject MapIndicator;
[SerializeField] private GameObject mapOutline;



// Use this for initialization
void Start()
{
#if UNITY_WEBGL && !UNITY_EDITOR
    Microphone.Init();
    Microphone.QueryAudioInput();
#endif
}

void Awake()
{
    //DontDestroyOnLoad(this);
    if (PhotonNetwork.IsConnected)
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Play Again Rejoin");

            ReJoinAfterPlayAgain();
        }
        else
        {
            Debug.Log("IsConnectedRejoin");
            ReJoinAfterLeave();
        }
    }

}

void Connect()
{
    if (!PhotonNetwork.IsConnected)
    {
            //Set the App version before connecting
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = gameVersion;
            // Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
            PhotonNetwork.ConnectUsingSettings();
    }


}


// USER MANAGER MENU
public void ExistingUser()
{
    UserManagerMenu.SetActive(false);
    ExistingUserMenu.SetActive(true);
}
public void Guest()
{
    UserManagerMenu.SetActive(false);
    GuestMenu.SetActive(true);
}
public void NewButton()
{
    UserManagerMenu.SetActive(false);
    NewUserMenu.SetActive(true);
}

// NEW USER MENU
public void NewAccount()
{

    if(UsernameCreationInput.text.Length < 5){
        UsernameShort.SetActive(true);
        PasswordError.SetActive(false);

    }
    else if(!(checkPassword(PasswordCreationInput.text,6))){
        PasswordError.SetActive(true);
        UsernameShort.SetActive(false);
    }
    else{
        PasswordError.SetActive(false);
        UsernameShort.SetActive(false);
        DB_Controller.GetComponent<DB_Controller>().Create(UsernameCreationInput.text, PasswordCreationInput.text);
    }

}

static bool checkPassword(string input, int minimum){

    bool hasNum = false;
    bool hasCap = false;
    bool hasLow = false;
    bool hasSpec = false;
    char currentChar;

    if(!(input.Length >= minimum)){
        return false;
    }

    for(int i = 0; i < input.Length; i++){
        currentChar = input[i];
        if(char.IsDigit(currentChar)){ 
            hasNum = true;
        }
        else if(char.IsUpper(currentChar)){ 
            hasCap = true;
        }
        else if(char.IsLower(currentChar)){ 
            hasLow = true;
        }
        else if(!(char.IsLetterOrDigit(currentChar))){ 
            hasSpec = true;
        }
        if(hasNum && hasCap && hasLow && hasSpec){ 
            return true;
        }
    }

    return false;
}

// EXISTING USER MENU
public void SignIn()
{
    DB_Controller.GetComponent<DB_Controller>().Login(UsernameLoginInput.text, PasswordLoginInput.text);
}

// GUEST MENU
public void SignInGuest()
{
    DB_Controller.GetComponent<DB_Controller>().CheckUsername(UsernameInput.text);
}

// HOME MENU
public void EnableFriendsMenu()
{
    FriendsMenu.SetActive(true);
    //Home_Home.SetActive(false);
    NewHome.SetActive(false);
    UpgradeMenu.SetActive(false);
    //InventoryMenu.SetActive(false);
    NewLobbyMenu.SetActive(false);
    MicMenu.SetActive(false);
    MicCheck = false;
    AddFriendStatus.SetActive(false);
    ContentFriends.GetComponent<PopulateGridFriends>().OnRefresh();
}

public void EnableMicThreshold() {
    btnHome.interactable = true;
    if (!IsGuest) {
        btnUpgrades.interactable = true;
    }
    btnMic.interactable = false;
    btnRooms.interactable = true;
    DB_Controller.GetComponent<DB_Controller>().getThresholds(PhotonNetwork.NickName);
    MicMenu.SetActive(true);
    Color32 blueCol = new Color32(93,93,118,255);
    saveMicButton.GetComponent<Image>().color = blueCol;
    MicCheck = true;
    NewHome.SetActive(false);
    NewLobbyMenu.SetActive(false);
    UpgradeMenu.SetActive(false);
    FriendsMenu.SetActive(false);
    AddFriendStatus.SetActive(false);
}

void Update() {
#if UNITY_WEBGL && !UNITY_EDITOR
    if (MicCheck) {
        Microphone.Update();
        string[] devices = Microphone.devices;

        float[] volumes = Microphone.volumes;

        if (devices.Length > 1) {
            int index = 0;
            string deviceName = devices[index];
            if (deviceName == null)
            {
                deviceName = string.Empty;
            }

            float volume = 0;
            if (Multiplier.value != null) {
                volume = volumes[index]*Multiplier.value;
            }
            slider.value = volume;
        }
    }
#endif

}
public void updateMultiplierSlider() {
    multiplierTextAsset.text = Multiplier.value.ToString();
    saveMicButton.GetComponent<Image>().color = new Color32(186,158,48,255);
}

public void updateThresholdSlider() {
    threshTextAsset.text = Threshold.value.ToString();
    saveMicButton.GetComponent<Image>().color = new Color32(186,158,48,255);
}

public void defaultMicSettings() {
    Threshold.value = 2;
    Multiplier.value = 240;
    saveMicButton.GetComponent<Image>().color = new Color32(186,158,48,255);
}

public void saveMicClicked() {
    saveMicButton.GetComponent<Image>().color = new Color32(93,93,118,255);
}

public void EnableLobbyMenu()
{
    FriendsMenu.SetActive(false);
    UpgradeMenu.SetActive(false);
    MicMenu.SetActive(false);
    MicCheck = false;
    //InventoryMenu.SetActive(false);
    NewLobbyMenu.SetActive(false);

    btnHome.interactable = false;
    if (!IsGuest) {
        btnUpgrades.interactable = true;
    }
    btnMic.interactable = true;
    btnRooms.interactable = true;



    AddFriendStatus.SetActive(false);
    ContentFriendsNew.GetComponent<PopulateGridFriends>().OnRefresh();
    //Home_Home.SetActive(true);
    NewHome.SetActive(true);

}

public void EnableLockerPanel()
{
    ControlSkins();
    LockerPanelHome.SetActive(true);
}

public void DisableLockerPanel()
{
    EquipSkin();
    LockerPanelHome.SetActive(false);
}

public void EnableCosmeticStorePanel()
{
    CosmeticPanelHome.SetActive(true);
}

public void DisableCosmeticStorePanel()
{
    CosmeticPanelHome.SetActive(false);
    skinGroupCosmetic.ResetTabsClose();
}

public void BuySkin()
{
    string skin_name = ThiefCosmetics.GetComponent<Image>().sprite.name;
    int skin_price = int.Parse(BuySkinButton.transform.GetChild(1).GetComponent<TMP_Text>().text);

    if (PlayerBalance >= skin_price)
    {
        UnlockingPanelCosmetics.SetActive(true);          
        PlayerBalance = PlayerBalance - skin_price;
        DB_Controller.GetComponent<DB_Controller>().EditCoinBalance(PhotonNetwork.NickName, PlayerBalance, 10);
        DB_Controller.GetComponent<DB_Controller>().AddSkin(PhotonNetwork.NickName, skin_name);
        BuySkinButton.gameObject.SetActive(false);
        OwnedSkinButton.SetActive(true);

    }
}

public void EquipSkin()
{
    string skin_name = ThiefSkins.GetComponent<Image>().sprite.name;
    ThiefSkinsPre.GetComponent<Image>().sprite = ThiefSkins.GetComponent<Image>().sprite;
    thief_1.GetComponent<Image>().sprite = ThiefSkins.GetComponent<Image>().sprite;
    thief_1_home.GetComponent<Image>().sprite = ThiefSkins.GetComponent<Image>().sprite;
    PlayerPrefs.SetString("skin", skin_name);
    PreGameScript.GetComponent<PreGame>().customProperties["skin"] = skin_name;
    PhotonNetwork.LocalPlayer.SetCustomProperties(PreGameScript.GetComponent<PreGame>().customProperties);
    PlayerPrefs.Save();

}

public void EquipSkinPre()
{
    string skin_name = ThiefSkinsPre.GetComponent<Image>().sprite.name;
    ThiefSkins.GetComponent<Image>().sprite = ThiefSkinsPre.GetComponent<Image>().sprite;
    thief_1.GetComponent<Image>().sprite = ThiefSkinsPre.GetComponent<Image>().sprite;
    thief_1_home.GetComponent<Image>().sprite = ThiefSkinsPre.GetComponent<Image>().sprite;
    PlayerPrefs.SetString("skin", skin_name);
    PreGameScript.GetComponent<PreGame>().customProperties["skin"] = skin_name;
    PhotonNetwork.LocalPlayer.SetCustomProperties(PreGameScript.GetComponent<PreGame>().customProperties);
    PlayerPrefs.Save();

}

public void ControlSkins()
{
    Debug.Log("SKIN CONTROLLER");

    foreach (Transform child in SkinPanelContent.transform)
    {
        if (!child.name.Equals("NoSkin"))
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    foreach (Transform child in SkinPanelContentPre.transform)
    {
        if (!child.name.Equals("NoSkin"))
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    DB_Controller.GetComponent<DB_Controller>().GetSkinList(PhotonNetwork.NickName);
    foreach (KeyValuePair<string, bool> kvp in PlayerSkins)
    {
        if (kvp.Key == "red" & kvp.Value)
        {
            GameObject obj = (GameObject)Instantiate(SkinIconPrefab, SkinPanelContent.transform);
            GameObject obj1 = (GameObject)Instantiate(SkinIconPrefab, SkinPanelContentPre.transform);

            obj.GetComponent<SelectSkinButton>().tabGroup = skinGroup;
            obj.transform.GetChild(0).GetComponent<Image>().sprite = red;
            obj1.GetComponent<SelectSkinButton>().tabGroup = skinGroupPre;
            obj1.transform.GetChild(0).GetComponent<Image>().sprite = red;
        }
        else if (kvp.Key == "white" & kvp.Value)
        {
            GameObject obj = (GameObject)Instantiate(SkinIconPrefab, SkinPanelContent.transform);
            GameObject obj1 = (GameObject)Instantiate(SkinIconPrefab, SkinPanelContentPre.transform);

            obj.GetComponent<SelectSkinButton>().tabGroup = skinGroup;
            obj.transform.GetChild(0).GetComponent<Image>().sprite = white;
            obj1.GetComponent<SelectSkinButton>().tabGroup = skinGroupPre;
            obj1.transform.GetChild(0).GetComponent<Image>().sprite = white;
        }
        else if (kvp.Key == "radioactive" & kvp.Value)
        {
            GameObject obj = (GameObject)Instantiate(SkinIconPrefab, SkinPanelContent.transform);
            GameObject obj1 = (GameObject)Instantiate(SkinIconPrefab, SkinPanelContentPre.transform);

            obj.GetComponent<SelectSkinButton>().tabGroup = skinGroup;
            obj.transform.GetChild(0).GetComponent<Image>().sprite = radioactive;
            obj1.GetComponent<SelectSkinButton>().tabGroup = skinGroupPre;
            obj1.transform.GetChild(0).GetComponent<Image>().sprite = radioactive;
        }
        else if (kvp.Key == "tuxedo" & kvp.Value)
        {
            Debug.Log("TUXEDO SKIN TRUE");
            GameObject obj = (GameObject)Instantiate(SkinIconPrefab, SkinPanelContent.transform);
            GameObject obj1 = (GameObject)Instantiate(SkinIconPrefab, SkinPanelContentPre.transform);

            obj.GetComponent<SelectSkinButton>().tabGroup = skinGroup;
            obj.transform.GetChild(0).GetComponent<Image>().sprite = tuxedo;
            obj1.GetComponent<SelectSkinButton>().tabGroup = skinGroupPre;
            obj1.transform.GetChild(0).GetComponent<Image>().sprite = tuxedo;
        }
            else if (kvp.Key == "pumpkin" & kvp.Value)
            {
                Debug.Log("PUMPKIN SKIN TRUE");
                GameObject obj = (GameObject)Instantiate(SkinIconPrefab, SkinPanelContent.transform);
                GameObject obj1 = (GameObject)Instantiate(SkinIconPrefab, SkinPanelContentPre.transform);

                obj.GetComponent<SelectSkinButton>().tabGroup = skinGroup;
                obj.transform.GetChild(0).GetComponent<Image>().sprite = pumpkin;
                obj1.GetComponent<SelectSkinButton>().tabGroup = skinGroupPre;
                obj1.transform.GetChild(0).GetComponent<Image>().sprite = pumpkin;
            }

        }

}

public void EnableUpgradeMenu()
{
    btnHome.interactable = true;
    btnUpgrades.interactable = false;
    btnMic.interactable = true;
    btnRooms.interactable = true;
    FriendsMenu.SetActive(false);
    AddFriendStatus.SetActive(false);
    //Home_Home.SetActive(false);
    NewHome.SetActive(false);

    //InventoryMenu.SetActive(false);
    NewLobbyMenu.SetActive(false);
    MicMenu.SetActive(false);

    GetInventory();
    UpgradeMenu.SetActive(true);
}


public void EnableRoomMenu()
{
    btnHome.interactable = true;
    if(!IsGuest) {
        btnUpgrades.interactable = true;
    }
    btnMic.interactable = true;
    btnRooms.interactable = false;
    FriendsMenu.SetActive(false);
    MicMenu.SetActive(false);
    MicCheck = false;
    AddFriendStatus.SetActive(false);
    NewHome.SetActive(false);

    //Home_Home.SetActive(false);
    UpgradeMenu.SetActive(false);

    //InventoryMenu.SetActive(true);
    LobbyScript.SetActive(true);
    NewLobbyMenu.SetActive(true);
    //GetInventory();

}

IEnumerator UpdateFriendList()
{
    for (; ; )
    {
        if (PhotonNetwork.IsConnectedAndReady && FriendList != null)
        {
            if (FriendList.Length == 1 & FriendList[0] == "")
            {
            }
            else
            {
                PhotonNetwork.FindFriends(FriendList);
            }

        }
        yield return new WaitForSeconds(1f);

    }
}

public void GetFriends(string username, int type)
{
    DB_Controller.GetComponent<DB_Controller>().GetFriends(username,type);
}

public void AddFriend()
{
    Debug.Log("ADD FRIEND INPUT: " +AddFriendInput.text);
    DB_Controller.GetComponent<DB_Controller>().CheckIfExists(PhotonNetwork.NickName, AddFriendInput.text);

}

public void ChangeAddFriendInput()
{
    AddFriendStatus.SetActive(false);
}

public void PlayButton()
{
    LobbyScript.SetActive(true);
    LobbyMenu.SetActive(true);
}

// UPGRADES

public void RemoveUpgrade(List<string> upgrades)
{
    DB_Controller.GetComponent<DB_Controller>().RemoveUpgrade(PhotonNetwork.NickName, upgrades);
}

public void BuyUpgradeSpeedBoots(int cost)
{
    //int updatedCost = int.Parse(speed_boots_cost.text);
    if (PlayerBalance >= cost)
    {
        PlayerBalance = PlayerBalance - cost;
        DB_Controller.GetComponent<DB_Controller>().EditCoinBalance(PhotonNetwork.NickName, PlayerBalance, 10);
        DB_Controller.GetComponent<DB_Controller>().AddUpgrade(PhotonNetwork.NickName, "speed_boots");

    }
}

public void BuyUpgradeShield()
{
    if (PlayerBalance >= 2000)
    {
        PlayerBalance = PlayerBalance - 2000;
        DB_Controller.GetComponent<DB_Controller>().EditCoinBalance(PhotonNetwork.NickName, PlayerBalance, 10);
        DB_Controller.GetComponent<DB_Controller>().AddUpgrade(PhotonNetwork.NickName, "shield");

    }
}

public void BuyUpgradeVision(int cost)
{
    //int updatedCost = int.Parse(vision_cost.text);
    if (PlayerBalance >= cost)
    {
        PlayerBalance = PlayerBalance - cost;
        DB_Controller.GetComponent<DB_Controller>().EditCoinBalance(PhotonNetwork.NickName, PlayerBalance, 10);
        DB_Controller.GetComponent<DB_Controller>().AddUpgrade(PhotonNetwork.NickName, "vision");

    }
}

public void BuyUpgradeNinja(int cost)
{
    //int updatedCost = int.Parse(vision_cost.text);
    if (PlayerBalance >= cost)
    {
        PlayerBalance = PlayerBalance - cost;
        DB_Controller.GetComponent<DB_Controller>().EditCoinBalance(PhotonNetwork.NickName, PlayerBalance, 10);
        DB_Controller.GetComponent<DB_Controller>().AddUpgrade(PhotonNetwork.NickName, "ninja");

    }
}


public void BuyUpgradeSelfRevive()
{
    if (PlayerBalance >= 5000)
    {
        PlayerBalance = PlayerBalance - 5000;
        DB_Controller.GetComponent<DB_Controller>().EditCoinBalance(PhotonNetwork.NickName, PlayerBalance, 10);
        DB_Controller.GetComponent<DB_Controller>().AddUpgrade(PhotonNetwork.NickName, "self_revive");

    }
}

public void BuyUpgradeFastHands(int cost)
{
    //int updatedCost = int.Parse(fast_hands_cost.text);
    if (PlayerBalance >= cost)
    {
        PlayerBalance = PlayerBalance - cost;
        DB_Controller.GetComponent<DB_Controller>().EditCoinBalance(PhotonNetwork.NickName, PlayerBalance, 10);
        DB_Controller.GetComponent<DB_Controller>().AddUpgrade(PhotonNetwork.NickName, "fast_hands");
    }
}

public void GetInventory()
{
    UnlockPanel.SetActive(true);
    UnlockPanelPre.SetActive(true);

    Debug.Log("GetInventory");
    List<string> keys = new List<string>(PlayerInventory.Keys);
    foreach (string key in keys)
    {
        PlayerInventory[key] = 0;
    }
    if (!IsGuest) {
        DB_Controller.GetComponent<DB_Controller>().GetUpgradeList(PhotonNetwork.NickName);
    }

}

public void SetOwnedStatus()
{
    foreach (KeyValuePair<string, int> kvp in PlayerInventory)
    {
        switch (kvp.Key)
        {
            case "speed_boots":
                if (kvp.Value == 0)
                {
                    speed_boots_page.transform.GetChild(1).gameObject.SetActive(true);
                    speed_boots_page.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "2000";
                    speed_boots_page.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    speed_boots_page.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeSpeedBoots(2000));
                    speed_boots_page.transform.GetChild(2).gameObject.SetActive(false);
                    speed_boots_page.transform.GetChild(3).gameObject.SetActive(false);

                    speed_boots_page_pre.transform.GetChild(1).gameObject.SetActive(true);
                    speed_boots_page_pre.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "2000";
                    speed_boots_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    speed_boots_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeSpeedBoots(2000));
                    speed_boots_page_pre.transform.GetChild(2).gameObject.SetActive(false);
                    speed_boots_page_pre.transform.GetChild(3).gameObject.SetActive(false);
                }
                if (kvp.Value == 1)
                {
                    speed_boots_page.transform.GetChild(1).gameObject.SetActive(false);
                    speed_boots_page.transform.GetChild(2).gameObject.SetActive(true);
                    speed_boots_page.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "5000";
                    speed_boots_page.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    speed_boots_page.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeSpeedBoots(5000));
                    speed_boots_page.transform.GetChild(3).gameObject.SetActive(false);

                    speed_boots_page_pre.transform.GetChild(1).gameObject.SetActive(false);
                    speed_boots_page_pre.transform.GetChild(2).gameObject.SetActive(true);
                    speed_boots_page_pre.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "5000";
                    speed_boots_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    speed_boots_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeSpeedBoots(5000));
                    speed_boots_page_pre.transform.GetChild(3).gameObject.SetActive(false);
                }
                if (kvp.Value == 2)
                {
                    speed_boots_page.transform.GetChild(1).gameObject.SetActive(false);
                    speed_boots_page.transform.GetChild(2).gameObject.SetActive(false);
                    speed_boots_page.transform.GetChild(3).gameObject.SetActive(true);
                    speed_boots_page.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "20000";
                    speed_boots_page.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    speed_boots_page.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeSpeedBoots(20000));

                    speed_boots_page_pre.transform.GetChild(1).gameObject.SetActive(false);
                    speed_boots_page_pre.transform.GetChild(2).gameObject.SetActive(false);
                    speed_boots_page_pre.transform.GetChild(3).gameObject.SetActive(true);
                    speed_boots_page_pre.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "20000";
                    speed_boots_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    speed_boots_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeSpeedBoots(20000));
                }
                if (kvp.Value == 3)
                {
                    speed_boots_page.transform.GetChild(1).gameObject.SetActive(false);
                    speed_boots_page.transform.GetChild(2).gameObject.SetActive(false);
                    speed_boots_page.transform.GetChild(3).gameObject.SetActive(true);
                    speed_boots_page.transform.GetChild(4).gameObject.SetActive(false);
                    speed_boots_page.transform.GetChild(5).gameObject.SetActive(true);

                    speed_boots_page_pre.transform.GetChild(1).gameObject.SetActive(false);
                    speed_boots_page_pre.transform.GetChild(2).gameObject.SetActive(false);
                    speed_boots_page_pre.transform.GetChild(3).gameObject.SetActive(true);
                    speed_boots_page_pre.transform.GetChild(4).gameObject.SetActive(false);
                    speed_boots_page_pre.transform.GetChild(5).gameObject.SetActive(true);
                    //speed_boots_unlock.gameObject.SetActive(false);
                    //speed_boots_owned.gameObject.SetActive(true);
                }
                break;

            case "shield":
                if (kvp.Value > 0)
                {
                    shield_unlock.gameObject.SetActive(false);
                    shield_owned.gameObject.SetActive(true);
                    shield_unlock_pre.gameObject.SetActive(false);
                    shield_owned_pre.gameObject.SetActive(true);
                }

                break;

            case "vision":

                if (kvp.Value == 0)
                {
                    vision_page.transform.GetChild(1).gameObject.SetActive(true);
                    vision_page.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "2000";
                    vision_page.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    vision_page.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeVision(2000));
                    vision_page.transform.GetChild(2).gameObject.SetActive(false);
                    vision_page.transform.GetChild(3).gameObject.SetActive(false);

                    vision_page_pre.transform.GetChild(1).gameObject.SetActive(true);
                    vision_page_pre.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "2000";
                    vision_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    vision_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeVision(2000));
                    vision_page_pre.transform.GetChild(2).gameObject.SetActive(false);
                    vision_page_pre.transform.GetChild(3).gameObject.SetActive(false);
                }
                if (kvp.Value == 1)
                {
                    vision_page.transform.GetChild(1).gameObject.SetActive(false);
                    vision_page.transform.GetChild(2).gameObject.SetActive(true);
                    vision_page.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "5000";
                    vision_page.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    vision_page.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeVision(5000));
                    vision_page.transform.GetChild(3).gameObject.SetActive(false);

                    vision_page_pre.transform.GetChild(1).gameObject.SetActive(false);
                    vision_page_pre.transform.GetChild(2).gameObject.SetActive(true);
                    vision_page_pre.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "5000";
                    vision_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    vision_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeVision(5000));
                    vision_page_pre.transform.GetChild(3).gameObject.SetActive(false);
                }
                if (kvp.Value == 2)
                {
                    vision_page.transform.GetChild(1).gameObject.SetActive(false);
                    vision_page.transform.GetChild(2).gameObject.SetActive(false);
                    vision_page.transform.GetChild(3).gameObject.SetActive(true);
                    vision_page.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "20000";
                    vision_page.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    vision_page.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeVision(20000));

                    vision_page_pre.transform.GetChild(1).gameObject.SetActive(false);
                    vision_page_pre.transform.GetChild(2).gameObject.SetActive(false);
                    vision_page_pre.transform.GetChild(3).gameObject.SetActive(true);
                    vision_page_pre.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "20000";
                    vision_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    vision_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeVision(20000));
                }
                if (kvp.Value == 3)
                {
                    vision_page.transform.GetChild(1).gameObject.SetActive(false);
                    vision_page.transform.GetChild(2).gameObject.SetActive(false);
                    vision_page.transform.GetChild(3).gameObject.SetActive(true);
                    vision_page.transform.GetChild(4).gameObject.SetActive(false);
                    vision_page.transform.GetChild(5).gameObject.SetActive(true);

                    vision_page_pre.transform.GetChild(1).gameObject.SetActive(false);
                    vision_page_pre.transform.GetChild(2).gameObject.SetActive(false);
                    vision_page_pre.transform.GetChild(3).gameObject.SetActive(true);
                    vision_page_pre.transform.GetChild(4).gameObject.SetActive(false);
                    vision_page_pre.transform.GetChild(5).gameObject.SetActive(true);

                }
                break;

            case "self_revive":
                if (kvp.Value > 0)
                {
                    self_revive_unlock.gameObject.SetActive(false);
                    self_revive_owned.gameObject.SetActive(true);
                    self_revive_unlock_pre.gameObject.SetActive(false);
                    self_revive_owned_pre.gameObject.SetActive(true);
                }

                break;

            case "fast_hands":
                if (kvp.Value == 0)
                {
                    fast_hands_page.transform.GetChild(1).gameObject.SetActive(true);
                    fast_hands_page.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "2000";
                    fast_hands_page.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    fast_hands_page.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeFastHands(2000));
                    fast_hands_page.transform.GetChild(2).gameObject.SetActive(false);
                    fast_hands_page.transform.GetChild(3).gameObject.SetActive(false);

                    fast_hands_page_pre.transform.GetChild(1).gameObject.SetActive(true);
                    fast_hands_page_pre.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "2000";
                    fast_hands_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    fast_hands_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeFastHands(2000));
                    fast_hands_page_pre.transform.GetChild(2).gameObject.SetActive(false);
                    fast_hands_page_pre.transform.GetChild(3).gameObject.SetActive(false);
                }
                if (kvp.Value == 1)
                {
                    fast_hands_page.transform.GetChild(1).gameObject.SetActive(false);
                    fast_hands_page.transform.GetChild(2).gameObject.SetActive(true);
                    fast_hands_page.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "5000";
                    fast_hands_page.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    fast_hands_page.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeFastHands(5000));
                    fast_hands_page.transform.GetChild(3).gameObject.SetActive(false);

                    fast_hands_page_pre.transform.GetChild(1).gameObject.SetActive(false);
                    fast_hands_page_pre.transform.GetChild(2).gameObject.SetActive(true);
                    fast_hands_page_pre.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "5000";
                    fast_hands_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    fast_hands_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeFastHands(5000));
                    fast_hands_page_pre.transform.GetChild(3).gameObject.SetActive(false);
                }
                if (kvp.Value == 2)
                {
                    fast_hands_page.transform.GetChild(1).gameObject.SetActive(false);
                    fast_hands_page.transform.GetChild(2).gameObject.SetActive(false);
                    fast_hands_page.transform.GetChild(3).gameObject.SetActive(true);
                    fast_hands_page.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "20000";
                    fast_hands_page.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    fast_hands_page.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeFastHands(20000));

                    fast_hands_page_pre.transform.GetChild(1).gameObject.SetActive(false);
                    fast_hands_page_pre.transform.GetChild(2).gameObject.SetActive(false);
                    fast_hands_page_pre.transform.GetChild(3).gameObject.SetActive(true);
                    fast_hands_page_pre.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "20000";
                    fast_hands_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    fast_hands_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeFastHands(20000));
                }
                if (kvp.Value == 3)
                {
                    fast_hands_page.transform.GetChild(1).gameObject.SetActive(false);
                    fast_hands_page.transform.GetChild(2).gameObject.SetActive(false);
                    fast_hands_page.transform.GetChild(3).gameObject.SetActive(true);
                    fast_hands_page.transform.GetChild(4).gameObject.SetActive(false);
                    fast_hands_page.transform.GetChild(5).gameObject.SetActive(true);

                    fast_hands_page_pre.transform.GetChild(1).gameObject.SetActive(false);
                    fast_hands_page_pre.transform.GetChild(2).gameObject.SetActive(false);
                    fast_hands_page_pre.transform.GetChild(3).gameObject.SetActive(true);
                    fast_hands_page_pre.transform.GetChild(4).gameObject.SetActive(false);
                    fast_hands_page_pre.transform.GetChild(5).gameObject.SetActive(true);

                }
                break;

            case "ninja":
                if (kvp.Value == 0)
                {
                    ninja_page.transform.GetChild(1).gameObject.SetActive(true);
                    ninja_page.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "2500";
                    ninja_page.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    ninja_page.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeNinja(2000));
                    ninja_page.transform.GetChild(2).gameObject.SetActive(false);
                    ninja_page.transform.GetChild(3).gameObject.SetActive(false);

                    ninja_page_pre.transform.GetChild(1).gameObject.SetActive(true);
                    ninja_page_pre.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "2000";
                    ninja_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    ninja_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeNinja(2000));
                    ninja_page_pre.transform.GetChild(2).gameObject.SetActive(false);
                    ninja_page_pre.transform.GetChild(3).gameObject.SetActive(false);
                }
                if (kvp.Value == 1)
                {
                    ninja_page.transform.GetChild(1).gameObject.SetActive(false);
                    ninja_page.transform.GetChild(2).gameObject.SetActive(true);
                    ninja_page.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "5000";
                    ninja_page.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    ninja_page.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeNinja(5000));
                    ninja_page.transform.GetChild(3).gameObject.SetActive(false);

                    ninja_page_pre.transform.GetChild(1).gameObject.SetActive(false);
                    ninja_page_pre.transform.GetChild(2).gameObject.SetActive(true);
                    ninja_page_pre.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "5000";
                    ninja_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    ninja_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeNinja(5000));
                    ninja_page_pre.transform.GetChild(3).gameObject.SetActive(false);
                }
                if (kvp.Value == 2)
                {
                    ninja_page.transform.GetChild(1).gameObject.SetActive(false);
                    ninja_page.transform.GetChild(2).gameObject.SetActive(false);
                    ninja_page.transform.GetChild(3).gameObject.SetActive(true);
                    ninja_page.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "20000";
                    ninja_page.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    ninja_page.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeNinja(20000));

                    ninja_page_pre.transform.GetChild(1).gameObject.SetActive(false);
                    ninja_page_pre.transform.GetChild(2).gameObject.SetActive(false);
                    ninja_page_pre.transform.GetChild(3).gameObject.SetActive(true);
                    ninja_page_pre.transform.GetChild(4).transform.GetChild(1).GetComponent<TMP_Text>().text = "20000";
                    ninja_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.RemoveAllListeners();
                    ninja_page_pre.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => BuyUpgradeNinja(20000));
                }
                if (kvp.Value == 3)
                {
                    ninja_page.transform.GetChild(1).gameObject.SetActive(false);
                    ninja_page.transform.GetChild(2).gameObject.SetActive(false);
                    ninja_page.transform.GetChild(3).gameObject.SetActive(true);
                    ninja_page.transform.GetChild(4).gameObject.SetActive(false);
                    ninja_page.transform.GetChild(5).gameObject.SetActive(true);

                    ninja_page_pre.transform.GetChild(1).gameObject.SetActive(false);
                    ninja_page_pre.transform.GetChild(2).gameObject.SetActive(false);
                    ninja_page_pre.transform.GetChild(3).gameObject.SetActive(true);
                    ninja_page_pre.transform.GetChild(4).gameObject.SetActive(false);
                    ninja_page_pre.transform.GetChild(5).gameObject.SetActive(true);

                }
                break;

            default:
                break;
        }
    }
    UnlockPanel.SetActive(false);
    UnlockPanelPre.SetActive(false);
}



// PRE GAME MENU



public void LeaveRoom()
{
    PhotonNetwork.LeaveRoom();
    EnableRoomMenu();
}
public void EnableHomeScreen()
{
    if (!bg.animating) {
        LobbyScreen.SetActive(false);
        UpgradeScreen.SetActive(false);
        btnPreLobby.interactable = true;
        btnPreMap.interactable= true;
        if (!IsGuest) {
            btnPreUpgrade.interactable = true;
        }
        btnPreHome.interactable = false;
        if (MapScreen.activeInHierarchy) {
            MapScreen.SetActive(false);
            bg.unZoom(PreGameHome);
        } else {
            PreGameHome.SetActive(true);
        }
    }

}

public void EnableUpgradeScreen()
{
    if (!bg.animating)
    {
        PreGameHome.SetActive(false);
        LobbyScreen.SetActive(false);
        btnPreHome.interactable = true;
        btnPreLobby.interactable = true;
        btnPreMap.interactable = true;
        btnPreUpgrade.interactable = false;

        if (MapScreen.activeInHierarchy)
        {
            MapScreen.SetActive(false);

            bg.unZoom(UpgradeScreen);
        }
        else
        {
            GetInventory();
            UpgradeScreen.SetActive(true);

        }
        //ContentLobby.GetComponent<PopulateGridLobby>().OnRefresh();
    }

}

// zoom in (make sure to call unzoom if map is active when navigating away)
public void EnableMapScreen()
{
    mapOutline.GetComponent<Image>().color = new Color(255,255,0,0);
    LobbyScreen.SetActive(false);
    PreGameHome.SetActive(false);
    UpgradeScreen.SetActive(false);
    btnPreHome.interactable = true;
    btnPreLobby.interactable = true;
    btnPreMap.interactable = false;
    if (!IsGuest) {
        btnPreUpgrade.interactable = true;
    }
    bg.zoom();
    MapIndicator.SetActive(false);


    // MapScreen.SetActive(true);
}
public void OnMouseEnterMap() {
    if (PreGameHome.activeInHierarchy) {
        mapOutline.GetComponent<Image>().color = new Color(255,255,0,200);
    }
}

public void OnMouseLeaveMap() {
    mapOutline.GetComponent<Image>().color = new Color(255,255,0,0);
}

public void ToggleNotes() {
    Notes.SetActive(!Notes.activeInHierarchy);
}


public void EnableLobbyScreen()
{
    if (!bg.animating) {
        PreGameHome.SetActive(false);
        UpgradeScreen.SetActive(false);
        btnPreHome.interactable = true;
        btnPreLobby.interactable = false;
        btnPreMap.interactable = true;
        if (!IsGuest) {
            btnPreUpgrade.interactable = true;
        }

        if (MapScreen.activeInHierarchy) {
            MapScreen.SetActive(false);
            bg.unZoom(LobbyScreen);
        } else {
            LobbyScreen.SetActive(true);
        }
        ContentLobby.GetComponent<PopulateGridLobby>().OnRefresh();
    }
}




public void ThiefController()
{
    PreGameScript.GetComponent<PreGame>().SetPlayerSkins();
    if (PhotonNetwork.PlayerListOthers.Length == 0)
    {
        thief_2.SetActive(false);
        thief_3.SetActive(false);
        thief_4.SetActive(false);

    }
    if (PhotonNetwork.PlayerListOthers.Length == 1)
    {
        thief_2.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerListOthers[0].NickName;
        thief_2.SetActive(true);
        thief_3.SetActive(false);
        thief_4.SetActive(false);

    }
    else if (PhotonNetwork.PlayerListOthers.Length == 2)
    {
        thief_2.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerListOthers[0].NickName;
        thief_3.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerListOthers[1].NickName;
        thief_2.SetActive(true);
        thief_3.SetActive(true);
        thief_4.SetActive(false);
    }
    else if (PhotonNetwork.PlayerListOthers.Length == 3)
    {
        thief_2.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerListOthers[0].NickName;
        thief_3.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerListOthers[1].NickName;
        thief_4.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerListOthers[2].NickName;
        thief_2.SetActive(true);
        thief_3.SetActive(true);
        thief_4.SetActive(true);
    }
}





    // HELPER FUNCTIONS
public void ChangeUserNameInput()
{
    Status.SetActive(false);
    StatusGuest.SetActive(false);
}
public void SetUserName()
{
    StartCoroutine(ShowLoadingScreenLogIn(0));
    ExistingUserMenu.SetActive(false);
    GuestMenu.SetActive(false);
    PhotonNetwork.NickName = UsernameLoginInput.text;
    //menu_script.SetActive(true);
    authValues = new AuthenticationValues();
    authValues.UserId = UsernameLoginInput.text;
    PhotonNetwork.AuthValues = authValues;
    Connect();
    thief_1.GetComponentInChildren<Text>().text = PhotonNetwork.NickName;
    thief_1_home.GetComponentInChildren<Text>().text = PhotonNetwork.NickName;

    Debug.Log("NICKNAME: " + PhotonNetwork.NickName);

    if (IsGuest)
    {
        PlayerPrefs.SetInt("IsGuest", 1);
        FriendsMenuButton.interactable = false;
        UpgradesMenuButton.interactable = false;
        CosmeticStoreButton.interactable = false;
        BalanceInfoHome.SetActive(false);
        BalanceInfoPre.SetActive(false);
        BalanceInfoLobby.SetActive(false);
        FriendPanel.SetActive(false);
        ColorBlock cb = UpgradeButtonFromLobby.GetComponent<Button>().colors;
        cb.disabledColor = new Color32(130, 130, 130, 200);
        UpgradeButtonFromLobby.GetComponent<Button>().colors = cb;
        UpgradeButtonFromPreGame.GetComponent<Button>().colors = cb;
        UpgradeButtonFromLobby.GetComponent<Button>().interactable = false;
        ChooseUpgradesButton.GetComponent<Button>().interactable = false;
        UpgradeButtonFromPreGame.GetComponent<Button>().interactable = false;

    }
    else
    {
        PlayerPrefs.SetInt("IsGuest", 0);

            // ADD NEW UPGRADES HERE
            PlayerInventory.Add("speed_boots", 0);
        PlayerInventory.Add("shield", 0);
        PlayerInventory.Add("vision", 0);
        PlayerInventory.Add("self_revive", 0);
        PlayerInventory.Add("fast_hands", 0);
        PlayerInventory.Add("ninja", 0);
        GetInventory();

        //ADD NEW SKINS HERE
        PlayerSkins.Add("red", false);
        PlayerSkins.Add("radioactive", false);
        PlayerSkins.Add("white", false);
        PlayerSkins.Add("tuxedo", false);
        PlayerSkins.Add("pumpkin", false);


        DB_Controller.GetComponent<DB_Controller>().GetSkinList(PhotonNetwork.NickName);

    }
    if (IsGuest)
        {
            PlayerSkins["red"] = true;
            PlayerSkins["radioactive"] = true;
            PlayerSkins["white"] = true;

        }

    ContentFriendsNew.GetComponent<PopulateGridFriends>().OnRefresh();


}
private IEnumerator ShowLoadingScreenLogIn(int type)
{
    Debug.Log("Showing loading screen");
    LoadingScreenLogIn.SetActive(true);
    yield return new WaitForSeconds(3);
    if (type == 0)
    {
        HomeMenu.SetActive(true);
    }
    else if (type == 1)
    {
        PreGameMenu.SetActive(true);
        ThiefController();

    }

    LoadingScreenLogIn.SetActive(false);


}
public void ReJoinAfterLeave()
{
    StartCoroutine(ShowLoadingScreenLogIn(0));
    LoadingScreenLogIn.transform.GetChild(3).GetComponent<Text>().text = "Loading...";
    StartMenu.SetActive(false);
    //RejoinWaitPanel.SetActive(true);
    if (PlayerPrefs.GetInt("IsGuest") == 1)
        {
            IsGuest = true;
        }
    PhotonNetwork.JoinLobby(TypedLobby.Default);


        thief_1.GetComponentInChildren<Text>().text = PhotonNetwork.NickName;
    thief_1_home.GetComponentInChildren<Text>().text = PhotonNetwork.NickName;

    if (IsGuest)
    {
        FriendsMenuButton.interactable = false;
        UpgradesMenuButton.interactable = false;
        CosmeticStoreButton.interactable = false;

        BalanceInfoHome.SetActive(false);
        BalanceInfoPre.SetActive(false);
        BalanceInfoLobby.SetActive(false);
        FriendPanel.SetActive(false);
        ColorBlock cb = UpgradeButtonFromLobby.GetComponent<Button>().colors;
        cb.disabledColor = new Color32(130, 130, 130, 200);
        UpgradeButtonFromLobby.GetComponent<Button>().colors = cb;
        UpgradeButtonFromPreGame.GetComponent<Button>().colors = cb;
        UpgradeButtonFromLobby.GetComponent<Button>().interactable = false;
        ChooseUpgradesButton.GetComponent<Button>().interactable = false;
        UpgradeButtonFromPreGame.GetComponent<Button>().interactable = false;

    }
    else
        {
            DB_Controller.GetComponent<DB_Controller>().GetCoinBalance(PhotonNetwork.NickName);

            // ADD NEW UPGRADES HERE
            PlayerInventory.Add("speed_boots", 0);
            PlayerInventory.Add("shield", 0);
            PlayerInventory.Add("vision", 0);
            PlayerInventory.Add("self_revive", 0);
            PlayerInventory.Add("fast_hands", 0);
            PlayerInventory.Add("ninja", 0);
            GetInventory();

            //ADD NEW SKINS HERE
            PlayerSkins.Add("red", false);
            PlayerSkins.Add("radioactive", false);
            PlayerSkins.Add("white", false);
            PlayerSkins.Add("tuxedo", false);
            PlayerSkins.Add("pumpkin", false);


            DB_Controller.GetComponent<DB_Controller>().GetSkinList(PhotonNetwork.NickName);

        }
        if (IsGuest)
        {
            PlayerSkins["red"] = true;
            PlayerSkins["radioactive"] = true;
            PlayerSkins["white"] = true;

        }

        ContentFriendsNew.GetComponent<PopulateGridFriends>().OnRefresh();
}

public void ReJoinAfterPlayAgain()
{
    StartCoroutine(ShowLoadingScreenLogIn(1));
    LoadingScreenLogIn.transform.GetChild(3).GetComponent<Text>().text = "Loading...";
    StartMenu.SetActive(false);
    //PhotonNetwork.JoinLobby(TypedLobby.Default);


    thief_1.GetComponentInChildren<Text>().text = PhotonNetwork.NickName;
    thief_1_home.GetComponentInChildren<Text>().text = PhotonNetwork.NickName;

    if (PlayerPrefs.GetInt("IsGuest") == 1)
    {
        IsGuest = true;
    }

    if (IsGuest)
    {
        FriendsMenuButton.interactable = false;
        UpgradesMenuButton.interactable = false;
        CosmeticStoreButton.interactable = false;
        BalanceInfoHome.SetActive(false);
        BalanceInfoPre.SetActive(false);
        BalanceInfoLobby.SetActive(false);
        FriendPanel.SetActive(false);
        ColorBlock cb = UpgradeButtonFromLobby.GetComponent<Button>().colors;
        cb.disabledColor = new Color32(130, 130, 130, 200);
        UpgradeButtonFromLobby.GetComponent<Button>().colors = cb;
        UpgradeButtonFromPreGame.GetComponent<Button>().colors = cb;
        UpgradeButtonFromLobby.GetComponent<Button>().interactable = false;
        ChooseUpgradesButton.GetComponent<Button>().interactable = false;
        UpgradeButtonFromPreGame.GetComponent<Button>().interactable = false;

    }
    else
        {
            DB_Controller.GetComponent<DB_Controller>().GetCoinBalance(PhotonNetwork.NickName);

            // ADD NEW UPGRADES HERE
            PlayerInventory.Add("speed_boots", 0);
            PlayerInventory.Add("shield", 0);
            PlayerInventory.Add("vision", 0);
            PlayerInventory.Add("self_revive", 0);
            PlayerInventory.Add("fast_hands", 0);
            PlayerInventory.Add("ninja", 0);
            GetInventory();

            //ADD NEW SKINS HERE
            PlayerSkins.Add("red", false);
            PlayerSkins.Add("radioactive", false);
            PlayerSkins.Add("white", false);
            PlayerSkins.Add("tuxedo", false);
            PlayerSkins.Add("pumpkin", false);


            DB_Controller.GetComponent<DB_Controller>().GetSkinList(PhotonNetwork.NickName);

        }
        if (IsGuest)
        {
            PlayerSkins["red"] = true;
            PlayerSkins["radioactive"] = true;
            PlayerSkins["white"] = true;

        }

        ContentFriendsNew.GetComponent<PopulateGridFriends>().OnRefresh();
}



public void BackToUserManager()
{
    ExistingUserMenu.SetActive(false);
    NewUserMenu.SetActive(false);
    GuestMenu.SetActive(false);
    UserManagerMenu.SetActive(true);
    UsernameShort.SetActive(false);
    PasswordError.SetActive(false);
}


public void JoinNewRooom()
{
    RoomOptions roomOptions = new RoomOptions();
    roomOptions.IsOpen = true;
    roomOptions.IsVisible = false;
    roomOptions.MaxPlayers = (byte)4;
    roomOptions.PublishUserId = true;
    if (!customProperties.ContainsKey("num_ready"))
    {
        customProperties.Add("num_ready", 0);
    }
    roomOptions.CustomRoomProperties = customProperties;
    PhotonNetwork.CreateRoom(PhotonNetwork.NickName, roomOptions, TypedLobby.Default);

}


// START MENU
public void StartGame()
{
    StartMenu.SetActive(false);
    UserManagerMenu.SetActive(true);
}

public void QuitGame()
{
    Application.Quit();
}

public void BackToHome()
{
    CreditsMenu.SetActive(false);
    StartMenu.SetActive(true);
}

public void OpenCredits()
{
    CreditsMenu.SetActive(true);
    StartMenu.SetActive(false);

}



#region Callbacks
public override void OnDisconnected(DisconnectCause cause)
{
    Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + cause.ToString() + " ServerAddress: " + PhotonNetwork.ServerAddress);
}

public override void OnConnectedToMaster()
{
    Debug.Log("OnConnectedToMaster");
    //After we connected to Master server, join the Lobby
    PhotonNetwork.JoinLobby(TypedLobby.Default);

}
public override void OnJoinRandomFailed(short returnCode, string message)
{
    Debug.Log("OnJoinRandomFailed got called. This can happen if the room is not existing or full or closed.");
    joiningRoom = false;
}

public override void OnCreatedRoom()
{
    Debug.Log("OnCreatedRoom");

}
public override void OnJoinedLobby()
{
    GetFriends(PhotonNetwork.NickName, 1);
    StartCoroutine("UpdateFriendList");
    //GetInventory();
    //ContentFriendsNew.GetComponent<PopulateGridFriends>().OnRefresh();


    //JoinNewRooom();
    PhotonNetwork.AutomaticallySyncScene = true;
    //RejoinWaitPanel.SetActive(false);
}

public override void OnPlayerEnteredRoom(Player player)
{
    Debug.Log("PLAYER ENTERED ROOM");
    ThiefController();
}

public override void OnPlayerLeftRoom(Player player)
{
    ThiefController();
}

public override void OnJoinedRoom()
{
    Debug.Log("OnJoinedRoom");
    NewLobbyMenu.SetActive(false);
    PreGameMenu.SetActive(true);
    RoomNameButton.GetComponentInChildren<Text>().text = "Room: " + PhotonNetwork.CurrentRoom.Name;
    StopCoroutine("UpdateFriendList");
    ThiefController();
    //PhotonNetwork.LoadLevel("PreGameLobby");

}

public override void OnLeftRoom()
{
    PreGameMenu.SetActive(false);
    NewLobbyMenu.SetActive(true);
}
public override void OnJoinRoomFailed(short returnCode, string message)
{
    Debug.Log("OnJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");

}
#endregion
}
