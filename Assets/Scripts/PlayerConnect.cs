using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnect : NetworkBehaviour {

    public Behaviour[] componentsToDisable;
    Camera sceneCamera;

    // Use this for initialization
    void Start () {
		if(!isLocalPlayer)
        {
            for(int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }
        }
        else
        {
            sceneCamera = Camera.main;
            if(sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
        }
	}

    // OnDisable is run when this object is disabled
    void OnDisable()
    {
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }
}
