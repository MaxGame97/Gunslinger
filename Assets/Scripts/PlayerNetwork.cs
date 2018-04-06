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

    [SyncVar] private bool playerIsDead = false;
    private float _timer = 0f;


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


    private void Update()
    {
        if (playerIsDead)
        {
            if (_timer > respawnTime)
            {
                //  CmdRespawn(playerObject);

                if (!isClient)
                    RpcRespawn(playerObject);
                else
                    CmdRespawn(playerObject);

                playerIsDead = false;
                _timer = 0;
            }
            else
            {
                _timer += Time.deltaTime;
            }
        }
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
       // playerObject.GetComponent<PlayerWeapon>().SetOwner(GetComponent<NetworkIdentity>().netId);


        //player object now exists on the server, propogate it to all the clients.
        //Since we also set this client to have authority of it! (tells it who owns it)
        NetworkServer.SpawnWithClientAuthority(playerObject, id.connectionToClient);

        // If we find the scoreboard in the scene, add this player to it.
       // if (FindObjectOfType<Scoreboard>())
       //     FindObjectOfType<Scoreboard>().RpcAddPlayer(GetComponent<NetworkIdentity>());
    }

    [ClientRpc] //tell clients we died
    public void RpcPlayerDied(GameObject _player)
    {
        _player.SetActive(false);

       // if(hasAuthority)    //If this is the local player, then set the value
            playerIsDead = true;
    }

    [Command]   //tell server we respawned
    private void CmdRespawn(GameObject _player)
    {
        _player.GetComponent<PlayerHealth>().RpcGainHealth(100);
        //_player.SetActive(true);
        RpcRespawn(_player);
    }

    [ClientRpc] //tell clients we respawned
    private void RpcRespawn(GameObject _player)
    {
            _player.SetActive(true);
        //Give player a new position?
        if (hasAuthority)
        {
            playerIsDead = false;
        }
    }
}
