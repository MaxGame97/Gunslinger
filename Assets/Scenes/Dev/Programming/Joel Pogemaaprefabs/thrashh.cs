using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class thrashh : NetworkBehaviour {

    public GameObject boxes;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F))
        {
            CmdspawnBoxes();
        }
    }

    [Command]
    private void CmdspawnBoxes()
    {
        var boxs = Instantiate(boxes);
        NetworkServer.Spawn(boxs);
    }
}
