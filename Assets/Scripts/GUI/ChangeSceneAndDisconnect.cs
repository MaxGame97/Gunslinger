using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class ChangeSceneAndDisconnect : MonoBehaviour {

    public NetworkIdentity playerNetworkIdentity;

    public int test;

	public void ChangeAndDC(int scene)
    {
        Network.Disconnect();
        
        GameObject networkManagerGameobject = GameObject.FindGameObjectWithTag("NetworkManager");
        if(networkManagerGameobject != null && playerNetworkIdentity != null)
        {
            Debug.Log(playerNetworkIdentity.isServer);
            if (playerNetworkIdentity.isServer)
            {
                networkManagerGameobject.GetComponent<NetworkManager>().StopHost();
            }
            else
            {
                networkManagerGameobject.GetComponent<NetworkManager>().StopClient();
            }
            Destroy(networkManagerGameobject);
            SceneManager.LoadScene(scene);
        }
        else
        {
            Debug.Log("Couldn't find networkManagerGameobject and/or playerNetworkIdentity");
        }
    }
}
