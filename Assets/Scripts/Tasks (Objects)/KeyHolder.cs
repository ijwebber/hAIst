using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHolder : MonoBehaviour
{
    public List<Key.KeyType> keyList;

    private void Awake() {
        keyList = new List<Key.KeyType>();
    }

    public void AddKey(Key.KeyType keyType) {
        keyList.Add(keyType);
    }

    public void RemoveKey(Key.KeyType keyType) {
        keyList.Remove(keyType);
    }

    public bool ContainsKey(Key.KeyType keyType) {
        return keyList.Contains(keyType);
    }


    void OnTriggerEnter(Collider other) {
        Debug.Log("key collided with " + other.gameObject.name);
        Key key = other.GetComponent<Key>();
        if (key != null) {
            AddKey(key.GetKeyType());
            Destroy(key.gameObject);
        }
    }
}
