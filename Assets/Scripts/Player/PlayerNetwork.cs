using UnityEngine.Networking;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerNetwork : NetworkBehaviour {

    [SyncVar ( hook = "OnPlayerNameChanged")] private string playerName;    //The players nickname
    [SyncVar] private int kills = 0;
    [SyncVar] private int deaths = 0;
    [SyncVar] private int ping = 0;
    [SyncVar] private bool playerIsDead = false;
    [SyncVar] private bool playerIsReady = false;
    [SerializeField] private float respawnTime = 3f;

    [HideInInspector] public string characterName;          // Name of the character we want to use
    [HideInInspector] public GameObject playerObject;       // Reference to the spawned player object
    [HideInInspector] public bool canSpawn = false;

    private Killfeed killfeed;                              // Reference to the killfeed
    private float _timer = 0f;

    #region GETTERS 
    public string Name
    {
        get { return playerName; }
    }

    public int Kills
    {
        get { return kills; }
    }

    public int Deaths
    {
        get { return deaths; }
    }

    public int Ping
    {
        get { return ping; }
    }
    #endregion

    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        Application.targetFrameRate = 144;
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitUntil(() => canSpawn);

        //playerObjectPrefab = FindObjectOfType<SpawnMenu>().characterToUse;

        //Spawn my player object
        CmdSpawnPlayerObject(GetComponent<NetworkIdentity>(), playerName);

        // yield return new WaitForSeconds(0.2f);    //CHANGE? WaitUntil all players have joined instead?

        killfeed = Killfeed.instance;
        if (!killfeed)
            Debug.LogWarning("Could not find a Killfeed object!");

        yield return new WaitForSeconds(0.1f);
        // Initialize the playerObject locally
        InitializePlayerObjects();

        if (playerObject == null)
            Debug.LogError(gameObject.name + " has no playerObject reference!");
        canSpawn = false;
    }

    void InitializePlayerObjects()  // Used to initialize values for this local player
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // Get reference of all playerObjects
        NetworkIdentity myID = GetComponent<NetworkIdentity>();             // Get reference to this objects network identity
        bool initializedSuccessfully = false;                               // set false as default

        if (gameObject.name != ("Network: " + playerName))  // If the gameObjects name is not correct, update it
            gameObject.name = "Network: " + playerName;
    }

    void OnPlayerNameChanged(string _name)  // If the playerName is changed across server, update this locally
    {
        playerName = _name;
    }

    private void Update()
    {
        if (playerIsDead && hasAuthority)   // if the player is dead, increment timer until desired time to spawn
        {               
            if (_timer > respawnTime)
            {
                if (!isServer)
                {
                    CmdRespawn(playerObject);   //Respawn player if we have the authority
                }
                else
                {
                    // Selects a position to spawn on from the list of spawnPoints.
                    Transform spawnPoint = CreateRandomSpawnPoint();
                    RpcRespawn(playerObject, spawnPoint.position, spawnPoint.rotation);
                }
                if(hasAuthority)
                    playerIsDead = false;
                _timer = 0;
            }
            else
                _timer += Time.deltaTime;   // Increment timer
        }
    }

    public void AssignPlayerName(string _name)   // Used to assign the playerName for this instances playerObject
    {
        playerName = _name;
    }

    void SpawnKillfeed(string _killer, bool _headshot)
    {
        if (killfeed)
            killfeed.Spawnfeed(_killer, playerName, _headshot);
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

/// <summary>
/// COMMANDs
/// </summary>

    [Command]   //Perfom function on the server
    private void CmdSpawnPlayerObject(NetworkIdentity _id, string _playerName)
    {

        GameObject _prefab = Resources.Load("Prefabs/Player") as GameObject; 
        //Instantiate the player object on this client
        GameObject go = Instantiate(_prefab, _id.transform.position, Quaternion.identity);
    
        //player object now exists on the server, spawn it on all the clients.
        //We also set this client to have authority of it!
        NetworkServer.SpawnWithClientAuthority(go, _id.connectionToClient);

        RpcSetPlayerObject(go, _playerName);
    }

    [ClientRpc]
    private void RpcSetPlayerObject(GameObject _playerObject, string _playerName)
    {
        if (isLocalPlayer)
        {
            playerObject = _playerObject;                                                 // Set the reference to our playerObject
            playerObject.name = playerName;                                               // Change the objects name
            playerObject.GetComponent<PlayerHealth>().SetOwner(gameObject);               // Set this to be the owner
            playerObject.GetComponent<PlayerWeapon>().SetOwner(gameObject);               // Set this to be the owner
            playerObject.GetComponent<PlayerObject>().SetPlayerObjectName(playerName);    // Set the name of the playerObject
            GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<PauseMenuSript>().SetOwner(playerObject);
        }
        else
        {
            _playerObject.name = _playerName;
        }
            
    }

    [Command]   //tell server we respawned
    private void CmdRespawn(GameObject _player)
    {
        Transform spawnPoint = CreateRandomSpawnPoint();

        RpcRespawn(_player, spawnPoint.position, spawnPoint.rotation);
    }

    [Command]
    public void CmdPlayerDied(GameObject _playerObjectsOwner, GameObject _playerObject, GameObject _killer, bool _headshot)
    {
        RpcPlayerDied(_playerObjectsOwner, _playerObject, _killer, _headshot);
    }

    /// <summary>
    /// CLIENTRPCs
    /// </summary>
    [ClientRpc]
    public void RpcIncreaseKillCount()
    {
        kills++;
    }

    [ClientRpc] //tell clients we died
    public void RpcPlayerDied(GameObject _playerObjectsOwner, GameObject _player, GameObject _killer, bool _headshot)
    {
        string killersName = "Unknown";
        string playersName = "Unknown";

        if (_killer)
        {
            PlayerNetwork killerNetwork = _killer.GetComponent<PlayerNetwork>();
            killerNetwork.kills++;
            killersName = killerNetwork.Name;
        }
        else
            Debug.LogError("No killer reference!");

        if(_player)
        {
            PlayerNetwork playerNetwork = _playerObjectsOwner.GetComponent<PlayerNetwork>();
            playersName = playerNetwork.playerName;
            playerNetwork.playerIsDead = true;          // set isDead to true
            playerNetwork.deaths++;                     // Incease players death count
            _player.SetActive(false);   // Disable playerObject
        }
        else
            Debug.LogError("No player reference!");

        if (!killfeed)
            killfeed = FindObjectOfType<Killfeed>();    // NOT THE BEST WAY TO HANDLE THIS. FIX?

        if (!killfeed)
            Debug.LogError("No killfeed in scene!");
        else
            killfeed.Spawnfeed(playersName, killersName, _headshot);
    }

    [ClientRpc] //tell clients we respawned
    private void RpcRespawn(GameObject _player, Vector3 _spawnPosition, Quaternion _spawnRotation)
    {
        if (GameManager.instance)
        {
            if (deaths >= GameManager.instance.StockAmount)
            {
                //Return if player is out of lives
                GameManager.instance.CmdPlayerDied(gameObject);
                return;
            }
        }


        if (_player)
        {
            //Give player a new position?
            // _player.transform.position = GetSpawnPosition();
            // Sets the new position for the player from the spawn position.
            _player.transform.position = _spawnPosition;
            _player.transform.rotation = _spawnRotation;

            _player.GetComponent<PlayerHealth>().ResetHealth();    //Reset the players health when respawning
            _player.SetActive(true);
        }
        else
        {
            Debug.LogError("NO PLAYER OBJECT REFERENCE");
        }

        //if (hasAuthority)
            //playerIsDead = false;
    }
}
