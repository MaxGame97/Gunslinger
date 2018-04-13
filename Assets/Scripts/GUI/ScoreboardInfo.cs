using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

public class ScoreboardInfo : NetworkBehaviour {

    [SerializeField] private Text playerName;       // Reference to the UI text object
    [SerializeField] private Text playerKills;      // Reference to the UI text object
    [SerializeField] private Text playerDeaths;     // Reference to the UI text object
    [SerializeField] private Text kdRatio;          // Reference to the UI text object
    [SerializeField] private Text playerPing;       // Reference to the UI text object

    private PlayerNetwork player;                   // Reference to the player stats we want to display
    private Scoreboard scoreboard;
    private bool isDead = false;
    private int kills;

    // Getters
    public string PlayerName { get { return playerName.text; } }
    public bool IsDead { get { return isDead; } }
    public int Kills { get { return kills; } }

    private void Start()
    {
        scoreboard = FindObjectOfType<Scoreboard>();    // Get reference to the scoreboard
    }

    // Assigns the player we want to use as a reference
    public void AssignPlayerNetwork(PlayerNetwork _player)
    {
        player = _player;
    }

    private void Update()
    {
        UpdateUI(); // Updates the UI elements
    }

    private void UpdateUI()
    {
        if(!player) // If there is no player reference, return
        {
            Debug.LogError("No player reference!");
            return;
        }

        if (playerName != null && playerName.text != player.Name)   // If the playername ui is not the same as the player name, update it
            playerName.text = player.Name;

        if (playerKills != null && playerKills.text != player.Kills.ToString()) // If the kill amount ui is not the same as the players kills, update it
        {
            playerKills.text = player.Kills.ToString(); // Update the ui element
            kills = player.Kills;                       // Add a kill (to this reference only)
            scoreboard.UpdateScoreboard(transform);     // Update the scoreboard and tell it who updated it
        }

        if (playerDeaths != null && playerDeaths.text != player.Deaths.ToString())  // Deaths
        {
            playerDeaths.text = player.Deaths.ToString();

            //if (GameManager.instance)
            //{
            //    if (GameManager.instance.gameMode == GameManager.GameMode.FFA)
            //        if (player.Deaths > GameManager.instance.StockAmount)
            //            isDead = true;
            //}
        }

        if(kdRatio != null)
        {
            if (player.Deaths > 0 && player.Kills > 0)  // If the player has kills AND deaths
            {
                if (kdRatio.text != (player.Kills / player.Deaths).ToString())  // Update the text element if it is not correct
                    kdRatio.text = ((float)player.Kills / (float)player.Deaths).ToString();
            }
            else if(player.Deaths == 0 && player.Kills == 0)    // If the player has NO kills AND deaths
            {
                kdRatio.text = "1";     // Set K/D as 1 (Default state).
            }
            else if(player.Deaths == 0) // IF the player HAS kills but NOT deaths
            {
                kdRatio.text = player.Kills.ToString(); 
            }
            else // IF the player HAS deaths but NOT kills
            {
                kdRatio.text = "0";
            }
        }
    }
}
