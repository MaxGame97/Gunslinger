using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour {

    [HideInInspector] private GameObject owner;

    private int maxHealth = 100;                                    //The players maximum health
    [SyncVar] public int currentHealth;                             //The players current health

    Text healthText;

    private void Start()
    {
        GameObject health = GameObject.Find("Health Text");         // Get reference to the health text                    
        if (health != null)
        {
            healthText = health.GetComponent<Text>();               // Get reference to the text component
            healthText.text = "Health: " + currentHealth;           // Initialize the health text
        }
        currentHealth = maxHealth;                                  // Initialize the current health
    }
    private void Update()
    {
        if(hasAuthority && healthText != null)                      // If we have authority and the text component
        {
            if(healthText.text != "Health: " + currentHealth)       // If the health has changed, we update the UI
                healthText.text = "Health: " + currentHealth;
        }
    }
  
    public void GainHealth(int _amount) 
    {
        currentHealth += _amount;           // Increase health by _amount
        if (currentHealth > maxHealth)      // If health exceeds maxHealth, clamp it
            currentHealth = maxHealth;
    }

    public void SetOwner(GameObject _owner) // Used to assign the owner (PlayerNetwork that spawned this playerObject)
    {
        owner = _owner;
    }

    /// <summary>
    /// COMMANDs
    /// </summary>
    [Command]
    void CmdDie(GameObject _owner, GameObject _playerObject, GameObject killer)
    {
        currentHealth = 0;      // If we died, set the health to 0.

        if (_owner != null)
            _owner.GetComponent<PlayerNetwork>().CmdPlayerDied(_owner, _playerObject, killer);  //Tell the owner that this playerObject died.
    }

    /// <summary>
    /// CLIENTRPCs
    /// </summary>
    [ClientRpc]
    public void RpcTakeDamage(int _damage, GameObject killer)
    {
        if (hasAuthority)
        {
            currentHealth -= _damage;       // Reduce health by _damage
            if (currentHealth <= 0)         // If health drops below 0, we died
            {
                if (currentHealth < 0)
                    currentHealth = 0;
                //if(isClient)
                CmdDie(owner, gameObject, killer);  // Tell the server that we died, who our owner is and who killed us.
            }
        }
    }
}
