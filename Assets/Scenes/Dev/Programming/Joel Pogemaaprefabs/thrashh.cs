using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thrashh : MonoBehaviour {

    public GameObject boxes;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Instantiate(boxes);
        }

    }
}
