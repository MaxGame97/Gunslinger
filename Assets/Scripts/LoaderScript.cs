using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoaderScript : MonoBehaviour {

    //NetworkManager Prefab
    [Tooltip("Drag the NetworkManagerPrefab here, make sure it's tagged as NetworkManager")]
    [SerializeField]
    private GameObject networkManager;

    //The only instance of the NetworkManager in the scene is stored here
    private static GameObject instance;

	//finds the networkManager if there is one, if not create one.
	void Awake () {
        instance = GameObject.FindGameObjectWithTag("NetworkManager");
        if (instance == null)
        {
            instance = Instantiate(networkManager);
        }
	}
}
