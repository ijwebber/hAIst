public class UpgradeList {
    // perma upgrades
    public int speed_boots;
    public int speed_boots_level;

    public int vision;
    public int vision_level;

    public int fast_hands;
    public int fast_hands_level;

    public int ninja;


    // consumables
    public bool self_revive;
    public bool shield;

    // perma upgrades
    public bool speed_boots_enabled;
    public bool vision_enabled;
    public bool fast_hands_enabled;

    // consumables
    public bool self_revive_enabled;
    public bool shield_enabled;

    public UpgradeList() {
        this.speed_boots = 0;
        this.vision = 0;
        this.fast_hands = 0;
        this.self_revive = false;
        this.shield = false;
    }
    
}