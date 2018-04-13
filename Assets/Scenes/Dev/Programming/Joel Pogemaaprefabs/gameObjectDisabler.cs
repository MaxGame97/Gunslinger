using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class gameObjectDisabler : NetworkBehaviour {

    //Mainly used to remove the UI from the othr players
    [Tooltip("Place all Game objects that should be unique to each player here")]
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
