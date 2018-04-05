using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour {

    [SerializeField] private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    private int maxHealth = 100;                                            //The players maximum health
    [SyncVar(hook = "OnHealthChanged")] public int currentHealth;           //The players current health

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

    [ClientRpc]
    public void RpcTakeDamage(int _damage, GameObject shooter)
    {
        currentHealth -= _damage;
        if (currentHealth <= 0)
        {
            //Die, killed by shooter
            if (isServer)
                RpcDisablePlayerObject();

            if (currentHealth < 0)
                currentHealth = 0;
        }
 
        UpdateHealthText();
    }

    void UpdateHealthText()
    {
        if(hasAuthority)
            healthText.text = "Health: " + currentHealth;
    }

    void OnHealthChanged(int newHealth)
    {
        currentHealth = newHealth;
        UpdateHealthText();

        if (currentHealth <= 0)
            if (isServer)
                RpcDisablePlayerObject();

    }

    [ClientRpc]
    void RpcDisablePlayerObject()
    {
        currentHealth = 0;
        UpdateHealthText();

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        //Get collider seperately and disable if one is found.
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;
    }

    [ClientRpc]
    void RpcRespawnPlayerObject()
    {
        currentHealth = maxHealth;
        UpdateHealthText();

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
        UpdateHealthText();
    }
}
