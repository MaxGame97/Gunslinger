using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

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
        CmdSpawnPlayerObject(GetComponent<NetworkIdentity>());

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

        //PlayerNetwork[] playersNetwork = new PlayerNetwork[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<NetworkIdentity>().clientAuthorityOwner == myID.connectionToClient) //If this client has authority over this object, then it is our player object
            {
                players[i].name = playerName;                                               // Change the objects name
                playerObject = players[i];                                                  // Set the reference to our playerObject
                players[i].GetComponent<PlayerHealth>().SetOwner(gameObject);               // Set this to be the owner
                players[i].GetComponent<PlayerWeapon>().SetOwner(gameObject);               // Set this to be the owner
                players[i].GetComponent<PlayerObject>().SetPlayerObjectName(playerName);    // Set the name of the playerObject
                initializedSuccessfully = true;                                             // Mark this as a successfull initialization
            }
        }
        //Debug.Log((isServer ? "Server " : "Client ") + (initializedSuccessfully ? "initialized successfully" : "did not initialize successfully"));
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
                CmdRespawn(playerObject);   //Respawn player if we have the authority
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

    /// <summary>
    /// COMMANDs
    /// </summary>

    [Command]   //Perfom function on the server
    private void CmdSpawnPlayerObject(NetworkIdentity _id)
    {

        GameObject _prefab = Resources.Load("Prefabs/Player") as GameObject; 
        //Instantiate the player object on this client
        GameObject go = Instantiate(_prefab, _id.transform.position, Quaternion.identity);
    
        //player object now exists on the server, spawn it on all the clients.
        //We also set this client to have authority of it!
        NetworkServer.SpawnWithClientAuthority(go, _id.connectionToClient);
    }

    [Command]   //tell server we respawned
    private void CmdRespawn(GameObject _player)
    {
        RpcRespawn(_player);    //Respawn players on all clients
    }

    [Command]
    public void CmdPlayerDied(GameObject _playerObjectsOwner, GameObject _playerObject, GameObject _killer)
    {
        RpcPlayerDied(_playerObjectsOwner, _playerObject, _killer);
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
    public void RpcPlayerDied(GameObject _playerObjectsOwner, GameObject _player, GameObject _killer)
    {
        string killersName = "Unknown";
        string playersName = "Unknown";

        if (_killer)
        {
            PlayerNetwork killerNetwork = _killer.GetComponent<PlayerNetwork>();
            killerNetwork.kills++;
            killersName = killerNetwork.Name;
        }

        if(_player)
        {
            PlayerNetwork playerNetwork = _playerObjectsOwner.GetComponent<PlayerNetwork>();
            playersName = playerNetwork.playerName;
            playerNetwork.playerIsDead = true;          // set isDead to true
            playerNetwork.deaths++;                     // Incease players death count
            _player.SetActive(false);   // Disable playerObject
        }

        if (!killfeed)
            killfeed = FindObjectOfType<Killfeed>();    // NOT THE BEST WAY TO HANDLE THIS. FIX?

        if (!killfeed)
            Debug.LogError("No killfeed in scene!");
        else
            killfeed.Spawnfeed(playersName, killersName, false);
    }

    [ClientRpc] //tell clients we respawned
    private void RpcRespawn(GameObject _player)
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

            _player.GetComponent<PlayerHealth>().ResetHealth();    //Reset the players health when respawning
            _player.SetActive(true);
        }

        if (hasAuthority)
            playerIsDead = false;
    }
}
