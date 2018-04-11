using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScript : MonoBehaviour {
    [SerializeField]
    private bool toggle;

    public void ValueChange()
    {
        toggle = !toggle;
    }

    public bool GetValue()
    {
        return toggle;
    }
}
