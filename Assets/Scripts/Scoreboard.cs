using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour {

    private List<ScoreboardInfo> children = new List<ScoreboardInfo>();
    [SerializeField] private Transform playerList;
    [SerializeField] private GameObject playerInfoPrefab;

    private void OnEnable()
    {
        PlayerNetwork[] temp = FindObjectsOfType<PlayerNetwork>();
        //PlayerNetwork[] temp = GameManager.instance.GetPlayers();   //Store reference to the gm's player list
        if (temp == null)
            return;
        if (temp.Length != children.Count)   //Amount of players does not match with the GameManagers amount, update it!
        {
            //Remove all players from scoreboard
            for (int i = 0; i < playerList.childCount; i++)
            {
                Destroy(playerList.GetChild(i));
            }

            //Re-populate the scoreboard with players
            for (int i = 0; i < temp.Length; i++)
            {
                GameObject go = Instantiate(playerInfoPrefab, playerList);
                ScoreboardInfo info = go.GetComponent<ScoreboardInfo>();

                info.AssignPlayerNetwork(temp[i]);
                children.Add(info);
            }
        }
    }

    public void UpdateScoreboard(Transform _child)
    {
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
}
