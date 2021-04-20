using UnityEngine;

public class IntentActions : MonoBehaviour
{    
    public void CarryOutIntent(string topIntent, float score) {
        if (score > 0.3) {
            switch (topIntent)
            {
                case "ShowMap":
                    ShowMap();
                    break;
                case "HideMap":
                    HideMap();
                    break;
                case "EnableLasers":
                    EnableLasers();
                    break;
                case "DisableLasers":
                    DisableLasers();
                    break;
                default:
                    Unsure();
                    break;
            }
        } else {
            Unsure();
        }
    }

    public void ShowMap() {
        Debug.Log("I need to show you the map!");
    }

    public void HideMap() {
        Debug.Log("I need to hide the map from you!");
    }

    public void EnableLasers() {
        Debug.Log("I have switched on the lasers!");
    }

    public void DisableLasers() {
        Debug.Log("I will turn off the lasers!");
    }

    public void Unsure() {
        Debug.Log("I am unsure as to what to do!");
    }
}
