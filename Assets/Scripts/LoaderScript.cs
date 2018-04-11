using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoaderScript : MonoBehaviour {

    [SerializeField]
    private GameObject networkManager;

    private GameObject instance;

	// Use this for initialization
	void Awake () {
        instance = GameObject.FindGameObjectWithTag("NetworkManager");
        if (instance == null)
        {
            instance = Instantiate(networkManager);
        }
	}
}
