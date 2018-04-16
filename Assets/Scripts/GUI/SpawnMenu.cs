using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class SpawnMenu : NetworkBehaviour {

    [SerializeField] private Text countdownTimerText;
    [SerializeField] [SyncVar] private float countdownTimer = 5f;

    [SerializeField] private Button[] selectButtons;
    [HideInInspector] public string characterToUse;

    PlayerNetwork myPlayer;
    private GameObject spawnManuObject;
    private bool hasStarted = false;

    private void Start()
    {
        spawnManuObject = transform.GetChild(0).gameObject;
     
        PlayerNetwork[] players = FindObjectsOfType<PlayerNetwork>();   // Get reference to all NetworkPlayers
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<NetworkIdentity>().isLocalPlayer)   // Get reference to this clients NetworkPlayer
                myPlayer = players[i];
        }
    }

    // This function is run from a button, and decides what playerObject/character to spawn.
    public void SelectCharacter(int _id)
    {
        switch (_id)
        {
            case 0:
                characterToUse = "Character 1";
                break;
            case 1:
                characterToUse = "Character 2";
                break;
            case 2:
                characterToUse = "Character 3";
                break;
            case 3:
                characterToUse = "Character 4";
                break;
            default:
                characterToUse = "Character 1";
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
                countdownTimer = 0;
                if (characterToUse == null)        // If a character has not been selected, default it to character 1
                    characterToUse = "Character 1";

                print ("reached");
                myPlayer.characterName = characterToUse;    // Tell player what character we want to use
                myPlayer.canSpawn = true;                   // Tell player he can spawn his object
                hasStarted = true;                          // say match has started
                spawnManuObject.SetActive(false);           // Disable the spawn menu
            }
            countdownTimerText.text = "Match starts in " + Mathf.RoundToInt(countdownTimer) + "s";  // Update the countdown timer
        }
    }
}
