using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour {

    [HideInInspector] public PlayerNetwork owner;

    private int maxHealth = 100;                                            //The players maximum health
    [SyncVar] public int currentHealth;                                     //The players current health

    Text healthText;

    private void Start()
    {
        GameObject health = GameObject.Find("Health Text");                 // Get reference to the health text
        currentHealth = maxHealth;                                          // Initialize the current health
        if (health != null)
        {
            healthText = health.GetComponent<Text>();
            healthText.text = "Health: " + currentHealth;                   // Initialize the health text
        }
    }

    private void Update()
    {
        if(hasAuthority && healthText != null)
            healthText.text = "Health: " + currentHealth;
    }

    [ClientRpc]
    public void RpcTakeDamage(int _damage)
    {
        if (!hasAuthority)
            return;

        currentHealth -= _damage;

        if (currentHealth <= 0)
        {
            //Die, killed by shooter
            CmdDie(gameObject);

            if (currentHealth < 0)
                currentHealth = 0;
        }
    }

    [Command] //Performed on every client
    void CmdDie(GameObject go)
    {
        currentHealth = 0;
        owner.RpcPlayerDied(go);
       // Debug.Log(killer + " killed " + gameObject.name);   //Display who killed who here?
    }

    public void GainHealth(int _amount)
    {
        currentHealth += _amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}
