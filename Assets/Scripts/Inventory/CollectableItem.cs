using UnityEngine;

public class CollectableItem : MonoBehaviour
{

    public string itemName;
    public int value;

    private Transform initialTransform;

    // Start is called before the first frame update
    void Start()
    {
        initialTransform = transform;
    }
}
