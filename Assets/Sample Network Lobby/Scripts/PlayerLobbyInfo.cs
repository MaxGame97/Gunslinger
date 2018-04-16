using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerLobbyInfo : NetworkBehaviour
{
    public Text playerNameText;
    public Text playerPingText;

    [SyncVar]
    private string playerName;

    [SyncVar]
    private float playerPing;

    private void Start()
    {
        AssignName("Player Name");
    }

    public void AssignName(string _name)
    {
        playerName = _name;
        playerNameText.text = playerName;
    }

    private void Update()
    {
        //playerPing = NetworkManager.singleton.client.GetRTT();
        playerPingText.text = playerPing.ToString();
    }
}