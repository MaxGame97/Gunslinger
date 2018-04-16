using UnityEngine;

public class PlayerUI : MonoBehaviour {

    private GameObject scoreboard;

	// Use this for initialization
	void Start () {
        GameObject _scoreboard = GameObject.Find("Scoreboard - Canvas");
        if (_scoreboard != null)
            scoreboard = _scoreboard.transform.GetChild(0).gameObject;
	}

	void Update () {
        ScoreboardInput();
	}

    void ScoreboardInput()
    {
        if(!GameManager.instance.gameHasEnded)  // Perform ONLY if the game has not ended yet.
        {
            if (Input.GetKeyDown(KeyCode.Tab))   //Enable scoreboard if Tab is pressed
            {
                scoreboard.SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.Tab))   //Disable scoreboard if Tab is released
            {
                scoreboard.SetActive(false);
            }
        }
    }
}
