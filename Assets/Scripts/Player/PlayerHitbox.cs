using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour {

    
    [SerializeField] private float damageMultiplier = 1;    // Damage multiplier that can be tweaked on different hitboxes of a character etc. 
    [SerializeField] private bool isHead;                   // isHead is used to check for headshots.

    public float DamageMultiplier { get { return damageMultiplier; } }
    public bool IsHead { get { return isHead; } }


}
