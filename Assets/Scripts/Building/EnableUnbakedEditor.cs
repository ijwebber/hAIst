using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableUnbakedEditor : MonoBehaviour
{
    public Light unbaked;
    void Start()
    {
        #if UNITY_EDITOR
        unbaked.gameObject.SetActive(true);
        #endif
    }

}
