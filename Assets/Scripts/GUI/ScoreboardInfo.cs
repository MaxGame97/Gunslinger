using UnityEngine.UI;
using UnityEngine;

public class ScoreboardInfo : MonoBehaviour {

    private PlayerNetwork player;                   // Reference to the player stats we want to display

    private bool isDead = false;
    public bool IsDead { get { return isDead; } }

    [SerializeField] private Text playerName;       // Reference to the UI text object
    [SerializeField] private Text playerKills;      // Reference to the UI text object
    [SerializeField] private Text playerDeaths;     // Reference to the UI text object
    [SerializeField] private Text playerPing;       // Reference to the UI text object

    public string PlayerName { get { return playerName.text; } }
    private int kills;

    public int Kills
    {
        get { return kills; }
    }

    private Scoreboard scoreboard;
    private void Start()
    {
        scoreboard = FindObjectOfType<Scoreboard>();
    }

    public void AssignPlayerNetwork(PlayerNetwork _player)
    {
        player = _player;
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (playerName != null && playerName.text != player.Name)   // Name
            playerName.text = player.Name;

        if (playerKills != null && playerKills.text != player.Kills.ToString()) // Kills 
        {
            playerKills.text = player.Kills.ToString();
            kills = player.Kills;
            scoreboard.UpdateScoreboard(transform);
        }

        if (playerDeaths != null && playerDeaths.text != player.Deaths.ToString())  // Deaths
        {
            playerDeaths.text = player.Deaths.ToString();
            if (GameManager.instance)
            {
                if (GameManager.instance.gameMode == GameManager.GameMode.FFA)
                {
                    if (player.Deaths > GameManager.instance.StockAmount)
                    {
                        isDead = true;
                    }
                }
            }
        }
            
        if (playerPing != null && playerPing.text != player.Ping.ToString())    // Ping
            playerPing.text = player.Ping.ToString();
    }
}
