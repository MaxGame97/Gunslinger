using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour {

    [SerializeField] private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    private int maxHealth = 100;                                            //The players maximum health
    [SyncVar] public int currentHealth;                                     //The players current health

    Text healthText;

    private void Start()
    {
        healthText = GameObject.Find("Health Text").GetComponent<Text>();   // Get reference to the health text
        currentHealth = maxHealth;                                          // Initialize the current health
        healthText.text = "Health: " + currentHealth;                       // Initialize the health text
    }

    public void Setup()
    {
        wasEnabled = new bool[disableOnDeath.Length];

        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }
    }

    private void Update()
    {
        if(hasAuthority)
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
                RpcDisablePlayerObject(shooter.gameObject.name);

            if (currentHealth < 0)
                currentHealth = 0;
        }
    }

    //void OnHealthChanged(int newHealth)
    //{
    //    currentHealth = newHealth;

    //    if (currentHealth <= 0)
    //        if (isServer)
    //            RpcDisablePlayerObject();
    //}

    [ClientRpc]
    void RpcDisablePlayerObject(string killer)
    {
        currentHealth = 0;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        //Get collider seperately and disable if one is found.
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;

        Debug.Log(gameObject.name + " died");
    }

    [ClientRpc]
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

   
    public void GainHealth(int _amount)
    {
        currentHealth += _amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}
