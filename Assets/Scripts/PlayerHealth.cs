using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour {

    [HideInInspector] public PlayerNetwork owner;
    [SerializeField] private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    private int maxHealth = 100;                                            //The players maximum health
    [SyncVar] public int currentHealth;                                     //The players current health

    Text healthText;

    private void Start()
    {
        healthText = GameObject.Find("Health Text").GetComponent<Text>();   // Get reference to the health text
        currentHealth = maxHealth;                                          // Initialize the current health
        if (healthText != null)
            healthText.text = "Health: " + currentHealth;                   // Initialize the health text
    }

    private void Update()
    {
        if(hasAuthority && healthText != null)
            healthText.text = "Health: " + currentHealth;
    }

    [ClientRpc]
    public void RpcTakeDamage(int _damage, NetworkIdentity shooter)
    {
        if (!hasAuthority)
            return;

        currentHealth -= _damage;
        if (currentHealth <= 0)
        {
            //Die, killed by shooter
            if (isServer)
                CmdDie(shooter.gameObject.name);

            if (currentHealth < 0)
                currentHealth = 0;
        }
    }

    [Command] //Performed on every client
    void CmdDie(string killer)
    {
        currentHealth = 0;

        owner.RpcPlayerDied();
        Debug.Log(killer + " killed " + gameObject.name);   //Display who killed who here?
    }

    [ClientRpc] //Performed on every client
    void RpcRespawnPlayerObject()
    {
        currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //Get collider seperately and enable if one is found.
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;
    }

    [ClientRpc] //Performed on every client
    public void RpcGainHealth(int _amount)
    {
        currentHealth += _amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}
