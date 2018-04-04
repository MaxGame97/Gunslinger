using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickerUpper : MonoBehaviour {

    private List<int> inventorySlots = new List<int>();

    private void Start()
    {
        inventorySlots.Add(0);
        inventorySlots.Add(0);
        inventorySlots.Add(0);
    }

    public void Pick(int type, int index)
    {
        if (index > 0 && type > 0)
        {
            inventorySlots.Insert(type, index);
        }
    }
}
