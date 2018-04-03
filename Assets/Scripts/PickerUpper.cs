using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickerUpper : MonoBehaviour {

    private Transform rb;
    
	void Start () {
        rb = GetComponent<Transform>();
	}

    public void Pick()
    {
        rb.position = Vector3.up;
    }
}
