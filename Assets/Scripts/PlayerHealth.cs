using UnityEngine.Networking;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour {

    private int maxHealth = 100;                    //The players maximum health
    [SyncVar] public int currentHealth;            //The players current health
     

    private void Start()
    {
        currentHealth = maxHealth;
    }

    [Command]
    public void CmdTakeDamage(int _damage, GameObject shooter)
    {
        if (!isServer)
            return;

        if(_damage > currentHealth)
        {
            //Die, killed by shooter
            gameObject.SetActive(false);
        }
        else
        {
            currentHealth -= _damage;
        }
    }

    [Command]
    public void CmdGainHealth(int _amount)
    {
        if (!isServer)
            return;

        currentHealth += _amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }
}
