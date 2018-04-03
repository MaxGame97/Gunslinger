using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PickerUpper p = other.gameObject.GetComponent<PickerUpper>();
            if (p != null)
            {
                p.Pick();
                Destroy(this.gameObject);
            }
        }
    }
}
