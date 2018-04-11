using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour {

    // Damage multiplier that can be tweaked on different hitboxes of a character etc. 
    [SerializeField] private float damageMultiplier = 1;
    public float DamageMultiplier { get { return damageMultiplier; } }

}
