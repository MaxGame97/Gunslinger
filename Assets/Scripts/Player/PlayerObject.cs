using UnityEngine.Networking;
using UnityEngine;

public class PlayerObject : NetworkBehaviour {

    [SyncVar (hook = "OnNameChanged")] private string playerObjectName;

    public void SetPlayerObjectName(string _name)   // Update the players name
    {
        playerObjectName = _name;
    }

    void OnNameChanged(string _name)    // If the name has been updated across network, update it locally as well.
    {
        if(hasAuthority)
            gameObject.name = _name;
    }

    private void Update()
    {
        if (gameObject.name != playerObjectName)    // if for any reason the player objects name is not set properly, set it properly
            gameObject.name = playerObjectName;
    }
}
