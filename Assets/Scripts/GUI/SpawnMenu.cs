using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class SpawnMenu : NetworkBehaviour {

    [SerializeField] private Text countdownTimerText;
    [SerializeField] [SyncVar] private float countdownTimer = 5f;

    [SerializeField] private Button[] selectButtons;
    //[SerializeField] private GameObject[] characterPrefabs;
    [HideInInspector] public string characterToUse;

    PlayerNetwork myPlayer;
    private GameObject spawnManuObject;
    private bool hasStarted = false;

    private void Start()
    {
        spawnManuObject = transform.GetChild(0).gameObject;
        StartCoroutine(LateStart(1f));  // Temp solution. Perhaps add players to list when joined?
    }

    IEnumerator LateStart(float _time)
    {
        yield return new WaitForSeconds(_time);
        PlayerNetwork[] players = FindObjectsOfType<PlayerNetwork>();   // Get reference to all NetworkPlayers
        Debug.Log(players.Length);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<NetworkIdentity>().isLocalPlayer)   // Get reference to this clients NetworkPlayer
                myPlayer = players[i];
        }
    }

    // This function is run from a button, and decides what playerObject/character to spawn.
    public void SelectCharacter(int _id)
    {
        Debug.Log("Selected character: " + (_id + 1).ToString());
        //myPlayer.playerObjectPrefab = characterPrefabs[_id];  // CORRECT WAY

        switch (_id)
        {
            case 0:
                characterToUse = "Player - Blue";
                break;
            case 1:
                characterToUse = "Player - Green";
                break;
            case 2:
                characterToUse = "Player - Red";
                break;
            case 3:
                characterToUse = "Player - Yellow";
                break;
            default:
                characterToUse = "Player - Blue";
                break;
        }
    }

    void Update()
    {
        if(!hasStarted) // Match has not started
        {
            if (isServer) 
                countdownTimer -= Time.deltaTime;

            if (countdownTimer <= 0)
            {
                // Match has started
                countdownTimer = 0;
                if (characterToUse == null)        // If a character has not been selected, default it to character 1
                    characterToUse = "Player - Blue";
                myPlayer.characterName = characterToUse;
                myPlayer.canSpawn = true;
                hasStarted = true;
                spawnManuObject.SetActive(false);
            }
            countdownTimerText.text = "Match starts in " + Mathf.RoundToInt(countdownTimer) + "s";
        }
    }
}
