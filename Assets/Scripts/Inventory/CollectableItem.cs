using UnityEngine;

public class CollectableItem : MonoBehaviour
{

    public string itemName;
    public int value;

    private Vector3 initialPos;

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
    }

    void ReturnToInitialPosition() {
        transform.position = initialPos;
    }
}
