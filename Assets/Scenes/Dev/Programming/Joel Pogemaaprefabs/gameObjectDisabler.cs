using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class gameObjectDisabler : NetworkBehaviour {

    [SerializeField] private GameObject[] gameObjectsToDisable;

    // Use this for initialization
    void Start()
    {
        if (!isLocalPlayer)
        {
            for (int i = 0; i < gameObjectsToDisable.Length; i++)
            {
                gameObjectsToDisable[i].SetActive(false);
            }
        }
    }
}
