using UnityEngine;
using UnityEngine.Networking;

public class PlayerObjectSpawner : NetworkBehaviour {

    public GameObject SpawnPlayer(NetworkConnection conn, GameObject playerObjectPrefab, string _name) // spawn a new player for the desired connection
    {
        //Instantiate the player object
        GameObject playerObject = Instantiate(playerObjectPrefab);
        playerObject.name = "PlayerObject: " + _name;

        NetworkServer.AddPlayerForConnection(conn, playerObject, 0); // spawn on the clients and set owner
        return playerObject;
    }
}
