using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour {

    private List<ScoreboardInfo> children = new List<ScoreboardInfo>();
    [SerializeField] private Transform playerList;
    [SerializeField] private GameObject playerInfoPrefab;

    private void OnEnable()
    {
        if(!playerList)
        {
            Debug.LogError("NO PLAYER LIST ASSIGNED");
            return;
        }

        if(!playerInfoPrefab)
        {
            Debug.LogError("NO PLAYER INFO PREFAB");
            return;
        }

        PlayerNetwork[] temp = FindObjectsOfType<PlayerNetwork>();  // Get reference to all players

        if (temp == null)   // If the reference is null, return
            return;

        if (temp.Length != children.Count)   //Amount of players in list does not match with the amount of actual players, update it!
        {
            //Remove all players from scoreboard
            for (int i = 0; i < playerList.childCount; i++)
            {
                Destroy(playerList.GetChild(i));    // Destroy all spawned UI objects
            }
            children.Clear();   // Clear the list

            //Re-populate the scoreboard with players
            for (int i = 0; i < temp.Length; i++)
            {
                GameObject go = Instantiate(playerInfoPrefab, playerList);  // Instantiate UI object
                ScoreboardInfo info = go.GetComponent<ScoreboardInfo>();    // Get reference to the info

                info.AssignPlayerNetwork(temp[i]);  // Set the UI's player reference
                children.Add(info); // Add the info to the list
            }
        }
    }

    public void UpdateScoreboard(Transform _child)
    {
        //CheckGameState();
        // We only want to update the list if the child at the top of the list
        if (_child.GetSiblingIndex() == 0)
            return;
        // We only want to update the list if the kill count is GREATER than the next one.
        if (children[_child.GetSiblingIndex()].Kills < children[_child.GetSiblingIndex() - 1].Kills) 
            return;

        for (int i = _child.GetSiblingIndex(); i > 0; i--)
        {
            if (children[i].Kills > children[i - 1].Kills)// If current object has more kills then object above
            {
                children[i].transform.SetSiblingIndex(i - 1);  // Swap places
                children[i - 1].transform.SetSiblingIndex(i + 1);  // Swap places

                // Update List of children
                children.Clear();
                children.AddRange(playerList.GetComponentsInChildren<ScoreboardInfo>());
            }
        }
    }
    

    // ---WILL BE USED FOR WIN-CONDITIONS---

    //void CheckGameState()
    //{
    //    List<ScoreboardInfo> deadPlayers = new List<ScoreboardInfo>();
    //    for (int i = 0; i < children.Count; i++)
    //    {
    //        if (!children[i].IsDead)
    //            deadPlayers.Add(children[i]);
    //    }

    //    if (deadPlayers.Count == children.Count - 1) // Game is only over after all but one is dead.
    //    {
    //        for (int i = 0; i < deadPlayers.Count; i++)
    //        {
    //            children.Remove(deadPlayers[i]);
    //        }
    //        Debug.Log("The game has ended! " + children[0].PlayerName + " won!");
    //    }
    //}
}
