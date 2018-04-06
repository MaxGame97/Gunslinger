using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

public class PlayerNetwork : NetworkBehaviour {

    private string playerName;                                          //The players nickname
    [SyncVar] private int kills;
    [SyncVar] private int deaths;
    [SyncVar] private int ping;
    [SerializeField] private float respawnTime = 3f;

    [SerializeField] private GameObject playerObjectPrefab;             //The player object to spawn
    private GameObject playerObject;                                    //Reference to the spawned player object                 
    [SyncVar] bool playerObjectState;                                   //Keeps track of the player objects state. 

    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //Spawn my player object
        StartCoroutine(LateStart(0.1f));
    }

    IEnumerator LateStart(float waitTime) //This allows the network to complete set-up before attempting to spawn player.
    {
        yield return new WaitForSeconds(waitTime);

        //Spawn my player object
        CmdSpawnPlayerObject(GetComponent<NetworkIdentity>());
    }

    public void AssignPlayerName(string _name)
    {
        playerName = _name;
    }



    /// <summary>
    /// COMMANDS ONLY GET EXECUTED ON THE SERVER
    /// </summary>

    [Command]   //Perfom function on the server
    private void CmdSpawnPlayerObject(NetworkIdentity id)
    {
        //Instantiate the player object on this client
        playerObject = Instantiate(playerObjectPrefab);
        playerObject.name = "PlayerObject: " + playerName;
        playerObjectState = true;
        playerObject.GetComponent<PlayerHealth>().owner = this;


        //player object now exists on the server, propogate it to all the clients.
        //Since we also set this client to have authority of it! (tells it who owns it)
        NetworkServer.SpawnWithClientAuthority(playerObject, id.connectionToClient);

        // If we find the scoreboard in the scene, add this player to it.
        if (FindObjectOfType<Scoreboard>())
            FindObjectOfType<Scoreboard>().RpcAddPlayer(GetComponent<NetworkIdentity>());
    }

    [ClientRpc] //tell clients we died
    public void RpcPlayerDied()
    {
        playerObject.SetActive(false);
    }

    [Command]   //tell server we respawned
    private void CmdRespawn()
    {
        Invoke("RpcRespawn", respawnTime);
    }

    [ClientRpc] //tell clients we respawned
    private void RpcRespawn()
    {
        playerObject.SetActive(true);
    }
}
