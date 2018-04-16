using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    #region SINGLETON
    // Makes sure there is only one instance of the game manager
    // Also makes this instance accessible from other scripts without a referece.
    // In other words, can be accessed with GameManager.instance!
    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region OnLevelLoaded
    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name != "Lobby Scene")
        {
            // If we're not in the lobby scene, we're probably in the game (change to check for game scene names instead perhaps?
            StartCoroutine(GetPlayersInScene(1f));
        }
    }
    #endregion

    public enum GameMode { FFA, DM };
    [SyncVar(hook = "OnGameModeChanged")] public GameMode gameMode = GameMode.FFA;
    [SerializeField] [SyncVar(hook = "OnStockAmountChanged")] private int stockAmount = 1;      // Amount of lives each player starts with in the FFA game mode.
    [SerializeField] private Dropdown stockInput;
    [SerializeField] private Dropdown gameModeInput;
    [SyncVar] [HideInInspector] public bool gameHasEnded = false;

    private List<PlayerNetwork> players = new List<PlayerNetwork>();
    private List<PlayerNetwork> deadPlayers = new List<PlayerNetwork>();

    private IEnumerator GetPlayersInScene(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        players.AddRange(FindObjectsOfType<PlayerNetwork>());
    }

    public int StockAmount
    {
        get { return stockAmount; }
    }

    private void Update()
    {
        // Lobby Code
        if (SceneManager.GetActiveScene().name == "Lobby Scene") // If this is the lobby scene
        {
            if (isServer && (!stockInput.interactable || !gameModeInput.interactable))    // If we're the SERVER, make sure we CAN adjust game mode and stock amount
            {
                stockInput.interactable = true;
                gameModeInput.interactable = true;
            }
            else if (!isServer && (stockInput.interactable || gameModeInput.interactable))// If we're the CLIENT, make sure we CAN'T adjust game mode and stock amount
            {
                stockInput.interactable = false;
                gameModeInput.interactable = false;
            }
        }

        // Game Code

    }

    [Command]
    public void CmdPlayerDied(GameObject _player)
    {
        if (!deadPlayers.Contains(_player.GetComponent<PlayerNetwork>()))
            deadPlayers.Add(_player.GetComponent<PlayerNetwork>());

        if (deadPlayers.Count == players.Count - 1) // If there is one player alive
        {
            for (int i = 0; i < deadPlayers.Count; i++)
            {
                players.Remove(deadPlayers[i]);
            }
            //Debug.Log("Winner is: " + players[0]);

            if (players[0].playerObject)
                Destroy(players[0].playerObject);
            RpcPostGame();
        }
    }

    [ClientRpc]
    private void RpcPostGame()
    {
        gameHasEnded = true;    // Mark the game as ended
        GameObject scoreboardCanvas = GameObject.Find("Scoreboard - Canvas");
        GameObject healthCanvas = GameObject.Find("Health - Canvas");

        if (healthCanvas)
            healthCanvas.SetActive(false);
        if (scoreboardCanvas)
            scoreboardCanvas.transform.GetChild(0).gameObject.SetActive(true);

        if (players[0].playerObject)
            Destroy(players[0].playerObject);
            

        FindObjectOfType<Scoreboard>().gameObject.SetActive(true);  // Enable the scoreboard
    }

    // --- LOBBY FUNCTIONS --- \\

    // Updates the stock amount on clients
    void OnStockAmountChanged(int _newValue)
    {
        stockAmount = _newValue;
        switch (stockAmount)
        {
            case 1:
                stockInput.value = 0;
                break;
            case 5:
                stockInput.value = 1;
                break;
            case 10:
                stockInput.value = 2;
                break;
            case 20:
                stockInput.value = 3;
                break;
            case 30:
                stockInput.value = 4;
                break;
            case 99:
                stockInput.value = 5;
                break;
            default:
                Debug.LogWarning("invalid stockamount!");
                break;
        }
        gameModeInput.Select();
        gameModeInput.RefreshShownValue();
    }

    // Updates the game mode on clients
    void OnGameModeChanged(GameMode _gameMode)
    {
        switch (_gameMode)
        {
            case GameMode.FFA:
                Debug.Log("Game Mode set to: FFA");
                gameMode = _gameMode;
                // Display FFA window
                break;
            case GameMode.DM:
                Debug.Log("Game Mode set to: DM");
                gameMode = _gameMode;
                // Display DM window
                break;
            default:
                Debug.LogWarning("Unknown Game Mode selected!");
                break;
        }
        gameModeInput.value = (int)gameMode;
        gameModeInput.Select();
        gameModeInput.RefreshShownValue();
    }

    // Sets the game mode
    public void SetGameMode()
    {
        if (isServer)
            gameMode = (GameMode)gameModeInput.value;   // Set the new Game Mode
    }

    // Sets the stock amount
    public void SetStockAmount()
    {
        if (isServer)                    // Only server can change values
        {
            switch (stockInput.value)
            {
                case 0:
                    stockAmount = 1;
                    break;
                case 1:
                    stockAmount = 5;
                    break;
                case 2:
                    stockAmount = 10;
                    break;
                case 3:
                    stockAmount = 20;
                    break;
                case 4:
                    stockAmount = 30;
                    break;
                case 5:
                    stockAmount = 99;
                    break;
                default:
                    Debug.LogWarning("stockInput.value is invalid!");
                    break;
            }
        }
    }
}
