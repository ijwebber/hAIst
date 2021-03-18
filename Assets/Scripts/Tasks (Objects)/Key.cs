using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private KeyType keyType;
    public enum KeyType {
        Red,
        Green,
        Blue
    }

    public KeyType GetKeyType() {
        return keyType;
    }
}
