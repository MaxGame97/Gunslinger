using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script that automatically creates a static gameobject that can hold references to other scripts.

public class GameManager : MonoBehaviour
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
    }
#endregion

    private List<PlayerNetwork> players = new List<PlayerNetwork>();        //A list to hold all players on the server.


    public void AddPlayer(PlayerNetwork _player)    //Add _player to the list if it doesn't already contain that player
    {
        if (!players.Contains(_player))
            players.Add(_player);
        else
            Debug.LogWarning("Player list already contains " + _player.name);
    }

    public void RemovePlayer(PlayerNetwork _player) //Remove _player from the list if the player exists in the list
    {
        if (players.Contains(_player))
            players.Remove(_player);
        else
            Debug.LogWarning("Player list does not contain " + _player.name);
    }

    public PlayerNetwork[] GetPlayers() //Return the list of players as an array.
    {
        if (players.Count > 0)
            return players.ToArray();
        else
            Debug.LogWarning("Player list is empty!");
        return null;
    }


    
    // Will probably remove.
    /*
    private InputHandler_Mattias inputHandler;
    public InputHandler_Mattias InputHandler
    {
        get
        {
            if(inputHandler == null)
            {
                inputHandler = gameObject.GetComponent<InputHandler_Mattias> ();
            }
            return inputHandler;
        }
    }
    */
}
