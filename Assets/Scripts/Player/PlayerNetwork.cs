using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerNetwork : NetworkBehaviour {

    private string playerName;                                          //The players nickname
    [SyncVar] private int kills;
    [SyncVar] private int deaths;
    [SyncVar] private int ping;
    [SerializeField] private float spawnTime = 3f;

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
        if(playerIsDead)
        {
            if(_timer >= spawnTime)
            {

                if (!isServer) // It doesn't matter which player that dies. Host or Client. It always goes into this IF statement.
                {
                    // Tell the server that we want to respawn.
                    CmdRespawn(playerObject);
                }
                else
                {
                    // Selects a position to spawn on from the list of spawnPoints.
                    Transform spawnPoint = CreateRandomSpawnPoint();
                    RpcRespawn(playerObject, spawnPoint.position, spawnPoint.rotation);
                }
              

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
        // Selects a random position from the available spawnPosition list.
        Transform spawnPoint = CreateRandomSpawnPoint();

        // Spawns the player object prefab on a selected position.
        playerObject = Instantiate(playerObjectPrefab, spawnPoint);
        playerObject.name = "PlayerObject: " + playerName;
        playerObjectState = true;
        playerObject.GetComponent<PlayerHealth>().owner = this;
        // playerObject.GetComponent<PlayerWeapon>().SetOwner(GetComponent<NetworkIdentity>().netId).

        //player object now exists on the server, propogate it to all the clients.
        //Since we also set this client to have authority of it! (tells it who owns it)
        NetworkServer.SpawnWithClientAuthority(playerObject, id.connectionToClient);

        // If we find the scoreboard in the scene, add this player to it.
       // if (FindObjectOfType<Scoreboard>())
       //     FindObjectOfType<Scoreboard>().RpcAddPlayer(GetComponent<NetworkIdentity>());
    }

    [ClientRpc] // Tell clients we died
    public void RpcPlayerDied(GameObject _player)
    {
        _player.SetActive(false);

        // if(hasAuthority)    //If this is the local player, then set the value
        playerIsDead = true;
    }

    [Command]   // Tell server we respawned
    private void CmdRespawn(GameObject _player)
    {
        // Selects a position to spawn on from the list of spawnPoints.
        Transform spawnPoint = CreateRandomSpawnPoint();

        //_player.SetActive(true);
        RpcRespawn(_player, spawnPoint.position, spawnPoint.rotation);
    }

    [ClientRpc] // Tell clients we respawned
    private void RpcRespawn(GameObject _player, Vector3 _spawnPosition, Quaternion _spawnRotation)
    {
        if (_player)
        {
            _player.GetComponent<PlayerHealth>().GainHealth(100);
            // Sets the player gameobject to active. (Respawn). 
            _player.SetActive(true);

            // Sets the new position for the player from the spawn position.
            _player.transform.position = _spawnPosition;
            _player.transform.rotation = _spawnRotation;
        }
        //Give player a new position?
        if (hasAuthority)
        {
            playerIsDead = false;
        }
    }

    // Function to create a random position from the list of available spawns from the NetworkManager/LobbyManager. 
    private Transform CreateRandomSpawnPoint()
    {
        // Gets the list of available startpoints from the NetworkManager/LobbyManager. 
        List<Transform> spawnPoints = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkManager>().startPositions;

        // Select one random to use for spawning from the list.
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        return spawnPoint;
    }
}
