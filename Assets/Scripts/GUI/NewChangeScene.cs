using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace Prototype.NetworkLobby
{
    public class NewChangeScene : MonoBehaviour
    {

        public LobbyManager lob;

        public GameObject pausePanel;

        //Changes the scene, disconnects the player and destroys the NetworkManager. you have to destroy the NetworkManager or 
        //else it will asume that you can still connect while your in the wrong scene
        //The parameter choses what scene to change to
        public void ChangeAndDC(int scene)
        {
            if (lob)
            {
                lob.backDelegate();
                pausePanel.SetActive(false);
            }
            else
                Debug.Log("still doesn't work");
        }
    }
}