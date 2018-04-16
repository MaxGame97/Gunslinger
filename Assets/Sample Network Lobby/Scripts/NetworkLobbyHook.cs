using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        //Get references
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        PlayerNetwork player = gamePlayer.GetComponent<PlayerNetwork>();

        //Assign name in script and to gameObject
        player.AssignPlayerName(lobby.playerName);
        gamePlayer.name = "Network: " + lobby.playerName;
    }
}