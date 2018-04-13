using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnect : NetworkBehaviour {

    [SerializeField] private Behaviour[] componentsToDisable;   // Assigned components will be disabled

    private GameObject sceneCamera;                             // Defines the scene camera

    // Use this for initialization
    void Start () {
        // If this player is not the local player
		if(!hasAuthority)
        {
            // Disable all assigned components
            for(int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }
        // Else - If this player is the local player
        else
        {
            // Get the scene camera
            sceneCamera = Camera.main.gameObject;
            // If the scene camera exists, disable it
            if(sceneCamera != null)
            {
                sceneCamera.SetActive(false);
            }
        }
	}

    // OnDisable is run when this object is disabled
    void OnDisable()
    {
        // If the scene camera exists, enable it
        if (sceneCamera != null)
        {
            sceneCamera.SetActive(true);
        }
    }

    private void OnEnable()
    {
        // If the scene camera exists, disable it
        if (sceneCamera != null)
        {
            sceneCamera.SetActive(false);
        }
    }
}
