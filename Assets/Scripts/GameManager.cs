using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

// Script that automatically creates a static gameobject that can hold references to other scripts.

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

    public enum GameMode { FFA, DM };
    [SyncVar (hook = "OnGameModeChanged")] public GameMode gameMode = GameMode.FFA;
    [SyncVar (hook = "OnStockAmountChanged")] private int stockAmount;      // Amount of lives each player starts with in the FFA game mode.
    [SerializeField] private Dropdown stockInput;
    [SerializeField] private Dropdown gameModeInput;
    public bool isHost = false;

    public int StockAmount
    {
        get { return stockAmount; }
    }

    // Updates the stock amount on non-hosts
    void OnStockAmountChanged(int _newValue)
    {
        Debug.Log("stockvalue changed! " + _newValue);
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
                Debug.LogWarning("stockamount invalid!");
                break;
        }
        gameModeInput.Select();
        gameModeInput.RefreshShownValue();
    }

    // Updates the game mode on non-hosts
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

    // Determine if we are hosting the lobby
    public void SetIsHost(bool _value)  
    {
        Debug.Log("Runs!");
        isHost = _value;
        if (!isHost)
        {
            Debug.Log("Is client");
            stockInput.interactable = false;
            gameModeInput.interactable = false;
        }
        else
        {
            Debug.Log("Is host");
            stockInput.interactable = true;
            gameModeInput.interactable = true;
        }
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
        //stockInput.text = stockAmount.ToString();
    }
}
