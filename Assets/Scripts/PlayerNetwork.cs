﻿using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

public class PlayerNetwork : NetworkBehaviour {


    [SerializeField] private GameObject playerObjectPrefab;             //The player object to spawn (the one the player controls
    private string playerName;                                          //The players nickname
    private GameObject playerObject;                                    //Reference to the spawned player object                 
    [SyncVar] bool playerObjectState;                                   //Keeps track of the player objects state. 


    private void Start()
    {
        if (!isLocalPlayer)
        {
            //Disable scripts, cameras, etc.
            return;
        }

        Debug.Log("Connection to client: " + connectionToClient);

        //Spawn my player object
        StartCoroutine(LateStart(0.1f));
    }

    IEnumerator LateStart(float waitTime) //This allows the network to complete set-up before attempting to spawn player.
    {
        yield return new WaitForSeconds(waitTime);

        //Spawn my player object
        CmdSpawnPlayerObject(GetComponent<NetworkIdentity>());
    }

    public void AssignPlayerName(string _name)
    {
        playerName = _name;
    }

    private void Update()
    {
        if(isLocalPlayer)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                CmdDamagePlayer(playerObject.name, gameObject);
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                CmdHealPlayer(playerObject.name);
            }
        }
    }

    [Command]   ///ENDAST FÖR TESTNING
    void CmdDamagePlayer(string _player, GameObject killer)
    {
        GameObject go = GameObject.Find(_player);
        go.GetComponent<PlayerHealth>().RpcTakeDamage(10, killer);
    }

    [Command]   ///ENDAST FÖR TESTNING
    void CmdHealPlayer(string _player)
    {
        GameObject go = GameObject.Find(_player);
        go.GetComponent<PlayerHealth>().GainHealth(10);
    }

    /// <summary>
    /// COMMANDS ONLY GET EXECUTED ON THE SERVER
    /// </summary>

    [Command]   //Perfom function on the server
    private void CmdSpawnPlayerObject(NetworkIdentity id)
    {
        //Instantiate the player object on this client
        playerObject = Instantiate(playerObjectPrefab);
        playerObject.name = "PlayerObject: " + playerName;
        playerObjectState = true;

        //player object now exists on the server, propogate it to all the clients.
        //Since we also set this client to have authority of it! (tells it who owns it)
        NetworkServer.SpawnWithClientAuthority(playerObject, id.connectionToClient);
    }

    [Command]
    private void CmdDisablePlayerObject()
    {
        playerObjectState = false;
    }

    [Command]
    private void CmdEnablePlayerObject()
    {
        playerObjectState = true;
    }
}