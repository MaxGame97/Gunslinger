using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class ChangeSceneAndDisconnect : MonoBehaviour {

    //This is used to find out if the player is host or not
    [Tooltip("Drag the player prefab here")]
    public NetworkIdentity playerNetworkIdentity;

    //Changes the scene, disconnects the player and destroys the NetworkManager. you have to destroy the NetworkManager or 
    //else it will asume that you can still connect while your in the wrong scene
    //The parameter choses what scene to change to
    public void ChangeAndDC(int scene)
    {
        //Disconnects from the network
        Network.Disconnect();

        //Finds the NetworkManager, This is done here because there is a chance that it has been created by the loader and in that case it can not be found imidiatly.
        //It is also only called when you want to change the scene so it only happens at most one time per scene
        GameObject networkManagerGameobject = GameObject.FindGameObjectWithTag("NetworkManager");

        //Makes sure that both the playerNetworkIdentity and the NetworkManager is found
        if (networkManagerGameobject != null && playerNetworkIdentity != null)
        {
            //If this is the host shut down the whole server
            if (playerNetworkIdentity.isServer)
            {
                networkManagerGameobject.GetComponent<NetworkManager>().StopHost();
            }

            //If it is a client just shut it down for yourself
            else
            {
                networkManagerGameobject.GetComponent<NetworkManager>().StopClient();
            }

            //Destroy the NetworkManager, it will be created by the loader if it is needed again
            Destroy(networkManagerGameobject);

            //Loads the new scene
            SceneManager.LoadScene(scene);
        }
        else
        {
            Debug.Log("Couldn't find networkManagerGameobject and/or playerNetworkIdentity");
        }
    }
}
