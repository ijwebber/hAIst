public class UpgradeList {
    // perma upgrades
    public int speed_boots;
    public int vision;
    public int fast_hands;

    // consumables
    public bool self_revive;
    public bool shield;

    public UpgradeList() {
        this.speed_boots = 0;
        this.vision = 0;
        this.fast_hands = 0;
        this.self_revive = false;
        this.shield = false;
    }
    
}