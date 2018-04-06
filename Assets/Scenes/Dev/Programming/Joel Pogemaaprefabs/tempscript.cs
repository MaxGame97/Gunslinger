using UnityEngine;
using UnityEngine.Networking;

public class tempscript: NetworkBehaviour
{

    [SerializeField] private Behaviour[] componentsToDisable;   // Assigned components will be disabled

    [SerializeField] private GameObject[] gameObjectsToDisable;

    private Camera sceneCamera;                                 // Defines the scene camera

    // Use this for initialization
    void Start()
    {
        // If this player is not the local player
        if (!isLocalPlayer)
        {
            // Disable all assigned components
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
            for (int i = 0; i < gameObjectsToDisable.Length; i++)
            {
                gameObjectsToDisable[i].SetActive(false);

            }
        }
        // Else - If this player is the local player
        else
        {
            // Get the scene camera
            sceneCamera = Camera.main;

            // If the scene camera exists, disable it
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
        }
    }

    // OnDisable is run when this object is disabled
    void OnDisable()
    {
        // If the scene camera exists, enable it
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }
}
